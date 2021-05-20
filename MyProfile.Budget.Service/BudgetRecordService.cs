using Common.Service;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.AreaAndSection;
using MyProfile.Entity.ModelView.BudgetView;
using MyProfile.Entity.ModelView.Currency;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Tag.Service;
using MyProfile.User.Service;
using MyProfile.UserLog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using RecordTag = MyProfile.Entity.ModelView.RecordTag;

namespace MyProfile.Budget.Service
{
    public class BudgetRecordService
    {
        private IBaseRepository repository;
        private CollectionUserService collectionUserService;
        private UserLogService userLogService;
        private TagService tagService;
        private AccountService accountService;
        private SectionService sectionService;
        private CurrencyService currencyService;
        private IMemoryCache cache;

        public BudgetRecordService(IBaseRepository repository,
            TagService tagService,
            AccountService accountService,
            IMemoryCache cache,
            SectionService sectionService,
            CurrencyService currencyService)
        {
            this.repository = repository;
            this.collectionUserService = new CollectionUserService(repository);
            this.userLogService = new UserLogService(repository);
            this.tagService = tagService;
            this.accountService = accountService;
            this.sectionService = sectionService;
            this.currencyService = currencyService;
            this.cache = cache;
        }

        public async Task<RecordModelView> GetByID(int id)
        {
            return await repository.GetAll<Record>(x => x.ID == id && x.UserID == UserInfo.Current.ID)
                .Select(x => new RecordModelView
                {
                    ID = id,
                    IsCorrect = true,
                    IsSaved = true,
                    Money = x.Total,
                    SectionID = x.BudgetSectionID,
                    SectionName = x.BudgetSection.Name,
                    Tag = x.RawData,
                    DateTimeOfPayment = x.DateTimeOfPayment,
                    Tags = x.Tags
                      .Select(y => new RecordTag
                      {
                          ID = y.UserTagID,
                          Title = y.UserTag.Title,
                          IsDeleted = y.UserTag.IsDeleted,
                          DateCreate = y.UserTag.DateCreate,
                      })
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateOrUpdate(RecordsModelView budgetRecord)
        {
            UserInfoModel currentUser = UserInfo.Current;
            bool isEdit = false;
            bool isCreate = false;
            DateTime now = DateTime.Now.ToUniversalTime();
            List<RecordTag> newUserTags = new List<RecordTag>();
            List<long> errorLogCreateIDs = new List<long>();
            List<long> errorLogEditIDs = new List<long>();
            budgetRecord.DateTimeOfPayment = new DateTime(budgetRecord.DateTimeOfPayment.Year, budgetRecord.DateTimeOfPayment.Month, budgetRecord.DateTimeOfPayment.Day, 13, 0, 0);
            decimal recordCashback = 0;
            decimal _money = 0;
            List<Account> accounts = new List<Account>();
            List<SectionLightModelView> sections = (await sectionService.GetAllSectionByUser()).ToList();
            bool isSpending = false;
            CurrencyRateHistory accountCurrency;
            List<RecordHistory> histories = new List<RecordHistory>();
            RecordHistory history;

            //not use action with cachback if the DateTimeOfPayment on this month
            bool isThisMonth = (new DateTime(budgetRecord.DateTimeOfPayment.Year, budgetRecord.DateTimeOfPayment.Month, 1)).Date <= now.Date
                && now.Date <= (new DateTime(budgetRecord.DateTimeOfPayment.Year, budgetRecord.DateTimeOfPayment.Month, DateTime.DaysInMonth(budgetRecord.DateTimeOfPayment.Year, budgetRecord.DateTimeOfPayment.Month))).Date;

            foreach (var record in budgetRecord.Records.Where(x => x.IsCorrect))
            {
                if (currentUser.IsAvailable == false)
                {
                    record.IsSaved = false;
                    continue;
                }
                recordCashback = 0;
                _money = 0;
                Account account = accounts.FirstOrDefault(x => x.ID == record.AccountID);
                history = new RecordHistory();


                SectionLightModelView section = sections.FirstOrDefault(x => x.ID == record.SectionID);

                try
                {
                    isSpending = section?.SectionTypeID == (int?)SectionTypeEnum.Spendings;

                    if (account == null)
                    {
                        account = await repository.GetAll<Account>(x => x.ID == record.AccountID)
                           .FirstOrDefaultAsync();

                        accounts.Add(account);
                    }
                    history.AccountOldBalance = account.Balance;
                    history.AccountOldBalanceCashback = account.CachbackBalance;

                    if (isSpending
                        && record.Money >= 100
                        && account.IsCachback
                        && section.IsCashback
                        && account.CachbackForAllPercent != null)
                    {
                        recordCashback = (record.Money * account.CachbackForAllPercent ?? 1) / 100;
                    }
                    if (record.Tags.Count() > 0)
                    {
                        record.Description = await tagService.ParseAndCreateDescription(record.Description, record.Tags, newUserTags);
                    }
                }
                catch (Exception ex)
                {
                    record.IsSaved = false;
                    record.isAnyError = true;

                    isCreate = record.ID <= 0;
                    isEdit = record.ID > 0;

                    if (isCreate)
                    {
                        errorLogCreateIDs.Add(await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "BudgetRecord_Parse", ex));
                    }

                    if (isEdit)
                    {
                        errorLogEditIDs.Add(await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "BudgetRecord_Parse", ex));
                    }
                }

                if (record.isAnyError == false)
                {
                    if (record.ID <= 0)// create
                    {
                        try
                        {
                            if (record.SectionID <= 0)
                            {
                                errorLogCreateIDs.Add(await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "BudgetRecord_Create", new Exception(), "SectionID == 0"));
                            }
                            else
                            {
                                var newRecord = new Record
                                {
                                    UserID = currentUser.ID,
                                    BudgetSectionID = record.SectionID,
                                    AccountID = record.AccountID,
                                    DateTimeCreate = now,
                                    DateTimeEdit = now,
                                    DateTimeOfPayment = budgetRecord.DateTimeOfPayment,
                                    Description = record.Description,
                                    IsHide = false,
                                    Total = record.Money,
                                    RawData = record.Tag,
                                    CurrencyID = record.CurrencyID,
                                    CurrencyRate = record.CurrencyRate,
                                    CurrencyNominal = record.CurrencyNominal ?? 1,
                                    IsShowForCollection = budgetRecord.IsShowInCollection,
                                    Cashback = recordCashback,
                                    Tags = record.Tags
                                        .Select(x => new Entity.Model.RecordTag
                                        {
                                            DateSet = now,
                                            UserTagID = x.ID
                                        })
                                        .ToList(),
                                };

                                await repository.CreateAsync(newRecord, true);

                                record.IsSaved = true;

                                #region Account 
                                accountCurrency = new CurrencyRateHistory
                                {
                                    CurrencyID = account.CurrencyID ?? 0,
                                    Date = newRecord.DateTimeOfPayment.Date,
                                };
                                history.RecordCachback = recordCashback;

                                if ((record.CurrencyID == currentUser.CurrencyID && currentUser.CurrencyID == account.CurrencyID)
                                    ||
                                    (record.CurrencyID != currentUser.CurrencyID && currentUser.CurrencyID == account.CurrencyID))
                                {
                                    _money = record.Money;
                                }
                                else if (record.CurrencyID != currentUser.CurrencyID && record.CurrencyID == account.CurrencyID)
                                {
                                    accountCurrency.Rate = record.CurrencyRate ?? 1;
                                    accountCurrency.Nominal = record.CurrencyNominal ?? 1;

                                    _money = record.Money / (record.CurrencyRate ?? 1);
                                }
                                else //When record.CurrencyID != currentUser.CurrencyID != account.CurrencyID
                                {
                                    var val = await currencyService.GetRateByCodeAsync(newRecord.DateTimeOfPayment, account.Currency.CodeName_CBR, currentUser.UserSessionID);

                                    if (val != null)
                                    {
                                        accountCurrency.Rate = val.Rate;
                                        accountCurrency.Nominal = val.Nominal;

                                        _money = record.Money / accountCurrency.Rate;
                                    }
                                    else
                                    {
                                        record.isAnyError = true;
                                        record.Error = "Не удалось списать средства со счета: " + account.Name;
                                        _money = 0;
                                        recordCashback = 0;

                                        await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "BudgetRecord_Create", new Exception(),
                                            $"account.Currency.CodeName_CBR: { account.Currency.CodeName_CBR}, newRecord.DateTimeOfPayment: {newRecord.DateTimeOfPayment } ");
                                    }
                                }

                                if (isSpending)
                                {
                                    account.Balance -= _money;

                                    if (isSpending && account.IsCachback && account.CachbackForAllPercent != null && recordCashback != 0 && section.IsCashback)
                                    {
                                        if (record.CurrencyID != account.CurrencyID && account.CurrencyID != currentUser.CurrencyID)
                                        {
                                            recordCashback = ((record.Money / accountCurrency.Rate) * (account.CachbackForAllPercent ?? 1)) / 100;
                                        }

                                        if (isThisMonth)//update CashbackBalance only for this month
                                        {
                                            account.CachbackBalance += recordCashback;
                                        }
                                    }
                                    else
                                    {
                                        recordCashback = 0;
                                    }

                                }
                                else
                                {
                                    account.Balance += _money;
                                }

                                history.ActionTypeCode = RecordActionTypeCode.Create;
                                history.RecordID = newRecord.ID;
                                history.AccountID = account.ID;
                                history.DateTimeOfPayment = newRecord.DateTimeOfPayment;
                                history.DateCreate = now;
                                history.RecordCurrencyID = newRecord.CurrencyID ?? 0;
                                history.RacordCurrencyRate = newRecord.CurrencyRate;
                                history.RecordCurrencyNominal = newRecord.CurrencyNominal;
                                history.AccountCurrencyID = account.CurrencyID ?? 0;
                                history.AccountCurrencyNominal = accountCurrency.Nominal;
                                history.AccountCurrencyRate = accountCurrency.Rate;
                                history.SectionID = newRecord.BudgetSectionID;
                                history.RecordTotal = newRecord.Total;
                                history.AccountNewBalance = account.Balance;
                                history.AccountNewBalanceCashback = account.CachbackBalance;
                                history.AccountTotal = _money;
                                history.AccountCashback = recordCashback;

                                histories.Add(history);
                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            record.IsSaved = false;

                            errorLogCreateIDs.Add(await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "BudgetRecord_Create", ex));
                        }

                        isCreate = true;
                    }
                    else
                    {//edit
                        try
                        {
                            if (record.SectionID <= 0)
                            {
                                errorLogEditIDs.Add(await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "BudgetRecord_Edit", new Exception(), "SectionID == 0"));
                                record.IsSaved = false;
                            }
                            else
                            {
                                var dbRecord = repository.GetAll<Record>(x => x.ID == record.ID).FirstOrDefault(); ;
                                long oldAccountID = dbRecord.AccountID ?? -1,
                                    oldSectionTypeID = dbRecord.BudgetSection.SectionTypeID ?? 0;
                                decimal oldTotal = dbRecord.Total,
                                    oldCashback = dbRecord.Cashback;
                                bool isChangeAccount = record.AccountID != dbRecord.AccountID;


                                dbRecord.BudgetSectionID = record.SectionID;
                                dbRecord.Total = record.Money;
                                dbRecord.RawData = record.Tag;
                                dbRecord.DateTimeOfPayment = budgetRecord.DateTimeOfPayment;
                                dbRecord.Description = record.Description;
                                dbRecord.CurrencyID = record.CurrencyID;
                                dbRecord.CurrencyNominal = record.CurrencyNominal ?? 1;
                                dbRecord.CurrencyRate = record.CurrencyRate;
                                dbRecord.IsShowForCollection = budgetRecord.IsShowInCollection;
                                dbRecord.DateTimeEdit = now;
                                dbRecord.AccountID = record.AccountID;
                                dbRecord.Cashback = recordCashback;

                                foreach (var newTag in tagService.CheckTags(dbRecord.Tags, record.Tags.ToList()))
                                {
                                    dbRecord.Tags.Add(new Entity.Model.RecordTag { DateSet = now, UserTagID = newTag.ID });
                                }

                                await repository.UpdateAsync(dbRecord, true);
                                record.IsSaved = true;

                                #region Account

                                accountCurrency = new CurrencyRateHistory
                                {
                                    CurrencyID = account.CurrencyID ?? 0,
                                    Date = dbRecord.DateTimeOfPayment.Date,
                                };

                                if (isChangeAccount == false)
                                {
                                    #region OLD Return back balance and cashback

                                    RecordHistory lastAccountRecordHistory = dbRecord.RecordHistories
                                           .Where(x => x.ActionTypeCode == RecordActionTypeCode.Create || x.ActionTypeCode == RecordActionTypeCode.Edit)
                                           .OrderByDescending(x => x.ID)
                                           .FirstOrDefault();

                                    if (oldSectionTypeID == (int)SectionTypeEnum.Spendings)
                                    {
                                        account.Balance += lastAccountRecordHistory.AccountTotal;// _money;

                                        if (isThisMonth)
                                        {
                                            account.CachbackBalance -= lastAccountRecordHistory.AccountCashback;
                                        }
                                    }
                                    else
                                    {
                                        account.Balance -= lastAccountRecordHistory.AccountTotal;
                                    }
                                    #endregion


                                    #region NEW action with balance and cashback

                                    if ((record.CurrencyID == currentUser.CurrencyID && currentUser.CurrencyID == account.CurrencyID)
                                         ||
                                        (record.CurrencyID != currentUser.CurrencyID && currentUser.CurrencyID == account.CurrencyID))
                                    {
                                        _money = record.Money;
                                    }
                                    else if (record.CurrencyID != currentUser.CurrencyID && record.CurrencyID == account.CurrencyID)
                                    {
                                        accountCurrency.Rate = record.CurrencyRate ?? 1;
                                        accountCurrency.Nominal = record.CurrencyNominal ?? 1;

                                        _money = record.Money / (record.CurrencyRate ?? 1);
                                    }
                                    else //When record.CurrencyID != currentUser.CurrencyID != account.CurrencyID
                                    {
                                        var val = await currencyService.GetRateByCodeAsync(dbRecord.DateTimeOfPayment.Date, account.Currency.CodeName_CBR, currentUser.UserSessionID);

                                        if (val != null)
                                        {
                                            accountCurrency.Rate = val.Rate;
                                            accountCurrency.Nominal = val.Nominal;

                                            _money = record.Money / accountCurrency.Rate;
                                        }
                                        else
                                        {
                                            record.isAnyError = true;
                                            record.Error = "Не удалось списать средства со счета: " + account.Name;
                                            _money = 0;
                                            recordCashback = 0;
                                        }
                                    }

                                    if (isSpending)//new section type
                                    {
                                        account.Balance -= _money;

                                        if (isSpending && account.IsCachback && account.CachbackForAllPercent != null && recordCashback != 0)
                                        {
                                            if (record.CurrencyID != account.CurrencyID && account.CurrencyID != currentUser.CurrencyID)
                                            {
                                                recordCashback = ((record.Money / accountCurrency.Rate) * (account.CachbackForAllPercent ?? 1)) / 100;
                                            }
                                            if (isThisMonth)
                                            {
                                                account.CachbackBalance += recordCashback;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        account.Balance += _money;
                                    }
                                    #endregion
                                }
                                else
                                {

                                    Account oldAccount = accounts.FirstOrDefault(x => x.ID == oldAccountID);
                                    if (oldAccount == null)
                                    {
                                        oldAccount = await repository.GetAll<Account>(x => x.ID == oldAccountID)
                                           .FirstOrDefaultAsync();

                                        accounts.Add(oldAccount);
                                    }

                                    RecordHistory lastAccountRecordHistory = dbRecord.RecordHistories
                                         .Where(x => (x.ActionTypeCode == RecordActionTypeCode.Create
                                              || x.ActionTypeCode == RecordActionTypeCode.Edit)
                                              && x.AccountID == oldAccount.ID)
                                         .OrderByDescending(x => x.ID)
                                         .FirstOrDefault();

                                    #region OLD Return back balance and cashback

                                    if (oldSectionTypeID == (int)SectionTypeEnum.Spendings)
                                    {
                                        oldAccount.Balance += lastAccountRecordHistory.AccountTotal; // _money;

                                        if (isThisMonth)
                                        {
                                            oldAccount.CachbackBalance -= lastAccountRecordHistory.AccountCashback;
                                        }
                                    }
                                    else
                                    {
                                        oldAccount.Balance -= lastAccountRecordHistory.AccountTotal; ;// _money;
                                    }

                                    #endregion

                                    #region NEW New action with balance and cashback

                                    if ((record.CurrencyID == currentUser.CurrencyID && currentUser.CurrencyID == account.CurrencyID)
                                         ||
                                        (record.CurrencyID != currentUser.CurrencyID && currentUser.CurrencyID == account.CurrencyID))
                                    {
                                        _money = record.Money;
                                    }
                                    else if (record.CurrencyID != currentUser.CurrencyID && record.CurrencyID == account.CurrencyID)
                                    {
                                        accountCurrency.Rate = record.CurrencyRate ?? 1;
                                        accountCurrency.Nominal = record.CurrencyNominal ?? 1;

                                        _money = record.Money / (record.CurrencyRate ?? 1);
                                    }
                                    else //When record.CurrencyID != currentUser.CurrencyID != account.CurrencyID
                                    {
                                        var val = await currencyService.GetRateByCodeAsync(dbRecord.DateTimeOfPayment.Date, account.Currency.CodeName_CBR, currentUser.UserSessionID);

                                        if (val != null)
                                        {
                                            accountCurrency.Rate = val.Rate;
                                            accountCurrency.Nominal = val.Nominal;

                                            _money = record.Money / accountCurrency.Rate;
                                        }
                                        else
                                        {
                                            record.isAnyError = true;
                                            record.Error = "Не удалось списать средства со счета: " + account.Name;
                                            _money = 0;
                                            recordCashback = 0;
                                        }
                                    }

                                    if (isSpending)// new section type
                                    {
                                        account.Balance -= _money;

                                        if (isSpending && account.IsCachback && account.CachbackForAllPercent != null && recordCashback != 0 && section.IsCashback)
                                        {
                                            if (record.CurrencyID != account.CurrencyID && account.CurrencyID != currentUser.CurrencyID)
                                            {
                                                recordCashback = ((record.Money / accountCurrency.Rate) * (account.CachbackForAllPercent ?? 1)) / 100;
                                            }
                                            if (isThisMonth)
                                            {
                                                account.CachbackBalance += recordCashback;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        account.Balance += _money;
                                    }
                                    #endregion
                                }

                                history.ActionTypeCode = RecordActionTypeCode.Edit;
                                history.RecordID = dbRecord.ID;
                                history.AccountID = account.ID;
                                history.DateTimeOfPayment = dbRecord.DateTimeOfPayment;
                                history.DateCreate = now;
                                history.RecordCurrencyID = dbRecord.CurrencyID ?? 0;
                                history.RacordCurrencyRate = dbRecord.CurrencyRate;
                                history.RecordCurrencyNominal = dbRecord.CurrencyNominal;
                                history.AccountCurrencyID = account.CurrencyID ?? 0;
                                history.AccountCurrencyNominal = accountCurrency.Nominal;
                                history.AccountCurrencyRate = accountCurrency.Rate;
                                history.SectionID = dbRecord.BudgetSectionID;
                                history.RecordTotal = dbRecord.Total;
                                history.RecordCachback = dbRecord.Cashback;
                                history.AccountNewBalance = account.Balance;
                                history.AccountNewBalanceCashback = account.CachbackBalance;
                                history.AccountTotal = _money;
                                history.AccountCashback = recordCashback;

                                histories.Add(history);
                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            record.IsSaved = false;
                            errorLogEditIDs.Add(await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "BudgetRecord_Edit", ex));
                        }
                        isEdit = true;
                    }
                }
            }
            if (histories.Count > 0)
            {
                repository.CreateRange(histories);
            }

            await repository.SaveAsync();

            if (isEdit)
            {
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Record_Edit, errorLogIDs: errorLogEditIDs);
            }
            if (isCreate)
            {
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Record_Create, errorLogIDs: errorLogCreateIDs);
            }
            if (currentUser.IsAvailable == false)
            {
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Record_IsNotAvailibleUser, errorLogIDs: errorLogCreateIDs);
            }

