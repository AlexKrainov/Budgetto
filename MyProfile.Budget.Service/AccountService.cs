using Microsoft.Extensions.Caching.Memory;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Account;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.UserLog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProfile.Budget.Service
{
    public class AccountService
    {
        private IBaseRepository repository;
        private IMemoryCache cache;
        private UserLogService userLogService;

        public AccountService(IBaseRepository repository,
            IMemoryCache cache,
            UserLogService userLogService)
        {
            this.repository = repository;
            this.cache = cache;
            this.userLogService = userLogService;
        }

        public List<AccountViewModel> GetAcounts()
        {
            var currentUserID = UserInfo.Current.ID;

            return repository.GetAll<Account>(x => x.UserID == currentUserID && x.IsDeleted == false)
                .Select(x => new AccountViewModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    Balance = x.Balance,
                    Description = x.Description,
                    IsDefault = x.IsDefault,
                    IsHide = x.IsHide,
                    AccountType = (AccountTypesEnum)x.AccountTypeID,
                    AccountTypeName = x.AccountType.Name,
                    AccountIcon = x.AccountType.Icon,
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
                    BankImage = x.Bank != null ? x.Bank.ImageSrc : "",
                    ExpirationDate = x.ExpirationDate,
                    InterestRate = x.InterestRate,
                    IsOverdraft = x.IsOverdraft,

                    IsCachback = x.IsCachback,
                    IsCachBackMoney = x.IsCachbackMoney,
                    CachBackBalance = x.CachbackBalance,
                    CashBackForAllPercent = x.CachbackForAllPercent,
                    ResetCashBackDate = x.ResetCachbackDate,

                    IsShow = true,
                })
                .OrderByDescending(x => x.IsDefault)
                .ThenBy(x => x.IsHide)
                .ThenBy(x => x.AccountType == AccountTypesEnum.Cash)
                .ThenBy(x => x.AccountType == AccountTypesEnum.Installment)
                .ThenBy(x => x.AccountType == AccountTypesEnum.Credit)
                .ThenBy(x => x.AccountType == AccountTypesEnum.Debed)
                .ToList();
        }

        public List<AccountViewModel> GetAcountsPast(DateTime start, DateTime finish, List<AccountViewModel> accounts)
        {
            var currentUser = UserInfo.Current;

            var records = repository.GetAll<Record>(x => x.UserID == currentUser.ID
             && x.IsDeleted == false
             && x.AccountID != null
             && x.DateTimeOfPayment >= start && x.DateTimeOfPayment <= finish
             && x.AccountRecordHistories.Count > 0)
                  .Select(x => x.AccountRecordHistories
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

            foreach (var account in accounts)
            {
                account.IsPast = true;
                //account.Balance += records.Where(x => x.SectionTypeID == (int)SectionTypeEnum.Spendings && (x.AccountID ?? 0) == account.ID).Sum(x => x.AccountTotal);
                //account.CachBackBalance += records.Where(x => x.SectionTypeID == (int)SectionTypeEnum.Spendings && (x.AccountID ?? 0) == account.ID).Sum(x => x.AccountCashback);

                account.BalanceSpendings += records.Where(x => x.SectionTypeID == (int)SectionTypeEnum.Spendings && (x.AccountID ?? 0) == account.ID).Sum(x => x.AccountTotal);
                account.BalanceEarnings += records.Where(x => x.SectionTypeID == (int)SectionTypeEnum.Earnings && (x.AccountID ?? 0) == account.ID).Sum(x => x.AccountTotal);
                account.BalanceInvestments += records.Where(x => x.SectionTypeID == (int)SectionTypeEnum.Investments && (x.AccountID ?? 0) == account.ID).Sum(x => x.AccountTotal);
            }

            return accounts;
        }

        public List<AccountShortViewModel> GetShortAccounts()
        {
            List<AccountShortViewModel> accounts;
            var currentUserID = UserInfo.Current.ID;

            if (cache.TryGetValue(typeof(AccountShortViewModel).Name + "_" + currentUserID, out accounts) == false)
            {
                accounts = repository.GetAll<Account>(x => x.UserID == currentUserID)
                 .Select(x => new AccountShortViewModel
                 {
                     ID = x.ID,
                     Name = x.Name,
                     IsDefault = x.IsDefault,
                     AccountType = (AccountTypesEnum)x.AccountTypeID,
                     AccountIcon = x.AccountType.Icon,
                     CurrencyID = x.CurrencyID,
                     CurrencySpecificCulture = x.Currency.SpecificCulture,
                     CurrencyCodeName = x.Currency.CodeName,
                     CurrencyIcon = x.Currency.Icon,

                     BankID = x.BankID,
                     BankName = x.Bank != null ? x.Bank.Name : "",
                     BankImage = x.Bank != null ? x.Bank.ImageSrc : "",

                     IsDeleted = x.IsDeleted
                 })
                 .ToList();

                cache.Set(typeof(AccountShortViewModel).Name + "_" + currentUserID, accounts, DateTime.Now.AddDays(2));
            }
            return accounts;
        }

        public bool ShowHide(AccountViewModel account)
        {
            var currentUser = UserInfo.Current;
            var accountDB = repository.GetAll<Account>(x => x.UserID == currentUser.ID && x.ID == account.ID).FirstOrDefault();

            try
            {
                accountDB.IsHide = !account.IsHide;
                accountDB.LastChanges = DateTime.Now.ToUniversalTime();

                repository.Update(accountDB, true);
            }
            catch (Exception ex)
            {
                userLogService.CreateErrorLog(currentUser.UserSessionID, "AccountService.ShowHide", ex, account.IsDeleted ? UserLogActionType.Account_Delete : UserLogActionType.Account_Recovery);
            }

            return account.IsHide;
        }

        public AccountViewModel Save(AccountViewModel account)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();

            if (account.ID <= 0)
            {
                Account accountDB = new Account
                {
                    Name = account.Name,
                    AccountTypeID = (int)account.AccountType,
                    Balance = account.Balance,
                    UserID = currentUser.ID,
                    IsDefault = account.IsDefault,
                    Description = account.Description,

                    BankID = account.BankID,
                    ExpirationDate = account.ExpirationDate,
                    InterestRate = account.InterestRate,
                    IsOverdraft = account.IsOverdraft,

                    IsCachback = account.IsCachback,
                    IsCachbackMoney = account.IsCachBackMoney,
                    CachbackBalance = account.CachBackBalance,
                    CachbackForAllPercent = account.CashBackForAllPercent,
                    ResetCachbackDate = account.ResetCashBackDate,

                    IsHide = account.IsHide,

                    DateCreate = now,
                    CurrencyID = account.CurrencyID, // ruble
                };

                try
                {
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
                    //accountDB.AccountTypeID = (int)account.AccountType;
                    accountDB.Balance = account.Balance;
                    accountDB.Description = account.Description;
                    accountDB.IsHide = account.IsHide;
                    accountDB.LastChanges = now;
                    accountDB.Name = account.Name;
                    accountDB.IsDefault = account.IsDefault;
                    //accountDB.CurrencyID = account.CurrencyID;

                    if (accountDB.AccountType.ID != (int)AccountTypesEnum.Cash)
                    {
                        accountDB.BankID = account.BankID;
                        accountDB.IsCachback = account.IsCachback;
                        accountDB.IsCachbackMoney = account.IsCachBackMoney;
                        accountDB.CachbackBalance = account.CachBackBalance;
                        accountDB.CachbackForAllPercent = account.CashBackForAllPercent;
                        accountDB.ExpirationDate = account.ExpirationDate;
                        accountDB.IsOverdraft = account.IsOverdraft;
                        accountDB.InterestRate = account.InterestRate;
                        accountDB.ResetCachbackDate = account.ResetCashBackDate;
                    }
                    else
                    {
                        accountDB.BankID = null;
                        accountDB.IsCachback = false;
                        accountDB.IsCachbackMoney = false;
                        accountDB.CachbackForAllPercent = null;
                        accountDB.ExpirationDate = null;
                        accountDB.IsOverdraft = false;
                        accountDB.InterestRate = null;
                        accountDB.ResetCachbackDate = null;
                    }

                    try
                    {
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

            cache.Remove(typeof(AccountShortViewModel).Name + "_" + currentUser.ID);

            return account;
        }

        public bool RemoveOrRecovery(AccountViewModel account, ref int accountWithID)
        {
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
                accountDB.IsDeleted = account.IsDeleted;
                accountDB.IsDefault = false;
                accountDB.LastChanges = DateTime.Now.ToUniversalTime();

                repository.Update(accountDB, true);

                if (account.IsDefault)
                {
                    accountWithID = UpdateIsDefaultAccount(accountDB);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <returns>ID of isDefault account</returns>
        private int UpdateIsDefaultAccount(Account account)
        {
            int accountIDWithIsDefault = -1;

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
                                        && x.AccountTypeID == (int)AccountTypesEnum.Cash)
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

        public List<AccountTypeModelView> GetAcountTypes()
        {
            List<AccountTypeModelView> accountTypes;

            if (cache.TryGetValue(typeof(AccountTypeModelView).Name, out accountTypes) == false)
            {
                accountTypes = repository.GetAll<AccountType>()
                .Select(x => new AccountTypeModelView
                {
                    ID = x.ID,
                    Name = x.Name,
                    Description = x.Description,
                    Icon = x.Icon,
                    accountType = (AccountTypesEnum)x.ID
                })
                .Take(2)
                .ToList();

                cache.Set(typeof(AccountTypeModelView).Name, accountTypes, DateTime.Now.AddDays(15));
            }

            return accountTypes;
        }

        public List<BankModelView> GetBanks()
        {
            List<BankModelView> banks;

            if (cache.TryGetValue(typeof(BankModelView).Name, out banks) == false)
            {
                banks = repository.GetAll<Bank>()
                .Select(x => new BankModelView
                {
                    ID = x.ID,
                    Name = x.Name,
                    ImageSrc = x.ImageSrc,
                })
                .ToList();

                cache.Set(typeof(BankModelView).Name, banks, DateTime.Now.AddDays(15));
            }

            return banks;
        }

    }
}
