using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.AreaAndSection;
using MyProfile.Entity.ModelView.BudgetView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Tag.Service;
using MyProfile.User.Service;
using MyProfile.UserLog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
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
        private IMemoryCache cache;

        public BudgetRecordService(IBaseRepository repository,
            TagService tagService,
            AccountService accountService,
            IMemoryCache cache,
            SectionService sectionService)
        {
            this.repository = repository;
            this.collectionUserService = new CollectionUserService(repository);
            this.userLogService = new UserLogService(repository);
            this.tagService = tagService;
            this.accountService = accountService;
            this.sectionService = sectionService;
            this.cache = cache;
        }

        public async Task<RecordModelView> GetByID(int id)
        {
            return await repository.GetAll<BudgetRecord>(x => x.ID == id && x.UserID == UserInfo.Current.ID)
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
            List<int> errorLogCreateIDs = new List<int>();
            List<int> errorLogEditIDs = new List<int>();
            budgetRecord.DateTimeOfPayment = new DateTime(budgetRecord.DateTimeOfPayment.Year, budgetRecord.DateTimeOfPayment.Month, budgetRecord.DateTimeOfPayment.Day, 13, 0, 0);
            decimal cashback = 0;
            List<Account> accounts = new List<Account>();
            List<SectionLightModelView> sections = (await sectionService.GetAllSectionByUser()).ToList();
            bool isSpending = false;

            foreach (var record in budgetRecord.Records.Where(x => x.IsCorrect))
            {
                if (currentUser.IsAvailable == false)
                {
                    record.IsSaved = false;
                    continue;
                }
                cashback = 0;

                Account account = accounts.FirstOrDefault(x => x.ID == record.AccountID);
                SectionLightModelView section = sections.FirstOrDefault(x => x.ID == record.SectionID);
                isSpending = section.SectionTypeID == (int?)SectionTypeEnum.Spendings;

                if (account == null)
                {
                    account = await repository.GetAll<Account>(x => x.ID == record.AccountID)
                       .FirstOrDefaultAsync();

                    accounts.Add(account);
                }
                if (isSpending
                    && record.Money >= 100
                    && account.IsCachback
                    && account.CachbackForAllPercent != null)
                {
                    cashback = (record.Money * account.CachbackForAllPercent ?? 1) / 100;
                }
                if (record.Tags.Count() > 0)
                {
                    record.Description = await tagService.ParseAndCreateDescription(record.Description, record.Tags, newUserTags);
                }

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
                            var newRecord = new BudgetRecord
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
                                Cashback = cashback,
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
                            if (isSpending)
                            {
                                account.Balance -= record.Money;

                                if (account.IsCachback && account.CachbackForAllPercent != null)
                                {
                                    account.CachbackBalance += cashback;
                                }
                            }
                            else
                            {
                                account.Balance += record.Money;
                            }
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
                            var dbRecord = repository.GetByID<BudgetRecord>(record.ID);
                            int oldAccountID = dbRecord.AccountID ?? -1,
                                oldSectionTypeID = dbRecord.BudgetSection.SectionTypeID ?? 0;
                            decimal oldTotal = dbRecord.Total,
                                oldCashback = dbRecord.Cashback;
                            bool isOldAccountHasChashback = dbRecord.Account.IsCachback && dbRecord.Account.CachbackForAllPercent != null;

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
                            dbRecord.Cashback = cashback;

                            foreach (var newTag in tagService.CheckTags(dbRecord.Tags, record.Tags.ToList()))
                            {
                                dbRecord.Tags.Add(new Entity.Model.RecordTag { DateSet = now, UserTagID = newTag.ID });
                            }

                            await repository.UpdateAsync(dbRecord, true);
                            record.IsSaved = true;

                            #region Account
                            if (oldAccountID == record.AccountID)
                            {
                                if (oldSectionTypeID == (int)SectionTypeEnum.Spendings)
                                {
                                    account.Balance += oldTotal;

                                    if (isOldAccountHasChashback)
                                    {
                                        account.CachbackBalance -= oldCashback;
                                    }
                                }
                                else
                                {
                                    account.Balance -= oldTotal;
                                }

                                if (isSpending)//new section type
                                {
                                    account.Balance -= record.Money;

                                    if (account.IsCachback && account.CachbackForAllPercent != null)
                                    {
                                        account.CachbackBalance += cashback;
                                    }
                                }
                                else
                                {
                                    account.Balance += record.Money;
                                }
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

                                if (oldSectionTypeID == (int)SectionTypeEnum.Spendings)
                                {
                                    oldAccount.Balance += oldTotal;
                                    if (oldAccount.IsCachback && oldAccount.CachbackForAllPercent != null)
                                    {
                                        oldAccount.CachbackBalance -= oldCashback;
                                    }
                                }
                                else
                                {
                                    oldAccount.Balance -= oldTotal;
                                }

                                if (isSpending)// new section type
                                {
                                    account.Balance -= record.Money;

                                    if (account.IsCachback && account.CachbackForAllPercent != null)
                                    {
                                        account.CachbackBalance += cashback;
                                    }
                                }
                                else
                                {
                                    account.Balance += record.Money;
                                }
                            }
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

            return true;
        }

        public async Task<bool> RemoveRecord(BudgetRecordModelView record)
        {
            var currentUser = UserInfo.Current;
            var db_record = await repository.GetAll<BudgetRecord>(x => x.ID == record.ID && x.UserID == currentUser.ID)
                .FirstOrDefaultAsync();

            if (db_record != null)
            {
                db_record.IsDeleted = true;
                db_record.DateTimeDelete = DateTime.Now.ToUniversalTime();

                if (db_record.BudgetSection.SectionTypeID == (int)SectionTypeEnum.Spendings)
                {
                    db_record.Account.Balance += db_record.Total;
                    if (db_record.Account.IsCachback && db_record.Account.CachbackForAllPercent != null)
                    {
                        db_record.Account.CachbackBalance -= db_record.Cashback;
                    }
                }
                else
                {
                    db_record.Account.Balance -= db_record.Total;
                }

                await repository.UpdateAsync(db_record, true);
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Record_Delete);
                return true;
            }
            return false;
        }

        public async Task<bool> RecoveryRecord(BudgetRecordModelView record)
        {
            var currentUser = UserInfo.Current;
            var db_record = await repository.GetAll<BudgetRecord>(x => x.ID == record.ID && x.UserID == currentUser.ID).FirstOrDefaultAsync();

            if (db_record != null)
            {
                db_record.IsDeleted = false;
                db_record.DateTimeDelete = null;

                if (db_record.BudgetSection.SectionTypeID == (int)SectionTypeEnum.Spendings)
                {
                    db_record.Account.Balance -= db_record.Total;
                    if (db_record.Account.IsCachback && db_record.Account.CachbackForAllPercent != null)
                    {
                        db_record.Account.CachbackBalance += db_record.Cashback;
                    }
                }
                else
                {
                    db_record.Account.Balance += db_record.Total;
                }

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
            Expression<Func<BudgetRecord, bool>> expression = null)
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

        private IQueryable<BudgetRecord> _getBudgetRecords(
            DateTime from,
            DateTime to,
            Expression<Func<BudgetRecord, bool>> expression = null)
        {
            Guid currentUserID = UserInfo.Current.ID;
            List<Guid> allCollectiveUserIDs = collectionUserService.GetAllCollectiveUserIDs();
            var predicate = PredicateBuilder.True<BudgetRecord>();

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
            var currentUserID = UserInfo.Current.ID;
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
                      BankImage = x.Account.Bank != null ? x.Account.Bank.ImageSrc : null,
                      Name = x.Account.Name,
                      AccountIcon = x.Account.AccountType.Icon
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
              .ToListAsync();
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
                      BankImage = x.Account.Bank != null ? x.Account.Bank.ImageSrc : null,
                      Name = x.Account.Name,
                      AccountIcon = x.Account.AccountType.Icon
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
              .GetAll<BudgetRecord>(x => x.UserID == currentUserID)
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
                      BankImage = x.Account.Bank != null ? x.Account.Bank.ImageSrc : null,
                      Name = x.Account.Name,
                      AccountIcon = x.Account.AccountType.Icon
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

        public async Task<List<int>> GetAllYears()
        {
            List<int> years = await repository.GetAll<BudgetRecord>(x => x.UserID == UserInfo.Current.ID)
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

        private async Task<Expression<Func<BudgetRecord, bool>>> getExpressionByCalendarFilterAsync(CalendarFilterModels filter)
        {
            Guid currentUserID = UserInfo.Current.ID;
            var expression = PredicateBuilder.True<BudgetRecord>();

            if (filter.IsConsiderCollection)
            {
                List<Guid> allCollectiveUserIDs = await collectionUserService.GetAllCollectiveUserIDsAsync();
                expression = expression.And(x => allCollectiveUserIDs.Contains(x.UserID));

                var userIDs_withoutCurrent = allCollectiveUserIDs.Where(x => x != currentUserID).ToList();

                filter.Sections.AddRange(await repository.GetAll<BudgetSection>(
                    x => userIDs_withoutCurrent.Contains(x.BudgetArea.UserID ?? Guid.Parse("086d7c26-1d8d-4cc7-e776-08d7eab4d0ed"))
                    && x.IsShowInCollective).Select(x => x.ID).ToListAsync());
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

        private Expression<Func<BudgetRecord, bool>> getExpressionByCalendarFilter(CalendarFilterModels filter)
        {
            Guid currentUserID = UserInfo.Current.ID;
            var expression = PredicateBuilder.True<BudgetRecord>();

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