            if (budgetRecord.Records.Any(x => x.IsSaved == false))
            {
                budgetRecord.NewTags = newUserTags;
            }

            cache.Remove(typeof(Entity.ModelView.Account.AccountShortViewModel).Name + "_" + currentUser.ID);

            if (currentUser.IsHelpRecord)
            {
                currentUser.IsHelpRecord = false;
                await UserInfo.AddOrUpdate_Authenticate(currentUser);
            }

            return true;
        }

        public async Task<bool> RemoveRecord(BudgetRecordModelView record)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();
            var db_record = await repository.GetAll<Record>(x => x.ID == record.ID && x.UserID == currentUser.ID)
                .FirstOrDefaultAsync();
            //not use action with cachback if the DateTimeOfPayment on this month
            bool isThisMonth = (new DateTime(db_record.DateTimeOfPayment.Year, db_record.DateTimeOfPayment.Month, 1)).Date <= now.Date
                && now.Date <= (new DateTime(db_record.DateTimeOfPayment.Year, db_record.DateTimeOfPayment.Month, DateTime.DaysInMonth(db_record.DateTimeOfPayment.Year, db_record.DateTimeOfPayment.Month))).Date;

            if (db_record != null)
            {
                db_record.IsDeleted = true;
                db_record.DateTimeDelete = now;


                #region Account and AccountHistory

                RecordHistory lastAccountRecordHistory = db_record.RecordHistories
                                      .Where(x => x.ActionTypeCode == RecordActionTypeCode.Create || x.ActionTypeCode == RecordActionTypeCode.Edit)
                                      .OrderByDescending(x => x.ID)
                                      .FirstOrDefault();

                if (lastAccountRecordHistory != null)
                {
                    if (db_record.BudgetSection.SectionTypeID == (int)SectionTypeEnum.Spendings)
                    {
                        db_record.Account.Balance += lastAccountRecordHistory.AccountTotal;

                        if (db_record.BudgetSection.SectionTypeID == (int)SectionTypeEnum.Spendings
                            && isThisMonth
                            && db_record.BudgetSection.IsCashback
                            && db_record.Account.IsCachback
                            && db_record.Account.CachbackForAllPercent != null)
                        {
                            db_record.Account.CachbackBalance -= lastAccountRecordHistory.AccountCashback;
                        }
                    }
                    else
                    {
                        db_record.Account.Balance -= lastAccountRecordHistory.AccountTotal;
                    }
                }
                else
                {
                    await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "BudgetRecord_RemoveRecord", new Exception(), "lastAccountRecordHistory is empty");
                    //record.isAnyError = true;
                    //record.Error = "Не удалось списать средства со счета: " + account.Name;
                }

