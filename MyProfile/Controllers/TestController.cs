using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.User.Service;

namespace MyProfile.Controllers
{
    public class TestController : Controller
    {
        private IBaseRepository repository;
        private UserLogService userLogService;
        private UserService userService;

        public TestController(IBaseRepository repository,
            UserLogService userLogService,
            UserService userService)
        {
            this.repository = repository;
            this.userLogService = userLogService;
            this.userService = userService;
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
            await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.ADMIN_GenerateRecords);
            return Json(new { isOk = true });
        }

        public IActionResult ClearAccountForConstructor()
        {
            var currentUser = UserInfo.Current;
            List<int> errorLogCreateIDs = new List<int>();

            try
            {
                var userSettings = repository.GetAll<MyProfile.Entity.Model.UserSettings>(x => x.ID == currentUser.ID).FirstOrDefault();
                userSettings.IsShowConstructor = true;
                repository.Update(userSettings);

                var goals = repository.GetAll<MyProfile.Entity.Model.Goal>(x => x.UserID == currentUser.ID && x.IsCreatedByConstructor).ToList();
                repository.DeleteRange(goals);

                var limits = repository.GetAll<MyProfile.Entity.Model.Limit>(x => x.UserID == currentUser.ID && x.IsCreatedByConstructor).ToList();
                repository.DeleteRange(limits, true);

                var templates = repository.GetAll<MyProfile.Entity.Model.Template>(x => x.UserID == currentUser.ID && x.IsCreatedByConstructor).ToList();
                repository.DeleteRange(templates, true);

                var sections = repository.GetAll<MyProfile.Entity.Model.BudgetSection>(x => x.BudgetArea.UserID == currentUser.ID
                && x.BudgetRecords.Count() == 0 && x.IsCreatedByConstructor).ToList();
                repository.DeleteRange(sections, true);

                var areas = repository.GetAll<MyProfile.Entity.Model.BudgetArea>(x => x.UserID == currentUser.ID
                && x.BudgetSectinos.Count() == 0 && x.IsCreatedByConstructor).ToList();
                repository.DeleteRange(areas, true);
            }
            catch (Exception ex)
            {
                errorLogCreateIDs.Add(userLogService.CreateErrorLog(currentUser.UserSessionID, "ADMIN_ClearAccountForConstructor", ex));
            }

            userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.ADMIN_ClearAccount);

            return Json(new { isOk = true });
        }
    }
}
