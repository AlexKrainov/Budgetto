using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.Identity;

namespace MyProfile.Controllers
{
    public class TestController : Controller
    {
        private IBaseRepository repository;
        public TestController(IBaseRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IActionResult> GenerateRecords()
        {
            var currentUser = UserInfo.Current;
            DateTime now = DateTime.Now;
            DateTime nowMinus2years = new DateTime(now.AddYears(-2).Year, 1, 1);
            var allDays = (now - nowMinus2years).TotalDays;
            var sectionSpending = await repository
                .GetAll<BudgetSection>(x => x.BudgetArea.UserID == currentUser.ID && x.SectionTypeID == (int)SectionTypeEnum.Spendings)
                .Select(x => new
                {
                    x.ID,
                    x.SectionTypeID,
                })
                .ToListAsync();
            var sectionEarnings = await repository
               .GetAll<BudgetSection>(x => x.BudgetArea.UserID == currentUser.ID && x.SectionTypeID == (int)SectionTypeEnum.Earnings)
               .Select(x => new
               {
                   x.ID,
                   x.SectionTypeID,
               })
               .ToListAsync();
            var sectionInvestments = await repository
               .GetAll<BudgetSection>(x => x.BudgetArea.UserID == currentUser.ID && x.SectionTypeID == (int)SectionTypeEnum.Investments)
               .Select(x => new
               {
                   x.ID,
                   x.SectionTypeID,
               })
               .ToListAsync();

            Random random = new Random();
            var records = new List<BudgetRecord>();

            for (int day = 0; day < allDays; day++)
            {
                //return Json(new { isOk = true }) ;
                int randomValue = random.Next(0, 1500);
                int randomSection = random.Next(0, sectionSpending.Count);

                records.Add(new BudgetRecord
                {
                    BudgetSectionID = sectionSpending[randomSection].ID,
                    DateTimeCreate = now,
                    DateTimeEdit = now,
                    DateTimeOfPayment = nowMinus2years.AddDays(day),
                    Description = null,
                    IsHide = false,
                    UserID = currentUser.ID,
                    Total = randomValue,
                    RawData = randomValue.ToString(),
                    CurrencyID = 1,
                    CurrencyNominal = 1,
                    IsShowForCollection = true,
                });

                int randomEarnings = random.Next(0, 5);
                if (randomEarnings == 4)
                {
                    randomValue = random.Next(5000, 10000);

                    randomSection = random.Next(0, sectionEarnings.Count);

                    records.Add(new BudgetRecord
                    {
                        BudgetSectionID = sectionEarnings[randomSection].ID,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = nowMinus2years.AddDays(day),
                        Description = null,
                        IsHide = false,
                        UserID = currentUser.ID,
                        Total = randomValue,
                        RawData = randomValue.ToString(),
                        CurrencyID = 1,
                        CurrencyNominal = 1,
                        IsShowForCollection = true,
                    });
                }

                int randomInvestings = random.Next(0, 12);
                if (randomInvestings == 4)
                {
                    randomValue = random.Next(20000, 40000);

                    randomSection = random.Next(0, sectionInvestments.Count);

                    records.Add(new BudgetRecord
                    {
                        BudgetSectionID = sectionInvestments[randomSection].ID,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = nowMinus2years.AddDays(day),
                        Description = null,
                        IsHide = false,
                        UserID = currentUser.ID,
                        Total = randomValue,
                        RawData = randomValue.ToString(),
                        CurrencyID = 1,
                        CurrencyNominal = 1,
                        IsShowForCollection = true,
                    });
                }

                if (records.Count == 300)
                {
                    try
                    {
                        await repository.CreateRangeAsync(records, true);
                        records = new List<BudgetRecord>();
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            try
            {
                await repository.CreateRangeAsync(records, true);
            }
            catch (Exception ex)
            {

            }

            return Json(new { isOk = true });
        }
    }
}