                db_record.RecordHistories.Add(new RecordHistory
                {
                    ActionTypeCode = RecordActionTypeCode.Delete,
                    AccountID = db_record.AccountID,
                    AccountCurrencyID = db_record.Account.CurrencyID,
                    DateCreate = now,
                    SectionID = db_record.BudgetSectionID,
                });
                #endregion

                await repository.UpdateAsync(db_record, true);
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Record_Delete);
                return true;
            }
            return false;
        }

        public async Task<bool> RecoveryRecord(BudgetRecordModelView record)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();
            var db_record = await repository.GetAll<Record>(x => x.ID == record.ID && x.UserID == currentUser.ID).FirstOrDefaultAsync();
            //not use action with cachback if the DateTimeOfPayment on this month
            bool isThisMonth = (new DateTime(db_record.DateTimeOfPayment.Year, db_record.DateTimeOfPayment.Month, 1)).Date <= now.Date
                && now.Date <= (new DateTime(db_record.DateTimeOfPayment.Year, db_record.DateTimeOfPayment.Month, DateTime.DaysInMonth(db_record.DateTimeOfPayment.Year, db_record.DateTimeOfPayment.Month))).Date;


            if (db_record != null)
            {
                db_record.IsDeleted = false;
                db_record.DateTimeDelete = null;

                #region Account and AccountHistory

                RecordHistory lastAccountRecordHistory = db_record.RecordHistories
                                      .Where(x => x.ActionTypeCode == RecordActionTypeCode.Create || x.ActionTypeCode == RecordActionTypeCode.Edit)
                                      .OrderByDescending(x => x.ID)
                                      .FirstOrDefault();

                if (lastAccountRecordHistory != null)
                {
                    if (db_record.BudgetSection.SectionTypeID == (int)SectionTypeEnum.Spendings)
                    {
                        db_record.Account.Balance -= lastAccountRecordHistory.AccountTotal;

                        if (db_record.BudgetSection.SectionTypeID == (int)SectionTypeEnum.Spendings
                            && isThisMonth
                            && db_record.BudgetSection.IsCashback
                            && db_record.Account.IsCachback
                            && db_record.Account.CachbackForAllPercent != null)
                        {
                            db_record.Account.CachbackBalance += lastAccountRecordHistory.AccountCashback;
                        }
                    }
                    else
                    {
                        db_record.Account.Balance += lastAccountRecordHistory.AccountTotal;
                    }
                }
                else
                {
                    await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "BudgetRecord_RecoveryRecord", new Exception(), "lastAccountRecordHistory is empty");
                }

                db_record.RecordHistories.Add(new RecordHistory
                {
                    ActionTypeCode = RecordActionTypeCode.Recovery,
                    AccountID = db_record.AccountID,
                    AccountCurrencyID = db_record.Account.CurrencyID,
                    DateCreate = now,
                    SectionID = db_record.BudgetSectionID,
                    AccountCashback = lastAccountRecordHistory.AccountCashback
                });

                #endregion

                await repository.UpdateAsync(db_record, true);
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Record_Recovery);
                return true;
            }
            return false;
        }

        public IQueryable<IGrouping<int, TmpBudgetRecord>> GetBudgetRecordsGroup(
            DateTime from,
            DateTime to,
            Func<TmpBudgetRecord, int> groupBy,
            Expression<Func<Record, bool>> expression = null)
        {
            return _getBudgetRecords(from, to, expression)
                .Select(x => new TmpBudgetRecord
                {
                    Total = x.Total,
                    DateTimeOfPayment = x.DateTimeOfPayment,
                    SectionID = x.BudgetSectionID,
                    SectionName = x.BudgetSection.Name,
                    SectionTypeID = x.BudgetSection.SectionTypeID,
                    AreaID = x.BudgetSection.BudgetArea.ID,
                    AreaName = x.BudgetSection.BudgetArea.Name,
                    CollectionSectionIDs = x.BudgetSection.CollectiveSections.Select(u => u.ChildSectionID ?? 0),
                })
              .GroupBy(groupBy)
              .AsQueryable();
        }

        public async Task<IQueryable<IGrouping<DateTime, TmpBudgetRecord>>> GetBudgetRecordsGroupByDate(CalendarFilterModels filter)
        {
            return repository.GetAll(await getExpressionByCalendarFilterAsync(filter))
                .Select(x => new TmpBudgetRecord
                {
                    Total = x.Total,
                    DateTimeOfPayment = x.DateTimeOfPayment,
                    //SectionID = x.BudgetSectionID,
                    //SectionName = x.BudgetSection.Name,
                    //AreaID = x.BudgetSection.BudgetArea.ID,
                    //AreaName = x.BudgetSection.BudgetArea.Name,
                    //CollectionSectionIDs = x.BudgetSection.CollectiveSections.Select(u => u.ChildSectionID ?? 0)
                })
              .GroupBy(x => x.DateTimeOfPayment.Date)
              .AsQueryable();
        }

        private IQueryable<Record> _getBudgetRecords(
            DateTime from,
            DateTime to,
            Expression<Func<Record, bool>> expression = null)
        {
            Guid currentUserID = UserInfo.Current.ID;
            List<Guid> allCollectiveUserIDs = collectionUserService.GetAllCollectiveUserIDs();
            var predicate = PredicateBuilder.True<Record>();

            predicate = predicate.And(x => allCollectiveUserIDs.Contains(x.UserID)
                  && from <= x.DateTimeOfPayment && to >= x.DateTimeOfPayment
                  && x.IsDeleted == false
                  && (x.UserID != currentUserID ? x.IsShowForCollection : true));

            if (expression != null)
            {
                predicate = predicate.And(expression);
            }
            return repository.GetAll(predicate);
        }


        public async Task<IList<BudgetRecordModelView>> GetBudgetRecordsByFilterAsync(CalendarFilterModels filter)
        {
            var expression = await getExpressionByCalendarFilterAsync(filter);

            return await repository
              .GetAll(expression)
              .Select(x => new BudgetRecordModelView
              {
                  ID = x.ID,
                  AccountID = x.AccountID,
                  DateTimeCreate = x.DateTimeCreate,
                  DateTimeEdit = x.DateTimeEdit,
                  Description = x.Description,
                  IsConsider = x.IsHide,
                  RawData = x.RawData,
                  Money = x.Total,
                  Cashback = x.Cashback,
                  CurrencyID = x.CurrencyID,
                  CurrencyNominal = x.CurrencyNominal,
                  CurrencyRate = x.CurrencyRate,
                  CurrencySpecificCulture = x.Currency.SpecificCulture,
                  CurrencyCodeName = x.Currency.CodeName,
                  DateTimeOfPayment = x.DateTimeOfPayment,
                  SectionID = x.BudgetSectionID,
                  SectionName = x.BudgetSection.Name,
                  SectionTypeID = x.BudgetSection.SectionTypeID,
                  AreaID = x.BudgetSection.BudgetArea.ID,
                  AreaName = x.BudgetSection.BudgetArea.Name,
                  CssIcon = x.BudgetSection.CssIcon,
                  CssBackground = x.BudgetSection.CssBackground,
                  CssColor = x.BudgetSection.CssColor,
                  IsShowForCollection = x.IsShowForCollection,
                  IsOwner = x.UserID == filter.UserID,
                  UserName = x.User.Name + " " + x.User.LastName,
                  ImageLink = x.User.ImageLink,
                  Section = new BudgetSectionModelView
                  {
                      SectionTypeID = x.BudgetSection.SectionTypeID,
                      AreaID = x.BudgetSection.BudgetAreaID,
                      AreaName = x.BudgetSection.BudgetArea.Name,
                      Name = x.BudgetSection.Name,
                      ID = x.BudgetSectionID,
                      CssIcon = x.BudgetSection.CssIcon,
                      CssBackground = x.BudgetSection.CssBackground,
                      CssColor = x.BudgetSection.CssColor,
                  },
                  Account = x.AccountID == null ? null : new AccountModelView
                  {
                      AccountType = x.Account.AccountTypeID,
                      BankImage = x.Account.Bank != null ? x.Account.Bank.LogoCircle : null,
                      Name = x.Account.Name,
                      AccountIcon = x.Account.AccountType.Icon,
                      CurrencyIcon = x.Account.Currency.Icon,
                      CardID = x.Account.CardID,
                      CardName = x.Account.CardID != null ? x.Account.Card.Name : null,
                      CardLogo = x.Account.CardID != null ? x.Account.Card.SmallLogo : null,

                  },
                  Tags = x.Tags
                      .Select(y => new RecordTag
                      {
                          ID = y.UserTagID,
                          Title = y.UserTag.Title,
                          IsDeleted = y.UserTag.IsDeleted,
                          DateCreate = y.UserTag.DateCreate,

                          CompanyID = y.UserTag.CompanyID,
                          CompanyName = y.UserTag.Company != null ? y.UserTag.Company.Name : null,
                          CompanyLogo = y.UserTag.Company != null ? y.UserTag.Company.LogoSquare : null,

                      }),
                  //x.RecordHistories
                  //  .OrderBy(z => z.DateCreate)
                  //  .Select(z => new
                  //  {
                  //      z.ActionTypeCode,
                  //      z.AccountNewBalance,
                  //      z.old
                  //  })
              })
              .OrderByDescending(x => x.DateTimeOfPayment.Date)
              .ToListAsync();
        }

        public async Task<IList<HistoryRecordModelView>> GetBudgetRecordsGroupByDateByFilterAsync(CalendarFilterModels filter)
        {
            var expression = await getExpressionByCalendarFilterAsync(filter);

            return repository
              .GetAll(expression)
              .Select(x => new BudgetRecordModelView
              {
                  ID = x.ID,
                  AccountID = x.AccountID,
                  DateTimeCreate = x.DateTimeCreate,
                  DateTimeEdit = x.DateTimeEdit,
                  Description = x.Description,
                  IsConsider = x.IsHide,
                  RawData = x.RawData,
                  Money = x.Total,
                  Cashback = x.Cashback,
                  CurrencyID = x.CurrencyID,
                  CurrencyNominal = x.CurrencyNominal,
                  CurrencyRate = x.CurrencyRate,
                  CurrencySpecificCulture = x.Currency.SpecificCulture,
                  CurrencyCodeName = x.Currency.CodeName,
                  DateTimeOfPayment = x.DateTimeOfPayment,
                  SectionID = x.BudgetSectionID,
                  SectionName = x.BudgetSection.Name,
                  SectionTypeID = x.BudgetSection.SectionTypeID,
                  AreaID = x.BudgetSection.BudgetArea.ID,
                  AreaName = x.BudgetSection.BudgetArea.Name,
                  CssIcon = x.BudgetSection.CssIcon,
                  CssBackground = x.BudgetSection.CssBackground,
                  CssColor = x.BudgetSection.CssColor,
                  IsShowForCollection = x.IsShowForCollection,
                  IsOwner = x.UserID == filter.UserID,
                  UserName = x.User.Name + " " + x.User.LastName,
                  ImageLink = x.User.ImageLink,
                  Currency = new CurrencyLightModelView
                  {
                      ID = x.CurrencyID,
                      Nominal = x.CurrencyNominal,
                      Rate = x.CurrencyRate,
                      SpecificCulture = x.Currency.SpecificCulture,
                      CodeName = x.Currency.CodeName,
                  },
                  Section = new BudgetSectionModelView
                  {
                      SectionTypeID = x.BudgetSection.SectionTypeID,
                      AreaID = x.BudgetSection.BudgetAreaID,
                      AreaName = x.BudgetSection.BudgetArea.Name,
                      Name = x.BudgetSection.Name,
                      ID = x.BudgetSectionID,
                      CssIcon = x.BudgetSection.CssIcon,
                      CssBackground = x.BudgetSection.CssBackground,
                      CssColor = x.BudgetSection.CssColor,
                  },
                  Account = x.AccountID == null ? null : new AccountModelView
                  {
                      AccountType = x.Account.AccountTypeID,
                      BankImage = x.Account.Bank != null ? x.Account.Bank.LogoCircle : null,
                      Name = x.Account.Name,
                      AccountIcon = x.Account.AccountType.Icon,
                      CurrencyIcon = x.Account.Currency.Icon,
                      CardID = x.Account.CardID,
                      CardName = x.Account.CardID != null ? x.Account.Card.Name : null,
                      CardLogo = x.Account.CardID != null ? x.Account.Card.SmallLogo : null,
                      ID = x.AccountID,

                      NewBalance = x.RecordHistories != null ? x.RecordHistories
                            .OrderBy(z => z.DateCreate)
                            .FirstOrDefault().AccountNewBalance : (decimal?)null,
                      OldBalance = x.RecordHistories != null ? x.RecordHistories
                            .OrderBy(z => z.DateCreate)
                            .FirstOrDefault().AccountOldBalance : (decimal?)null
                  },
                  Tags = x.Tags
                      .Select(y => new RecordTag
                      {
                          ID = y.UserTagID,
                          Title = y.UserTag.Title,
                          //IsDeleted = y.UserTag.IsDeleted,
                          //DateCreate = y.UserTag.DateCreate,

                          CompanyID = y.UserTag.CompanyID,
                          CompanyName = y.UserTag.Company != null ? y.UserTag.Company.Name : null,
                          CompanyLogo = y.UserTag.Company != null ? y.UserTag.Company.LogoSquare : null,

                      }),
              })
              .OrderByDescending(x => x.DateTimeOfPayment.Date)
              .ToList()
              .GroupBy(x => x.DateTimeOfPayment.Date)
              .Select(y => new HistoryRecordModelView
              {
                  GroupDate = y.Key,
                  Records = y
                    .OrderBy(x => x.DateTimeOfPayment.Date)
                    .Select(x => x)
                  //.ToList()
              })
              .OrderByDescending(x => x.GroupDate)
              .ToList();
        }


        public IList<BudgetRecordModelView> GetBudgetRecordsByFilter(CalendarFilterModels filter)
        {
            var currentUserID = UserInfo.Current.ID;
            var expression = getExpressionByCalendarFilter(filter);

            return repository
              .GetAll(expression)
              .Select(x => new BudgetRecordModelView
              {
                  ID = x.ID,
                  AccountID = x.AccountID,
                  DateTimeCreate = x.DateTimeCreate,
                  DateTimeEdit = x.DateTimeEdit,
                  Description = x.Description,
                  IsConsider = x.IsHide,
                  RawData = x.RawData,
                  Money = x.Total,
                  Cashback = x.Cashback,
                  CurrencyID = x.CurrencyID,
                  CurrencyNominal = x.CurrencyNominal,
                  CurrencyRate = x.CurrencyRate,
                  CurrencySpecificCulture = x.Currency.SpecificCulture,
                  CurrencyCodeName = x.Currency.CodeName,
                  DateTimeOfPayment = x.DateTimeOfPayment,
                  SectionID = x.BudgetSectionID,
                  SectionName = x.BudgetSection.Name,
                  SectionTypeID = x.BudgetSection.SectionTypeID,
                  AreaID = x.BudgetSection.BudgetArea.ID,
                  AreaName = x.BudgetSection.BudgetArea.Name,
                  CssIcon = x.BudgetSection.CssIcon,
                  CssBackground = x.BudgetSection.CssBackground,
                  CssColor = x.BudgetSection.CssColor,
                  IsShowForCollection = x.IsShowForCollection,
                  IsOwner = x.UserID == currentUserID,
                  UserName = x.User.Name + " " + x.User.LastName,
                  ImageLink = x.User.ImageLink,
                  Account = x.AccountID == null ? null : new AccountModelView
                  {
                      AccountType = x.Account.AccountTypeID,
                      BankImage = x.Account.Bank != null ? x.Account.Bank.LogoCircle : null,
                      Name = x.Account.Name,
                      AccountIcon = x.Account.AccountType.Icon,
                      CurrencyIcon = x.Account.Currency.Icon,
                      CardID = x.Account.CardID,
                      CardName = x.Account.CardID != null ? x.Account.Card.Name : null,
                      CardLogo = x.Account.CardID != null ? x.Account.Card.SmallLogo : null,
                  },
                  Tags = x.Tags
                  .Select(y => new RecordTag
                  {
                      ID = y.UserTagID,
                      Title = y.UserTag.Title,
                      IsDeleted = y.UserTag.IsDeleted,
                      DateCreate = y.UserTag.DateCreate,
                  })
              })
              .OrderByDescending(x => x.DateTimeOfPayment.Date)
              .ToList();
        }

        public async Task<IList<BudgetRecordModelView>> GetLast(int count)
        {
            var currentUserID = UserInfo.Current.ID;

            return await repository
              .GetAll<Record>(x => x.UserID == currentUserID)
              .Select(x => new BudgetRecordModelView
              {
                  ID = x.ID,
                  AccountID = x.AccountID,
                  DateTimeCreate = x.DateTimeCreate,
                  DateTimeEdit = x.DateTimeEdit,
                  Description = x.Description,
                  IsConsider = x.IsHide,
                  RawData = x.RawData,
                  Money = x.Total,
                  Cashback = x.Cashback,

                  CurrencyID = x.CurrencyID,
                  CurrencyNominal = x.CurrencyNominal,
                  CurrencyRate = x.CurrencyRate,
                  CurrencySpecificCulture = x.Currency.SpecificCulture,
                  CurrencyCodeName = x.Currency.CodeName,

                  DateTimeOfPayment = x.DateTimeOfPayment,
                  SectionID = x.BudgetSectionID,
                  SectionName = x.BudgetSection.Name,
                  AreaID = x.BudgetSection.BudgetArea.ID,
                  AreaName = x.BudgetSection.BudgetArea.Name,
                  IsShowForCollection = x.IsShowForCollection,
                  IsOwner = x.UserID == currentUserID,
                  UserName = x.User.Name + " " + x.User.LastName,
                  ImageLink = x.User.ImageLink,
                  Account = x.AccountID == null ? null : new AccountModelView
                  {
                      AccountType = x.Account.AccountTypeID,
                      BankImage = x.Account.Bank != null ? x.Account.Bank.LogoCircle : null,
                      Name = x.Account.Name,
                      AccountIcon = x.Account.AccountType.Icon,
                      CardID = x.Account.CardID,
                      CardName = x.Account.CardID != null ? x.Account.Card.Name : null,
                      CardLogo = x.Account.CardID != null ? x.Account.Card.SmallLogo : null,
                  },
                  Tags = x.Tags
                  .Select(y => new RecordTag
                  {
                      ID = y.UserTagID,
                      Title = y.UserTag.Title,
                      IsDeleted = y.UserTag.IsDeleted,
                      DateCreate = y.UserTag.DateCreate,
                  })
              })
              .OrderByDescending(x => x.ID)
              .Take(count)
              //.OrderBy(x => x.ID)
              .ToListAsync();
        }

        public async Task<decimal> GetTotalSpendsForLimitByFilter(CalendarFilterModels filter)
        {
            var expression = await getExpressionByCalendarFilterAsync(filter);

            return await repository
              .GetAll(expression)
              .SumAsync(x => x.Total);
        }

        public decimal GetTotalForSummaryByFilter(SummaryFilter filter)
        {
            var expression = PredicateBuilder.True<Record>();

            expression = expression.And(x => x.UserID == filter.UserID
                    && filter.StartDate <= x.DateTimeOfPayment && filter.EndDate >= x.DateTimeOfPayment
                    && (x.UserID != filter.UserID ? x.IsShowForCollection : true)
                    && x.IsDeleted == false);

            if (filter.SectionTypes != null)// by sections
            {
                expression = expression.And(x => filter.SectionTypes.Contains(x.BudgetSection.SectionTypeID ?? 0));
            }
            else if (filter.Sections != null)
            {
                expression = expression.And(x => filter.Sections.Contains(x.BudgetSectionID));
            }

            return repository
              .GetAll(expression)
              .Sum(x => x.Total);
        }


        public async Task<List<int>> GetAllYears()
        {
            List<int> years = await repository.GetAll<Record>(x => x.UserID == UserInfo.Current.ID)
                .GroupBy(x => x.DateTimeOfPayment.Year)
                //.OrderBy(x => x.Key)
                .Select(x => x.Key)
                .ToListAsync();

            if (!years.Any(x => x == DateTime.Now.Year))
            {
                years.Add(DateTime.Now.Year);
            }

            return years
                .OrderBy(x => x)
                .ToList();
        }

        private async Task<Expression<Func<Record, bool>>> getExpressionByCalendarFilterAsync(CalendarFilterModels filter)
        {
            var expression = PredicateBuilder.True<Record>();

            if (filter.IsConsiderCollection)
            {
                List<Guid> allCollectiveUserIDs = await collectionUserService.GetAllCollectiveUserIDsAsync();
                expression = expression.And(x => allCollectiveUserIDs.Contains(x.UserID));

                var userIDs_withoutCurrent = allCollectiveUserIDs.Where(x => x != filter.UserID).ToList();

                filter.Sections.AddRange(await repository.GetAll<BudgetSection>(
                    x => userIDs_withoutCurrent.Contains(x.BudgetArea.UserID ?? Guid.Parse("086d7c26-1d8d-4cc7-e776-08d7eab4d0ed"))
                    && x.IsShowInCollective).Select(x => x.ID).ToListAsync());
            }
            else
            {
                expression = expression.And(x => x.UserID == filter.UserID);
            }

            if (filter.IsSection)// by sections
            {
                expression = expression.And(x => filter.Sections.Contains(x.BudgetSectionID));
            }
            else // by tags
            {
                expression = expression.And(x => x.Tags.Any(y => filter.Tags.Contains(y.UserTagID)));
            }

            expression = expression.And(x => filter.StartDate <= x.DateTimeOfPayment && filter.EndDate >= x.DateTimeOfPayment
                  && (x.UserID != filter.UserID ? x.IsShowForCollection : true)
                  && x.IsDeleted == false);
            return expression;
        }

        private Expression<Func<Record, bool>> getExpressionByCalendarFilter(CalendarFilterModels filter)
        {
            Guid currentUserID = UserInfo.Current.ID;
            var expression = PredicateBuilder.True<Record>();

            if (filter.IsConsiderCollection)
            {
                List<Guid> allCollectiveUserIDs = collectionUserService.GetAllCollectiveUserIDs();
                expression = expression.And(x => allCollectiveUserIDs.Contains(x.UserID));

                var userIDs_withoutCurrent = allCollectiveUserIDs.Where(x => x != currentUserID).ToList();

                filter.Sections.AddRange(repository.GetAll<BudgetSection>(
                    x => userIDs_withoutCurrent.Contains(x.BudgetArea.UserID ?? Guid.Parse("086d7c26-1d8d-4cc7-e776-08d7eab4d0ed"))
                    && x.IsShowInCollective).Select(x => x.ID).ToList());
            }
            else
            {
                expression = expression.And(x => x.UserID == currentUserID);
            }

            if (filter.IsSection)// by sections
            {
                expression = expression.And(x => filter.Sections.Contains(x.BudgetSectionID));
            }
            else // by tags
            {
                expression = expression.And(x => x.Tags.Any(y => filter.Tags.Contains(y.UserTagID)));
            }

            expression = expression.And(x => filter.StartDate <= x.DateTimeOfPayment && filter.EndDate >= x.DateTimeOfPayment
                  && (x.UserID != currentUserID ? x.IsShowForCollection : true)
                  && x.IsDeleted == false);
            return expression;
        }
    }

}
