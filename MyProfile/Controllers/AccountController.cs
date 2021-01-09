using Microsoft.AspNetCore.Mvc;
using MyProfile.Budget.Service;
using MyProfile.Entity.ModelView.Account;
using MyProfile.Entity.Repository;
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
            var accountTypes = accountService.GetAcountTypes();
            var banks = accountService.GetBanks();

            return Json(new { isOk = true, accountTypes, banks });
        }

        [HttpGet]
        public JsonResult GetAccounts()
        {
            var accounts = accountService.GetAcounts();
            return Json(new { isOk = true, accounts = accounts });
        }

        [HttpGet]
        public JsonResult GetShortAccounts()
        {
            var accounts = accountService.GetShortAccounts();
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
        
        [HttpPost]
        public JsonResult ShowHide([FromBody] AccountViewModel account)
        {
            var isHide = accountService.ShowHide(account);

            return Json(new { isOk = true, isHide });
        }

    }
}
