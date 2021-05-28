using MyProfile.Entity.Model;
using System;
using System.Linq;

namespace MyProfile.Budget.Service
{
    public partial class AccountService
    {
        public int AccountDailyWork()
        {
            int count = 0;

            count += CheckDepositForInterest();
            count += CheckGracePeriod();

            return count;
        }

        #region Credit card
        private int CheckGracePeriod()
        {
            var now = DateTime.Now.Date;

            var accounts = repository.GetAll<AccountInfo>(x => x.Account.AccountTypeID == (int)AccountTypes.Credit
                && x.CreditExpirationDate.Value.Date <= now
                && x.Account.IsDeleted == false)
              .ToList();

            for (int i = 0; i < accounts.Count; i++)
            {
                accounts[i].CreditExpirationDate = accounts[i].CreditExpirationDate.Value.AddMonths(1);
                repository.Update(accounts[i], true);
            }

            return accounts.Count();
        }


        #endregion

        #region Deposit
        public int CheckDepositForInterest()
        {
            int count = 0;
            var accounts = repository.GetAll<Account>(x => x.AccountTypeID == (int)AccountTypes.Deposit && x.AccountInfo.IsFinishedDeposit == false && x.IsDeleted == false)
                .ToList();

            for (int i = 0; i < accounts.Count; i++)
            {
                if (CalculateDeposit(accounts[i]))
                {
                    count += 1;
                }
                repository.Update(accounts[i], true);
            }
            return count;
        }

        public bool CalculateDeposit(Account account, bool isNew = false)
        {
            var nowDate = DateTime.Now.ToUniversalTime().Date;
            bool isAdded = false;

            decimal percentPerDay = (decimal)account.AccountInfo.InterestRate / 366;
            decimal lastPeriodDays = 0;

            while (account.AccountInfo.InterestNextDate.Value.Date <= nowDate && account.AccountInfo.IsFinishedDeposit == false)
            {
                account.AccountInfo.LastInterestAccrualDate = account.AccountInfo.InterestNextDate;
                switch ((TimeList)account.AccountInfo.CapitalizationTimeListID)
                {
                    case TimeList.Daily:
                        account.AccountInfo.InterestNextDate = account.AccountInfo.InterestNextDate.Value.AddDays(1).Date;
                        break;
                    case TimeList.Weekly:
                        account.AccountInfo.InterestNextDate = account.AccountInfo.InterestNextDate.Value.AddDays(7).Date;
                        break;
                    case TimeList.Monthly:
                        account.AccountInfo.InterestNextDate = account.AccountInfo.InterestNextDate.Value.AddMonths(1).Date;
                        break;
                    case TimeList.Quarterly:
                        account.AccountInfo.InterestNextDate = account.AccountInfo.InterestNextDate.Value.AddMonths(3).Date;
                        break;
                    case TimeList.HalfYearly:
                        account.AccountInfo.InterestNextDate = account.AccountInfo.InterestNextDate.Value.AddMonths(6).Date;
                        break;
                    case TimeList.Yearly:
                        account.AccountInfo.InterestNextDate = account.AccountInfo.InterestNextDate.Value.AddYears(1).Date;
                        break;
                    default:
                        break;
                }

                if (account.AccountInfo.InterestNextDate.Value.Date >= account.ExpirationDate.Value.Date)
                {
                    account.AccountInfo.InterestNextDate = account.ExpirationDate.Value.Date;

                    //Set to notification to user
                    //By default show on the site
                }
                if (nowDate >= account.ExpirationDate.Value.Date)
                {
                    account.AccountInfo.IsFinishedDeposit = true;
                }
                lastPeriodDays = (decimal)((account.AccountInfo.LastInterestAccrualDate.Value.Date - account.AccountInfo.InterestNextDate.Value.Date).TotalDays) * -1;

                var percentPerPeriod = lastPeriodDays * percentPerDay;
                decimal totalBalance = account.Balance;

                if (account.AccountInfo.IsCapitalization)
                {
                    totalBalance = account.Balance + (account.AccountInfo.InterestBalance ?? 0);
                }

                decimal lastInterestBalance = totalBalance / 100 * percentPerPeriod;

                if ((account.AccountInfo.LastInterestAccrualDate.Value.Date != account.DateStart.Value.Date
                        && account.AccountInfo.LastInterestAccrualDate.Value.Date <= nowDate.Date
                        && isNew == false)
                    || (isNew
                        && account.AccountInfo.LastInterestAccrualDate.Value.Date == nowDate.Date))
                {
                    account.AccountHistories.Add(new AccountHistory
                    {
                        ActionType = AccountHistoryActionType.AddedPercent,
                        CurrentDate = account.AccountInfo.LastInterestAccrualDate.Value.Date,
                        OldBalance = account.AccountInfo.InterestBalance,
                        NewBalance = account.AccountInfo.InterestBalance + lastInterestBalance,
                        ValueTo = lastInterestBalance,
                        StateField = $"{{ balance: {account.Balance}}}"
                    });

                    account.AccountInfo.InterestBalance += lastInterestBalance;
                    isAdded = true;
                }
            }
            return isAdded;
        }


        public decimal СalculateEndDeposit(decimal balance, decimal interestRate, DateTime startDate, DateTime endDate, TimeList timeList)
        {
            var now = DateTime.Now.ToUniversalTime().Date;

            decimal percentPerDay = (decimal)interestRate / 366;
            decimal lastPeriodDays = 0;
            decimal interestBalance = 0;
            DateTime lastInterestAccrualDate = startDate;

            while (startDate < endDate)
            {
                lastInterestAccrualDate = startDate;
                switch (timeList)
                {
                    case TimeList.Daily:
                        startDate = startDate.AddDays(1).Date;
                        break;
                    case TimeList.Weekly:
                        startDate = startDate.AddDays(7).Date;
                        break;
                    case TimeList.Monthly:
                        startDate = startDate.AddMonths(1).Date;
                        break;
                    case TimeList.Quarterly:
                        startDate = startDate.AddMonths(3).Date;
                        break;
                    case TimeList.HalfYearly:
                        startDate = startDate.AddMonths(6).Date;
                        break;
                    case TimeList.Yearly:
                        startDate = startDate.AddYears(1).Date;
                        break;
                    default:
                        break;
                }

                if (startDate.Date >= endDate)
                {
                    startDate = endDate;
                }
                lastPeriodDays = (decimal)((lastInterestAccrualDate - startDate).TotalDays) * -1;

                var percentPerPeriod = lastPeriodDays * percentPerDay;
                var lastInterestBalance = (balance + interestBalance) / 100 * percentPerPeriod;
                interestBalance += lastInterestBalance;
            }

            return interestBalance;
        }

        public void GetLastPeriod(DateTime dateEnd, DateTime lastDate, DateTime nextDate, TimeList timeList)
        {
            while (nextDate > dateEnd)
            {
                lastDate = nextDate;
                switch (timeList)
                {
                    case TimeList.Daily:
                        nextDate = nextDate.AddDays(1).Date;
                        break;
                    case TimeList.Weekly:
                        nextDate = nextDate.AddDays(7).Date;
                        break;
                    case TimeList.Monthly:
                        nextDate = nextDate.AddMonths(1).Date;
                        break;
                    case TimeList.Quarterly:
                        nextDate = nextDate.AddMonths(3).Date;
                        break;
                    case TimeList.HalfYearly:
                        nextDate = nextDate.AddMonths(6).Date;
                        break;
                    case TimeList.Yearly:
                        nextDate = nextDate.AddYears(1).Date;
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
    }
}
