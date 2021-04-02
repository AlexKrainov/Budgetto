﻿using Common.Service;
using Microsoft.Extensions.Caching.Memory;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.UserLog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProfile.Budget.Service
{
    public partial class AccountService
    {

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
            }
            return count;
        }

        public bool CalculateDeposit(Account account)
        {
            var now = DateTime.Now.ToUniversalTime().Date;
            bool isAdded = false;

            decimal percentPerDay = (decimal)account.AccountInfo.InterestRate / 366;
            decimal lastPeriodDays = 0;

            while (account.AccountInfo.InterestNextDate.Value.Date <= now && account.AccountInfo.IsFinishedDeposit == false)
            {
                account.AccountInfo.LastInterestAccrualDate = account.AccountInfo.InterestNextDate;
                switch ((TimeList)account.AccountInfo.CapitalizationOfDeposit)
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
                    account.AccountInfo.IsFinishedDeposit = true;

                    //Set to notification to user
                }
                lastPeriodDays = (decimal)((account.AccountInfo.LastInterestAccrualDate.Value.Date - account.AccountInfo.InterestNextDate.Value.Date).TotalDays) * -1;

                var percentPerPeriod = lastPeriodDays * percentPerDay;
                var lastInterestBalance = (account.Balance + (account.AccountInfo.InterestBalance ?? 0)) / 100 * percentPerPeriod;

                if (account.AccountInfo.LastInterestAccrualDate.Value.Date != account.DateStart.Value.Date 
                    && account.AccountInfo.LastInterestAccrualDate.Value.Date <= now.Date)
                {
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
    }
}