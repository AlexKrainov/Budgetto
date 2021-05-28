using Common.Service;
using Microsoft.Extensions.Caching.Memory;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Account;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Progress.Service;
using MyProfile.UserLog.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProfile.Budget.Service
{
    public partial class AccountService
    {
        private IBaseRepository repository;
        private IMemoryCache cache;
        private UserLogService userLogService;
        private CurrencyService сurrencyService;
        private ProgressService progressService;

        public AccountService(IBaseRepository repository,
            IMemoryCache cache,
            UserLogService userLogService,
            CurrencyService сurrencyService,
            ProgressService progressService)
        {
            this.repository = repository;
            this.cache = cache;
            this.userLogService = userLogService;
            this.сurrencyService = сurrencyService;
            this.progressService = progressService;
        }

        public List<MainAccountModelView> GetMainAccounts(Guid currentUserID)
        {
            var now = DateTime.Now.ToUniversalTime();
            var currentUser = UserInfo.Current;

            var mainAccounts = repository.GetAll<Account>(x => x.UserID == currentUserID && x.IsDeleted == false && x.ParentAccountID == null)
            .Select(y => new MainAccountModelView
            {
                ID = y.ID,
                Name = y.Name,
                BankID = y.BankID,
                BankLogo = y.Bank != null ? y.Bank.LogoRectangle : null,
                BankName = y.Bank != null ? y.Bank.Name : null,
                BankTypeID = y.Bank != null ? y.Bank.BankTypeID : null,
                Description = y.Description,
                AccountType = (AccountTypes)y.AccountTypeID,
                AccountIcon = y.AccountType.Icon,
                CurrencyID = y.CurrencyID,
                Currency = new Entity.ModelView.Currency.CurrencyClientModelView
                {
                    id = y.CurrencyID ?? 0,
                    codeName = y.Currency.CodeName,
                    specificCulture = y.Currency.SpecificCulture,
                    icon = y.Currency.Icon,
                },

                IsShow = true,
                IsHideCurrentAccount = y.IsHide,

                Accounts = y.ChildAccounts
                    .Where(x => x.IsDeleted == false)
                    .Select(x => new AccountViewModel
                    {
                        ID = x.ID,
                        ParentID = x.ParentAccountID,
                        Name = x.Name,
                        Balance = x.Balance,
                        Description = x.Description,
                        IsDefault = x.IsDefault,
                        IsHideCurrentAccount = x.IsHide,
                        AccountType = (AccountTypes)x.AccountTypeID,
                        AccountTypeName = x.AccountType.Name,
                        AccountIcon = x.AccountType.Icon,
                        PaymentSystemID = x.PaymentSystemID,
                        PaymentLogo = x.PaymentSystemID != null ? x.PaymentSystem.Logo : null,
                        CurrencyID = x.CurrencyID ?? 0,
                        CurrencyIcon = x.Currency.Icon,
                        Currency = new Entity.ModelView.Currency.CurrencyClientModelView
                        {
                            id = x.CurrencyID ?? 0,
                            codeName = x.Currency.CodeName,
                            specificCulture = x.Currency.SpecificCulture,
                            icon = x.Currency.Icon,
                        },

                        BankID = x.BankID,
                        BankName = x.Bank != null ? x.Bank.Name : "",
                        BankLogo = x.Bank != null ? x.Bank.LogoRectangle : "",
                        BankTypeID = x.Bank != null ? x.Bank.BankTypeID : null,
                        DateStart = x.DateStart,
                        ExpirationDate = x.ExpirationDate,
                        IsOverdraft = x.IsOverdraft,

                        //deposit
                        InterestRate = x.AccountInfo != null ? x.AccountInfo.InterestRate : null,
                        InterestBalance = x.AccountInfo != null ? x.AccountInfo.InterestBalance : null,
                        InterestBalanceForEnd = x.AccountInfo != null ? x.AccountInfo.InterestBalanceForEndOfDeposit : null,
                        IsFinishedDeposit = x.AccountInfo != null ? x.AccountInfo.IsFinishedDeposit : false,
                        TimeListID = x.AccountInfo != null ? x.AccountInfo.CapitalizationTimeListID : (int)TimeList.Monthly,
                        IsCapitalization = x.AccountInfo != null ? x.AccountInfo.IsCapitalization : false,

                        //credit card
                        CreditLimit = x.AccountInfo != null ? x.AccountInfo.CreditLimit : null,
                        CreditExpirationDate = x.AccountInfo != null ? x.AccountInfo.CreditExpirationDate : null,

                        CardID = x.CardID,
                        CardName = x.CardID != null ? x.Card.Name : null,
                        CardLogo = x.CardID != null ? x.Card.SmallLogo : null,

                        IsCachback = x.IsCachback,
                        IsCachBackMoney = x.IsCachbackMoney,
                        CachBackBalance = x.CachbackBalance,
                        CachBackForAllPercent = x.CachbackForAllPercent,
                        ResetCashBackDate = x.ResetCachbackDate,
                        IsCountTheBalance = x.IsCountTheBalance,
                        IsCountBalanceInMainAccount = x.IsCountBalanceInMainAccount,

                        IsShow = true,
                        IsCash = x.AccountTypeID == (int)AccountTypes.Cash,
                    })
                    .OrderByDescending(x => x.IsDefault)
                    .ThenBy(x => x.IsHideCurrentAccount)
                    .ThenBy(x => x.ID)
                    .ThenBy(x => x.AccountType == AccountTypes.Cash)
                    .ThenBy(x => x.AccountType == AccountTypes.Installment)
                    .ThenBy(x => x.AccountType == AccountTypes.Credit)
                    .ThenBy(x => x.AccountType == AccountTypes.Debed)
                    .ToList()
            })
            .OrderBy(x => x.ID)
            .ToList();

            var currencyRates = сurrencyService.GetRatesByDate(now, currentUser.UserSessionID);
            bool hasCurrencyRates = currencyRates.Count() > 0;
            decimal accountBalance = 0;

            foreach (var mainAccount in mainAccounts)
            {
                foreach (var account in mainAccount.Accounts)
                {
                    accountBalance = account.Balance;

                    if ((account.AccountType == AccountTypes.InvestmentsIIS || account.AccountType == AccountTypes.Deposit)
                        && account.DateStart.HasValue
                        && account.ExpirationDate.HasValue)
                    {
                        var allDays = (account.ExpirationDate.Value - account.DateStart.Value).TotalDays;
                        var leftDays = (account.ExpirationDate.Value - now).TotalDays;
                        account.Percent = 100 - Math.Round(leftDays / allDays * 100, 2);
                    }

                    if (account.AccountType == AccountTypes.Credit
                       && account.CreditExpirationDate.HasValue)
                    {
                        var allDays = (account.CreditExpirationDate.Value - account.CreditExpirationDate.Value.AddMonths(-1)).TotalDays;
                        var leftDays = (account.CreditExpirationDate.Value - now).TotalDays;
                        account.Percent = 100 - Math.Round(leftDays / allDays * 100, 2);

                        account.CreditNeedMoney = account.CreditLimit - account.Balance;

                        accountBalance = account.CreditNeedMoney >= 0 ? 0 : (account.CreditNeedMoney * (-1)) ?? 0;
                    }

                    if (account.AccountType == AccountTypes.Deposit)
                    {
                        if (account.IsFinishedDeposit)
                        {
                            account.InterestBalanceForEnd = account.Balance + account.InterestBalance;
                        }
                        else
                        {
                            account.InterestBalanceForEnd = account.Balance + account.InterestBalanceForEnd;
                        }
                    }

                    try
                    {
                        if (account.IsCountBalanceInMainAccount && mainAccount.CurrencyID != null)
                        {
                            if (mainAccount.CurrencyID == account.CurrencyID)
                            {
                                if (account.AccountType != AccountTypes.Credit)
                                {
                                    mainAccount.Balance += accountBalance;
                                }
                                else
                                {
                                    mainAccount.Balance += accountBalance;
                                }
                            }
                            else
                            {
                                if (hasCurrencyRates)
                                {
                                    if (mainAccount.Currency.codeName == "RUB")
                                    {
                                        mainAccount.Balance += accountBalance * currencyRates.FirstOrDefault(x => x.CharCode == account.Currency.codeName).Rate;
                                    }
                                    else if (account.Currency.codeName == "RUB")
                                    {
                                        mainAccount.Balance += accountBalance / currencyRates.FirstOrDefault(x => x.CharCode == mainAccount.Currency.codeName).Rate;
                                    }
                                    else
                                    {
                                        var mainRate = currencyRates.FirstOrDefault(x => x.CharCode == mainAccount.Currency.codeName).Rate;
                                        var accRate = currencyRates.FirstOrDefault(x => x.CharCode == account.Currency.codeName).Rate;

                                        mainAccount.Balance += (accRate / mainRate) * accountBalance;
                                    }
                                }
                                else
                                {
                                    mainAccount.ConvertError = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        mainAccount.ConvertError = true;
                    }
                }

            }

            return mainAccounts;
        }

        public List<AccountViewModel> GetAccountsWithoutParant(Guid currentUserID)
        {
            return repository.GetAll<Account>(x => x.UserID == currentUserID && x.IsDeleted == false && x.ParentAccountID != null)
                  .Select(x => new AccountViewModel
                  {
                      ID = x.ID,
                      ParentID = x.ParentAccountID,
                      Name = x.Name,
                      Balance = x.Balance,
                      Description = x.Description,
                      IsDefault = x.IsDefault,
                      IsHideCurrentAccount = x.IsHide,
                      AccountType = (AccountTypes)x.AccountTypeID,
                      AccountTypeName = x.AccountType.Name,
                      AccountIcon = x.AccountType.Icon,
                      PaymentSystemID = x.PaymentSystemID,
                      PaymentLogo = x.PaymentSystemID != null ? x.PaymentSystem.Logo : null,
                      CurrencyID = x.CurrencyID ?? 0,
                      CurrencyIcon = x.Currency.Icon,
                      Currency = new Entity.ModelView.Currency.CurrencyClientModelView
                      {
                          id = x.CurrencyID ?? 0,
                          codeName = x.Currency.CodeName,
                          specificCulture = x.Currency.SpecificCulture,
                          icon = x.Currency.Icon,
                      },

                      BankID = x.BankID,
                      BankName = x.Bank != null ? x.Bank.Name : "",
                      BankLogo = x.Bank != null ? x.Bank.LogoRectangle : "",
                      BankTypeID = x.Bank != null ? x.Bank.BankTypeID : null,
                      DateStart = x.DateStart,
                      ExpirationDate = x.ExpirationDate,
                      IsOverdraft = x.IsOverdraft,

                      //deposit
                      InterestRate = x.AccountInfo != null ? x.AccountInfo.InterestRate : null,

                      CardID = x.CardID,
                      CardName = x.CardID != null ? x.Card.Name : null,
                      CardLogo = x.CardID != null ? x.Card.SmallLogo : null,

                      IsCachback = x.IsCachback,
                      IsCachBackMoney = x.IsCachbackMoney,
                      CachBackBalance = x.CachbackBalance,
                      CachBackForAllPercent = x.CachbackForAllPercent,
                      ResetCashBackDate = x.ResetCachbackDate,
                      IsCountTheBalance = x.IsCountTheBalance,

                      IsShow = true,
                      IsCash = x.AccountTypeID == (int)AccountTypes.Cash,
                  })
                  .OrderByDescending(x => x.IsDefault)
                  .ThenBy(x => x.IsHideCurrentAccount)
                  .ThenBy(x => x.AccountType == AccountTypes.Cash)
                  .ThenBy(x => x.AccountType == AccountTypes.Installment)
                  .ThenBy(x => x.AccountType == AccountTypes.Credit)
                  .ThenBy(x => x.AccountType == AccountTypes.Debed)
                  .ToList();
        }

        /// <summary>
        /// Get all money by section type for the period
        /// </summary>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        /// <param name="accounts"></param>
        /// <returns></returns>
        public void GetAcountsAllMoneyByPeriod(DateTime start, DateTime finish, List<MainAccountModelView> mainAccounts)
        {
            var currentUser = UserInfo.Current;

            var records = repository.GetAll<Record>(x => x.UserID == currentUser.ID
                 && x.IsDeleted == false
                 && x.AccountID != null
                 && x.DateTimeOfPayment >= start && x.DateTimeOfPayment <= finish
                 && x.RecordHistories.Count > 0)
                      .Select(x => x.RecordHistories
                                          .OrderByDescending(x => x.ID)
                                          .FirstOrDefault())
                      .Select(x => new
                      {
                          x.ID,
                          x.AccountID,
                          x.Section.SectionTypeID,
                          x.DateTimeOfPayment,
                          x.AccountTotal,
                          x.AccountCashback
                      })
                      .ToList();

            var accountHistories = repository.GetAll<AccountHistory>(x => x.Account.UserID == currentUser.ID
                    && x.Account.IsDeleted == false
                    && (x.ActionType == AccountHistoryActionType.MoveMoney
                        || x.ActionType == AccountHistoryActionType.AddedPercent)
                    && x.CurrentDate >= start && x.CurrentDate <= finish)
                .Select(x => new
                {
                    x.AccountID,
                    x.AccountIDFrom,
                    x.Actions,
                    x.ValueFrom,
                    x.ValueTo,
                    x.ActionType,
                })
                .ToList();

            foreach (var mainAccount in mainAccounts)
            {
                foreach (var account in mainAccount.Accounts)
                {
                    account.IsPast = true;

                    //if (account.BankTypeID == (int)BankTypes.Bank)
                    account.BalanceSpendings += records.Where(x => x.SectionTypeID == (int)SectionTypeEnum.Spendings && (x.AccountID ?? 0) == account.ID).Sum(x => x.AccountTotal);
                    account.BalanceEarnings += records.Where(x => x.SectionTypeID == (int)SectionTypeEnum.Earnings && (x.AccountID ?? 0) == account.ID).Sum(x => x.AccountTotal);
                    account.BalancePastCachback += records.Where(x => x.SectionTypeID == (int)SectionTypeEnum.Spendings && (x.AccountID ?? 0) == account.ID).Sum(x => x.AccountCashback);

                    account.Output = accountHistories.Where(x => x.AccountIDFrom == account.ID && x.ActionType == AccountHistoryActionType.MoveMoney).Sum(x => x.ValueFrom);
                    account.Input = accountHistories.Where(x => x.AccountID == account.ID && x.ActionType == AccountHistoryActionType.MoveMoney).Sum(x => x.ValueTo);

                    account.InterestBalanceForPeriod = accountHistories.Where(x => x.AccountID == account.ID && x.ActionType == AccountHistoryActionType.AddedPercent).Sum(x => x.ValueTo);
                }
            }
        }

        public void Transfer(TransferMoney transfer)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();

            var accountFrom = repository.GetAll<Account>(x => x.UserID == currentUser.ID && x.IsDeleted == false && x.ID == transfer.AccountFromID).FirstOrDefault();
            var accountTo = repository.GetAll<Account>(x => x.UserID == currentUser.ID && x.IsDeleted == false && x.ID == transfer.AccountToID).FirstOrDefault();
            //input, output
            //AccountHistory accountHistoryFrom = new AccountHistory
            //{
            //    CurrentDate = now,
            //    ActionType = AccountHistoryActionType.MoveMoney,
            //    AccountID2 = transfer.AccountToID,
            //    OldBalance = accountFrom.Balance,
            //    Comment = transfer.Comment,
            //    ValueFrom = transfer.Value,
            //    ValueTo = transfer.EndValue,
            //    Actions = "output",
            //    CurrencyValue = transfer.CurrencyValue
            //};

            AccountHistory accountHistory = new AccountHistory
            {
                CurrentDate = now,
                ActionType = AccountHistoryActionType.MoveMoney,
                AccountIDFrom = transfer.AccountFromID,
                OldBalance = accountTo.Balance,
                Comment = transfer.Comment,
                ValueFrom = transfer.Value,
                ValueTo = transfer.EndValue,
                StateField = $"{{ fromAccountID: {transfer.AccountFromID}, fromOldBalance: {accountFrom.Balance}, fromNewBalance: {(accountFrom.Balance - transfer.Value)}, toAccountID: {transfer.AccountToID }, toOldBalance: {accountTo.Balance}, toNewBalance: {(accountTo.Balance + transfer.EndValue)} }}",
                CurrencyValue = transfer.CurrencyValue
            };

            if (transfer.Value > 0
                && transfer.EndValue > 0
                && accountFrom != null
                && accountTo != null)
            {
                accountFrom.Balance -= transfer.Value;
                accountTo.Balance += transfer.EndValue;
            }

            #region Recount InterestBalanceForEndOfDeposit
            if (accountFrom.AccountTypeID == (int)AccountTypes.Deposit && accountFrom.AccountInfo.IsFinishedDeposit == false)
            {
                accountFrom.AccountInfo.InterestBalanceForEndOfDeposit =
                    СalculateEndDeposit((accountFrom.AccountInfo.InterestBalance ?? 0) + accountFrom.Balance, accountFrom.AccountInfo.InterestRate ?? 0, accountFrom.AccountInfo.LastInterestAccrualDate.Value.Date, accountFrom.ExpirationDate.Value.Date, (TimeList)accountFrom.AccountInfo.CapitalizationTimeListID);
            }

            if (accountTo.AccountTypeID == (int)AccountTypes.Deposit && accountTo.AccountInfo.IsFinishedDeposit == false)
            {
                accountTo.AccountInfo.InterestBalanceForEndOfDeposit =
                    СalculateEndDeposit((accountTo.AccountInfo.InterestBalance ?? 0) + accountTo.Balance, accountTo.AccountInfo.InterestRate ?? 0, accountTo.AccountInfo.LastInterestAccrualDate.Value.Date, accountTo.ExpirationDate.Value.Date, (TimeList)accountTo.AccountInfo.CapitalizationTimeListID);
            }
            #endregion

            //accountHistoryFrom.NewBalance = accountFrom.Balance;
            accountHistory.NewBalance = accountTo.Balance;

            //accountFrom.AccountHistories.Add(accountHistoryFrom);
            accountTo.AccountHistories.Add(accountHistory);

            userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Account_TransferMoney);

            repository.Save();
        }

        public List<AccountShortViewModel> GetShortAccounts(Guid currentUserID)
        {
            List<AccountShortViewModel> accounts;

            if (cache.TryGetValue(typeof(AccountShortViewModel).Name + "_" + currentUserID, out accounts) == false)
            {
                accounts = repository.GetAll<Account>(x => x.UserID == currentUserID && x.ParentAccountID != null)
                 .Select(x => new AccountShortViewModel
                 {
                     ID = x.ID,
                     Name = x.Name,
                     IsDefault = x.IsDefault,
                     AccountType = (AccountTypes)x.AccountTypeID,
                     AccountIcon = x.AccountType.Icon,
                     CurrencyID = x.CurrencyID,
                     CurrencySpecificCulture = x.Currency.SpecificCulture,
                     CurrencyCodeName = x.Currency.CodeName,
                     CurrencyIcon = x.Currency.Icon,
                     Balance = x.Balance,

                     BankID = x.BankID,
                     BankName = x.Bank != null ? x.Bank.Name : "",
                     BankImage = x.Bank != null ? x.Bank.LogoCircle : "",

                     PaymentSystemID = x.PaymentSystemID,
                     PaymentLogo = x.PaymentSystemID != null ? x.PaymentSystem.Logo : null,

                     IsDeleted = x.IsDeleted,

                     CardID = x.CardID,
                     CardName = x.CardID != null ? x.Card.Name : null,
                     CardLogo = x.CardID != null ? x.Card.SmallLogo : null,
                 })
                 .ToList();

                cache.Set(typeof(AccountShortViewModel).Name + "_" + currentUserID, accounts, DateTime.Now.AddDays(2));
            }
            return accounts;
        }

        public AccountViewModel Save(AccountViewModel account)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();

            if (account.ID <= 0)
            {
                if (account.IsCash)
                {
                    account.AccountType = AccountTypes.Cash;
                }
                else if (account.AccountType == AccountTypes.Undefined)
                {
                    var bankTypes = GetBankTypesAndAcountTypes();
                    account.AccountType = (AccountTypes)bankTypes.FirstOrDefault(x => x.ID == account.BankTypeID).AccountTypes.FirstOrDefault().ID;
                }

                Account accountDB = new Account
                {
                    ParentAccountID = account.ParentID,
                    Name = account.Name,
                    AccountTypeID = (int)account.AccountType,
                    Balance = account.Balance,
                    UserID = currentUser.ID,
                    IsDefault = account.IsDefault,
                    Description = account.Description,
                    PaymentSystemID = account.PaymentSystemID,

                    BankID = account.BankID,
                    DateStart = account.DateStart,
                    ExpirationDate = account.ExpirationDate,
                    IsOverdraft = account.IsOverdraft,

                    CardID = account.CardID,

                    IsCachback = account.IsCachback,
                    IsCachbackMoney = account.IsCachBackMoney,
                    CachbackBalance = account.CachBackBalance,
                    CachbackForAllPercent = account.CachBackForAllPercent,
                    ResetCachbackDate = account.ResetCashBackDate,
                    IsCountTheBalance = account.IsCountTheBalance,
                    IsCountBalanceInMainAccount = account.IsCountBalanceInMainAccount,

                    IsHide = account.IsHideCurrentAccount,

                    DateCreate = now,
                    LastChanges = now,
                    CurrencyID = account.CurrencyID,
                };

                if (account.ParentID != null)
                {
                    accountDB.AccountInfo = new AccountInfo();

                    if (account.AccountType == AccountTypes.Deposit)
                    {
                        accountDB.AccountInfo.IsCapitalization = account.IsCapitalization;
                        accountDB.AccountInfo.InterestRate = account.InterestRate;
                        accountDB.AccountInfo.CapitalizationTimeListID = account.TimeListID;
                        accountDB.AccountInfo.InterestNextDate = accountDB.DateStart;
                        accountDB.AccountInfo.InterestBalance = 0;
                        accountDB.AccountInfo.InterestBalanceForEndOfDeposit = СalculateEndDeposit(account.Balance, account.InterestRate ?? 0, account.DateStart.Value.Date, account.ExpirationDate.Value.Date, (TimeList)account.TimeListID);
                        CalculateDeposit(accountDB, isNew: true);
                    }
                    else if (account.AccountType == AccountTypes.Credit)
                    {
                        accountDB.AccountInfo.CreditExpirationDate = account.CreditExpirationDate;
                        accountDB.AccountInfo.CreditLimit = account.CreditLimit;
                    }
                }

                if (account.IsCachback)
                {
                    if (account.CachBackBalance <= 0)
                    {
                        accountDB.CachbackBalance = 0.0m;
                    }
                }

                try
                {
                    AccountHistory accountHistory = new AccountHistory
                    {
                        NewAccountStateJson = Serialize(accountDB),
                        CurrentDate = now,
                        ActionType = AccountHistoryActionType.Create
                    };
                    accountDB.AccountHistories.Add(accountHistory);
                    repository.Create(accountDB, true);
                    UpdateIsDefaultAccount(accountDB);
                    account.ID = accountDB.ID;
                }
                catch (Exception ex)
                {
                    userLogService.CreateErrorLog(currentUser.UserSessionID, "AccountService.Save", ex, UserLogActionType.Account_Create);
                }

                userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Account_Create);
            }
            else
            {
                var accountDB = repository.GetAll<Account>(x => x.ID == account.ID && x.UserID == currentUser.ID && x.IsDeleted == false)
                    .FirstOrDefault();

                if (accountDB != null)
                {
                    //string newValue, oldValue;
                    string actions = GetEditActions(accountDB, account);//, newValue, oldValue);

                    AccountHistory accountHistory = new AccountHistory
                    {
                        OldAccountStateJson = Serialize(accountDB),
                        CurrentDate = now,
                        ActionType = AccountHistoryActionType.Edit,
                        Actions = actions,
                    };

                    //accountDB.AccountTypeID = (int)account.AccountType;
                    accountDB.Balance = account.Balance;
                    accountDB.Description = account.Description;
                    accountDB.IsHide = account.IsHideCurrentAccount;
                    accountDB.LastChanges = now;
                    accountDB.Name = account.Name;
                    accountDB.IsDefault = account.IsDefault;
                    accountDB.IsCountTheBalance = account.IsCountTheBalance;
                    accountDB.IsCountBalanceInMainAccount = account.IsCountBalanceInMainAccount;
                    accountDB.PaymentSystemID = account.PaymentSystemID;

                    //User can change currency only for MainAccount , it's only for display the balance sum
                    if (accountDB.ParentAccountID == null)
                    {
                        accountDB.CurrencyID = account.CurrencyID;
                    }

                    if (accountDB.CardID == null && account.CardID != null)
                    {
                        accountDB.CardID = account.CardID;
                    }

                    if (accountDB.AccountType.ID != (int)AccountTypes.Cash)
                    {
                        accountDB.BankID = account.BankID;
                        accountDB.IsCachback = account.IsCachback;
                        accountDB.IsCachbackMoney = account.IsCachBackMoney;
                        accountDB.CachbackBalance = account.CachBackBalance;
                        accountDB.CachbackForAllPercent = account.CachBackForAllPercent;
                        accountDB.DateStart = account.DateStart;
                        accountDB.ExpirationDate = account.ExpirationDate;
                        accountDB.IsOverdraft = account.IsOverdraft;
                        accountDB.ResetCachbackDate = account.ResetCashBackDate;

                        if (account.IsCachback)
                        {
                            if (account.CachBackBalance <= 0)
                            {
                                accountDB.CachbackBalance = 0.0m;
                            }
                        }
                    }
                    else
                    {
                        accountDB.BankID = null;
                        accountDB.IsCachback = false;
                        accountDB.IsCachbackMoney = false;
                        accountDB.CachbackForAllPercent = null;
                        accountDB.ExpirationDate = null;
                        accountDB.IsOverdraft = false;
                        accountDB.ResetCachbackDate = null;
                    }

                    if (accountDB.ParentAccountID != null)
                    {
                        if (accountDB.AccountInfo == null)
                        {
                            accountDB.AccountInfo = new AccountInfo();
                        }

                        accountDB.AccountInfo.InterestRate = account.InterestRate;
                        accountDB.AccountInfo.IsCapitalization = account.IsCapitalization;

                        if (account.AccountType == AccountTypes.Credit)
                        {
                            accountDB.AccountInfo.CreditLimit = account.CreditLimit;
                            accountDB.AccountInfo.CreditExpirationDate = account.CreditExpirationDate;
                        }
                    }

                    try
                    {
                        accountHistory.NewAccountStateJson = Serialize(accountDB);
                        accountDB.AccountHistories.Add(accountHistory);

                        repository.Update(accountDB, true);

                        UpdateIsDefaultAccount(accountDB);
                    }
                    catch (Exception ex)
                    {
                        userLogService.CreateErrorLog(currentUser.UserSessionID, "AccountService.Save", ex, UserLogActionType.Account_Update);
                    }

                    userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Account_Update);
                }
            }

            #region Progress

            if (currentUser.IsCompleteIntroductoryProgress == false)
            {
                progressService.SetCompleteProgressItemTypeAsync(currentUser.ID, ProgressTypeEnum.Introductory, ProgressItemTypeEnum.CreateAccount).Wait();
            }

            #endregion

            cache.Remove(typeof(AccountShortViewModel).Name + "_" + currentUser.ID);

            return account;
        }

        /// <summary>
        /// check parameters
        /// </summary>
        /// <param name="accountDB"></param>
        /// <param name="account"></param>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        private string GetEditActions(Account oldAccount, AccountViewModel newAccount)//, string newValue, string oldValue)
        {
            StringBuilder actions = new StringBuilder();

            if (oldAccount.Name != newAccount.Name)
            {
                actions.Append("Name,");
                //newValue = $"Name: '{newAccount.Name}'";
                //oldValue = $"Name: '{oldAccount.Name}'";
            }

            if (oldAccount.Balance != newAccount.Balance)
            {
                actions.Append("Balance,");
            }
            if (oldAccount.CachbackBalance != newAccount.CachBackBalance)
            {
                actions.Append("CachbackBalance,");
            }
            if (oldAccount.CachbackForAllPercent != newAccount.CachBackForAllPercent)
            {
                actions.Append("Balance,");
            }
            if (oldAccount.CurrencyID != newAccount.CurrencyID)
            {
                actions.Append("CurrencyID,");
            }
            if (oldAccount.Description != newAccount.Description)
            {
                actions.Append("Description,");
            }
            if (oldAccount.ExpirationDate != newAccount.ExpirationDate)
            {
                actions.Append("ExpirationDate,");
            }
            if (oldAccount.AccountInfo != null && oldAccount.AccountInfo.InterestRate != newAccount.InterestRate)
            {
                actions.Append("InterestRate,");
            }
            if (oldAccount.IsCachback != newAccount.IsCachback)
            {
                actions.Append("IsCachback,");
            }
            if (oldAccount.IsCachbackMoney != newAccount.IsCachBackMoney)
            {
                actions.Append("IsCachbackMoney,");
            }
            if (oldAccount.IsCountTheBalance != newAccount.IsCountTheBalance)
            {
                actions.Append("IsCountTheBalance,");
            }
            if (oldAccount.IsDefault != newAccount.IsDefault)
            {
                actions.Append("IsDefault,");
            }
            if (oldAccount.IsHide != newAccount.IsHideCurrentAccount)
            {
                actions.Append("IsHide,");
            }
            if (oldAccount.IsOverdraft != newAccount.IsOverdraft)
            {
                actions.Append("IsOverdraft,");
            }
            if (oldAccount.PaymentSystemID != newAccount.PaymentSystemID)
            {
                actions.Append("PaymentSystemID,");
            }
            if (oldAccount.ResetCachbackDate != newAccount.ResetCachbackDate)
            {
                actions.Append("ResetCachbackDate,");
            }
            if (oldAccount.DateStart != newAccount.DateStart)
            {
                actions.Append("DateStart,");
            }

            //newValue = $"{{ {newValue} }}";
            //oldValue = $"{{ {oldValue} }}";
            return actions.ToString();
        }

        public bool ToggleShowHide(int accountID)
        {
            var now = DateTime.Now.ToUniversalTime();
            var currentUser = UserInfo.Current;
            var accountDB = repository.GetAll<Account>(x => x.UserID == currentUser.ID && x.ID == accountID).FirstOrDefault();

            try
            {
                AccountHistory accountHistory = new AccountHistory
                {
                    CurrentDate = now,
                    ActionType = AccountHistoryActionType.Toggle,
                    Actions = !accountDB.IsHide ? "hide" : "show",
                };
                accountDB.IsHide = !accountDB.IsHide;
                accountDB.LastChanges = now;
                accountDB.AccountHistories.Add(accountHistory);

                repository.Update(accountDB, true);
                userLogService.CreateUserLog(currentUser.UserSessionID, accountDB.IsHide ? UserLogActionType.Account_Toggle_Hide : UserLogActionType.Account_Toggle_Show);
            }
            catch (Exception ex)
            {
                userLogService.CreateErrorLog(currentUser.UserSessionID, "AccountService.ShowHide", ex, UserLogActionType.Account_Toggle);
            }

            return accountDB.IsHide;
        }

        public bool RemoveOrRecovery(AccountViewModel account, ref long accountIDWithIsDefault)
        {
            var now = DateTime.Now.ToUniversalTime();
            var currentUser = UserInfo.Current;
            var accountDB = repository.GetAll<Account>(x => x.UserID == currentUser.ID && x.ID == account.ID).FirstOrDefault();

            if (account.IsDeleted && repository.GetAll<Account>(x => x.UserID == currentUser.ID && x.IsDeleted == false).Count() <= 1)
            {
                userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Account_TryToDeleteLastAccount);
                //User cannot remove last account
                return !account.IsDeleted;
            }
            try
            {
                AccountHistory accountHistory = new AccountHistory
                {
                    OldAccountStateJson = Serialize(accountDB),
                    CurrentDate = now,
                    ActionType = account.IsDeleted ? AccountHistoryActionType.Delete : AccountHistoryActionType.Recovery
                };

                accountDB.IsDeleted = account.IsDeleted;
                accountDB.IsDefault = false;
                accountDB.LastChanges = now;

                accountHistory.NewAccountStateJson = Serialize(accountDB);
                accountDB.AccountHistories.Add(accountHistory);

                repository.Update(accountDB, true);

                if (account.IsDefault)
                {
                    accountIDWithIsDefault = UpdateIsDefaultAccount(accountDB);
                }

                cache.Remove(typeof(AccountShortViewModel).Name + "_" + currentUser.ID);
            }
            catch (Exception ex)
            {
                userLogService.CreateErrorLog(currentUser.UserSessionID, "AccountService.RemoveOrRecovery", ex, account.IsDeleted ? UserLogActionType.Account_Delete : UserLogActionType.Account_Recovery);

                return !account.IsDeleted;
            }

            userLogService.CreateUserLog(currentUser.UserSessionID, account.IsDeleted ? UserLogActionType.Account_Delete : UserLogActionType.Account_Recovery);

            return account.IsDeleted;
        }

        public int ResetAllCahbacks()
        {
            var now = DateTime.Now.ToUniversalTime();
            var accounts = repository
                .GetAll<Account>(x => x.IsDeleted == false && x.IsCachback && x.CachbackBalance != 0)
                .ToList();

            for (int i = 0; i < accounts.Count; i++)
            {
                AccountHistory accountHistory = new AccountHistory
                {
                    CurrentDate = now,
                    ActionType = AccountHistoryActionType.ResetCacback,
                    CachbackBalance = accounts[i].CachbackBalance
                };

                accounts[i].CachbackBalance = 0;
                accounts[i].AccountHistories.Add(accountHistory);

            }
            repository.Save();

            return accounts.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <returns>ID of isDefault account</returns>
        private long UpdateIsDefaultAccount(Account account)
        {
            long accountIDWithIsDefault = -1;

            if (account.IsDefault)
            {
                var accounts = repository.GetAll<Account>(x => x.UserID == account.UserID && x.IsDefault && x.ID != account.ID && x.IsDeleted == false)
                    .ToList();

                for (int i = 0; i < accounts.Count(); i++)
                {
                    accounts[i].IsDefault = false;
                    repository.Update(accounts[i]);
                }
                repository.Save();
                accountIDWithIsDefault = account.ID;
            }
            else
            {
                if (!repository.Any<Account>(x => x.UserID == account.UserID && x.IsDeleted == false && x.IsDefault))
                {
                    var accountDB = repository.GetAll<Account>(x =>
                                        x.UserID == account.UserID
                                        && x.IsDeleted == false
                                        && x.AccountTypeID == (int)AccountTypes.Cash)
                                           .FirstOrDefault();

                    if (accountDB != null)
                    {
                        accountDB.IsDefault = true;
                        accountIDWithIsDefault = accountDB.ID;
                    }
                    else
                    {
                        accountDB = repository.GetAll<Account>(x =>
                                        x.UserID == account.UserID
                                        && x.IsDeleted == false)
                                           .FirstOrDefault();

                        accountDB.IsDefault = true;
                        accountIDWithIsDefault = accountDB.ID;
                    }

                    repository.Update(accountDB, true);
                }
            }
            return accountIDWithIsDefault;
        }

        private string Serialize(Account accountDB)
        {
            Account account = new Account
            {
                AccountTypeID = accountDB.AccountTypeID,
                Balance = accountDB.Balance,
                BankID = accountDB.BankID,
                CachbackBalance = accountDB.CachbackBalance,
                CachbackForAllPercent = accountDB.CachbackForAllPercent,
                CurrencyID = accountDB.CurrencyID,
                DateCreate = accountDB.DateCreate,
                Description = accountDB.Description,
                ExpirationDate = accountDB.ExpirationDate,
                ID = accountDB.ID,
                IsCachback = accountDB.IsCachback,
                IsCachbackMoney = accountDB.IsCachbackMoney,
                IsDefault = accountDB.IsDefault,
                IsDeleted = accountDB.IsDeleted,
                IsHide = accountDB.IsHide,
                IsOverdraft = accountDB.IsOverdraft,
                LastChanges = accountDB.LastChanges,
                Name = accountDB.Name,
                ResetCachbackDate = accountDB.ResetCachbackDate,
                UserID = accountDB.UserID,
                IsCountTheBalance = accountDB.IsCountTheBalance,
                IsCountBalanceInMainAccount = accountDB.IsCountBalanceInMainAccount,
            };

            return JsonConvert.SerializeObject(account, Formatting.Indented);
        }

        public List<ShortBankTypeModelView> GetBankTypesAndAcountTypes()
        {
            List<ShortBankTypeModelView> accountTypes;

            if (cache.TryGetValue(typeof(ShortBankTypeModelView).Name, out accountTypes) == false)
            {
                accountTypes = repository.GetAll<BankType>()
                    .Where(x => x.IsVisible)
                    .Select(x => new ShortBankTypeModelView
                    {
                        ID = x.ID,
                        Name = x.Name,
                        CodeName = x.CodeName,
                        AccountTypes = x.AccountTypes
                            .Where(y => y.IsVisible)
                            .Select(y => new AccountTypeModelView
                            {
                                ID = y.ID,
                                Name = y.Name,
                                Description = y.Description,
                                Icon = y.Icon
                            })
                            .ToList()
                    })
                    .ToList();

                cache.Set(typeof(ShortBankTypeModelView).Name, accountTypes, DateTime.Now.AddDays(15));
            }

            return accountTypes;
        }

        public List<ShortPaymentSystemViewModel> GetPaymentSystems()
        {
            List<ShortPaymentSystemViewModel> paymentSystems;

            if (cache.TryGetValue(typeof(ShortPaymentSystemViewModel).Name, out paymentSystems) == false)
            {
                paymentSystems = repository.GetAll<PaymentSystem>()
                    .Where(x => x.IsVisible)
                    .Select(x => new ShortPaymentSystemViewModel
                    {
                        ID = x.ID,
                        Name = x.Name,
                        CodeName = x.CodeName,
                        Logo = x.Logo
                    })
                    .ToList();

                cache.Set(typeof(ShortPaymentSystemViewModel).Name, paymentSystems, DateTime.Now.AddDays(15));
            }

            return paymentSystems;
        }


        public List<BankTypeModelView> BankTypesAndBanks()
        {
            List<BankTypeModelView> banks;

            if (cache.TryGetValue(typeof(BankTypeModelView).Name, out banks) == false)
            {
                banks = repository.GetAll<BankType>()
                    .Where(x => x.IsVisible)
                    .Select(y => new BankTypeModelView
                    {
                        ID = y.ID,
                        CodeName = y.CodeName,
                        Name = y.Name,

                        Banks = y.Banks
                            .OrderBy(x => x.Raiting)
                            .Select(x => new ShortBankModelView
                            {
                                ID = x.ID,
                                Name = x.Name,
                                LogoCircle = x.LogoCircle,
                                LogoRectangle = x.LogoRectangle,
                                BankTypeID = x.BankTypeID,
                            })
                            .ToList()
                    })
                    .ToList();

                cache.Set(typeof(BankTypeModelView).Name, banks, DateTime.Now.AddDays(15));
            }

            return banks;
        }

        public List<ShortBankModelView> GetBanks()
        {
            List<ShortBankModelView> banks;

            if (cache.TryGetValue(typeof(ShortBankModelView).Name, out banks) == false)
            {
                banks = repository.GetAll<Bank>()
                .Select(x => new ShortBankModelView
                {
                    id = x.ID,
                    text = x.Name,

                    ID = x.ID,
                    Name = x.Name,
                    LogoCircle = x.LogoCircle,
                    LogoRectangle = x.LogoRectangle,
                    BankTypeID = x.BankTypeID,
                })
                .ToList();

                cache.Set(typeof(ShortBankModelView).Name, banks, DateTime.Now.AddDays(15));
            }

            return banks;
        }

        public List<ShortCardModelView> GetCards()
        {
            List<ShortCardModelView> cards;

            if (cache.TryGetValue(typeof(ShortCardModelView).Name, out cards) == false)
            {
                cards = repository.GetAll<Card>(x => x.AccountType.IsVisible)
                .OrderBy(x => x.Raiting)
                .Select(x => new ShortCardModelView
                {
                    id = x.ID,
                    text = x.Name, // + " - " + x.AccountType.Name + " (" + x.Bank.Name + ")",

                    ID = x.ID,
                    AccountType = (AccountTypes)x.AccountTypeID,
                    Name = x.Name,
                    AccountTypeName = x.AccountType.Name,
                    BankID = x.BankID ?? 0,
                    BankName = x.Bank.Name,
                    BankLogoRectangle = x.Bank.LogoRectangle,
                    Logo = x.SmallLogo,
                })
                .ToList();

                cache.Set(typeof(ShortCardModelView).Name, cards, DateTime.Now.AddDays(15));
            }

            return cards;
        }

    }
}
