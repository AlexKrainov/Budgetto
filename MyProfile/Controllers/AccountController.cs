using Microsoft.AspNetCore.Mvc;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Account;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.UserLog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    public class AccountController : Controller
    {
        private IBaseRepository repository;
        private BudgetRecordService budgetRecordService;
        private UserLogService userLogService;
        private AccountService accountService;

        public AccountController(IBaseRepository repository,
            AccountService accountService,
            BudgetRecordService budgetRecordService,
            UserLogService userLogService)
        {
            this.repository = repository;
            this.budgetRecordService = budgetRecordService;
            this.userLogService = userLogService;
            this.accountService = accountService;
        }

        [HttpGet]
        public JsonResult GetEnvironment()
        {
            var bankTypes = accountService.GetBankTypesAndAcountTypes();
            var paymentSystems = accountService.GetPaymentSystems();

            return Json(new { isOk = true, bankTypes, paymentSystems });
        }
        
        [HttpGet]
        public JsonResult GetEnvironmentForMain()
        {
            var bankTypes = accountService.BankTypesAndBanks();

            return Json(new { isOk = true, bankTypes });
        }

        [HttpGet]
        public JsonResult GetAccounts(DateTime? date, int year, PeriodTypesEnum periodType)
        {
            DateTime start;
            DateTime finish;
            DateTime now = DateTime.Now;
            List<MainAccountModelView> mainAccounts = new List<MainAccountModelView>();

            if (date.HasValue)
            {
                start = new DateTime(date.Value.Year, date.Value.Month, 01, 00, 00, 00);
                finish = new DateTime(date.Value.Year, date.Value.Month, DateTime.DaysInMonth(date.Value.Year, date.Value.Month), 23, 59, 59);
            }
            else
            {
                start = new DateTime(year, 1, 01, 00, 00, 00);
                finish = new DateTime(year, 12, 31, 23, 59, 59);
            }

            mainAccounts = accountService.GetMainAccounts(UserInfo.Current.ID); // current data of accounts (month is now or year is now)
            bool isPast = now >= start && now >= finish;

            if (isPast)
            {
                accountService.GetAcountsAllMoneyByPeriod(start, finish, mainAccounts);
            }

            return Json(new { isOk = true, accounts = mainAccounts, isPast });
        }

        [HttpGet]
        public JsonResult GetShortAccounts()
        {
            var accounts = accountService.GetShortAccounts(UserInfo.Current.ID);
            return Json(new
            {
                isOk = true,
                accounts = accounts,
                accountByDefault = accounts.FirstOrDefault(x => x.IsDefault)
            });
        }

        [HttpPost]
        public JsonResult Save([FromBody] AccountViewModel account)
        {
            var newAccount = accountService.Save(account);
            return Json(new { isOk = true, account = newAccount });
        }
        
        [HttpPost]
        public JsonResult RemoveOrRecovery([FromBody] AccountViewModel account)
        {
            int accountIDWithIsDefault = -1;
            var isDeleted = accountService.RemoveOrRecovery(account, ref accountIDWithIsDefault);

            return Json(new { isOk = true, isDeleted, accountIDWithIsDefault });
        }

        [HttpGet]
        public JsonResult Toggle(int accountID)
        {
            var isHide = accountService.ToggleShowHide(accountID);

            return Json(new { isOk = true, isHide });
        }

        [HttpPost]
        public JsonResult TransferMoney([FromBody] TransferMoney transfer)
        {
            accountService.Transfer(transfer);
            return Json(new { isOk = true});
        }


    }
}
