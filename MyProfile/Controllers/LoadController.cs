using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyProfile.Budget.Service;
using MyProfile.Code;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.BudgetView;
using MyProfile.Entity.ModelView.TemplateModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Template.Service;
using MyProfile.User.Service;
using MyProfile.UserLog.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    public class LoadController : Controller
    {
        private IBaseRepository repository;
        private TemplateService templateService;
        private BudgetService budgetService;
        private SectionService sectionService;
        private BudgetRecordService budgetRecordService;
        private UserLogService userLogService;

        public LoadController(IBaseRepository repository,
            BudgetService budgetService,
            TemplateService templateService,
            SectionService sectionService,
            BudgetRecordService budgetRecordService,
            UserLogService userLogService)
        {
            this.repository = repository;
            this.templateService = templateService;
            this.budgetService = budgetService;
            this.sectionService = sectionService;
            this.budgetRecordService = budgetRecordService;
            this.userLogService = userLogService;
        }
        public IActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public JsonResult Load([FromBody] List<Month1> months)
        {
            int currentMonth = 0;
            var now = DateTime.Now.ToUniversalTime();

            List<BudgetRecord> records = new List<BudgetRecord>();

            foreach (var month in months)
            {
                if (month.day == "day")
                {
                    currentMonth = currentMonth + 1;
                    continue;
                }
                var date = new DateTime(2020, currentMonth, int.Parse(month.day), 13, 00, 00);
                //"1265.00 ₽"
                if (!string.IsNullOrEmpty(month.product))
                {
                    month.product = month.product.Replace(" ₽", "");

                    records.Add(new BudgetRecord
                    {
                        UserID = Guid.Parse("5A4454D6-3706-473D-57CA-08D88D635029"),
                        CurrencyID = 1,
                        BudgetSectionID = 29,
                        CurrencyNominal = 1,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = date,
                        Total = Decimal.Parse(month.product),
                        RawData = month.product
                    });
                }

                if (!string.IsNullOrEmpty(month.spending))
                {
                    month.spending = month.spending.Replace(" ₽", "");

                    records.Add(new BudgetRecord
                    {
                        UserID = Guid.Parse("5A4454D6-3706-473D-57CA-08D88D635029"),
                        CurrencyID = 1,
                        BudgetSectionID = 66,
                        CurrencyNominal = 1,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = date,
                        Total = Decimal.Parse(month.spending),
                        RawData = month.spending
                    });
                }

                if (!string.IsNullOrEmpty(month.babySpending))
                {
                    month.babySpending = month.babySpending.Replace(" ₽", "");

                    records.Add(new BudgetRecord
                    {
                        UserID = Guid.Parse("5A4454D6-3706-473D-57CA-08D88D635029"),
                        CurrencyID = 1,
                        BudgetSectionID = 56,
                        CurrencyNominal = 1,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = date,
                        Total = Decimal.Parse(month.babySpending),
                        RawData = month.babySpending
                    });
                }

                if (!string.IsNullOrEmpty(month.anotherSpending))
                {
                    month.anotherSpending = month.anotherSpending.Replace(" ₽", "");

                    records.Add(new BudgetRecord
                    {
                        UserID = Guid.Parse("5A4454D6-3706-473D-57CA-08D88D635029"),
                        CurrencyID = 1,
                        BudgetSectionID = 66,
                        CurrencyNominal = 1,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = date,
                        Total = Decimal.Parse(month.anotherSpending),
                        RawData = month.anotherSpending,
                        Description = month.comment
                    });
                }

                if (!string.IsNullOrEmpty(month.tax) && month.tax != " должен вернуть (132к + 5% - 10к(мамины за подарок) - 7.4к (за подарок для мамы 5к и 2.4к за шашлык) + 1.5к (за сыр)) - 10к (за ворота) - 9к ( мама отдала)")
                {
                    month.tax = month.tax.Replace(" ₽", "");

                    records.Add(new BudgetRecord
                    {
                        UserID = Guid.Parse("5A4454D6-3706-473D-57CA-08D88D635029"),
                        CurrencyID = 1,
                        BudgetSectionID = 50,
                        CurrencyNominal = 1,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = date,
                        Total = Decimal.Parse(month.tax),
                        RawData = month.tax
                    });
                }

                if (!string.IsNullOrEmpty(month.home))
                {
                    month.home = month.home.Replace(" ₽", "");
                    var m = Decimal.Parse(month.home);
                    var BudgetSectionID = 36;

                    if (m <= 2300 && m >= 1900)
                    {
                        BudgetSectionID = 37; //связь
                    }
                    else if (m >= 4500)
                    {
                        BudgetSectionID = 38; //жкх
                    }
                    else if (m >= 1000 && m <= 1100)
                    {
                        BudgetSectionID = 59; //вода
                    }
                    else if (m >= 1100 && m <= 1900 || m >= 800 && m <= 1000)
                    {
                        BudgetSectionID = 39; //электричество
                    }
                    else if (m == 169)
                    {
                        BudgetSectionID = 74; //яндекс плюс
                    }
                    else if (m == 299)
                    {
                        BudgetSectionID = 74; //ютуб
                    }
                    else if (m <= 100)
                    {
                        BudgetSectionID = 57; //банкинг
                    }

                    records.Add(new BudgetRecord
                    {
                        UserID = Guid.Parse("5A4454D6-3706-473D-57CA-08D88D635029"),
                        CurrencyID = 1,
                        BudgetSectionID = BudgetSectionID,
                        CurrencyNominal = 1,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = date,
                        Total = m,
                        RawData = month.home
                    });
                }

            }
            repository.CreateRange(records, true);
            return Json(new { isOk = true });
        }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Month1
        {
            public string day { get; set; }
            public string product { get; set; }
            public string home { get; set; }
            public string comment2 { get; set; }
            public string anotherSpending { get; set; }
            public string comment { get; set; }
            public string babySpending { get; set; }
            public string spending { get; set; }
            public string cashBack { get; set; }
            public string tax { get; set; }
        }

        public class Month2
        {
            public string day { get; set; }
            public string product { get; set; }
            public string spending { get; set; }
            public string babySpending { get; set; }
            public string anotherSpending { get; set; }
            public string comment { get; set; }
            public string home { get; set; }
            public string comment2 { get; set; }
            public string cashBack { get; set; }
        }

        public class Month3
        {
            public string day { get; set; }
            public string product { get; set; }
            public string spending { get; set; }
            public string home { get; set; }
            public string comment2 { get; set; }
            public string anotherSpending { get; set; }
            public string comment { get; set; }
            public string cashBack { get; set; }
            public string tax { get; set; }
            public string babySpending { get; set; }
        }

        public class Month4
        {
            public string day { get; set; }
            public string product { get; set; }
            public string home { get; set; }
            public string comment2 { get; set; }
            public string spending { get; set; }
            public string anotherSpending { get; set; }
            public string comment { get; set; }
            public string tax { get; set; }
            public string babySpending { get; set; }
            public string cashBack { get; set; }
        }

        public class Month5
        {
            public string day { get; set; }
            public string product { get; set; }
            public string anotherSpending { get; set; }
            public string comment { get; set; }
            public string home { get; set; }
            public string comment2 { get; set; }
            public string cashBack { get; set; }
            public string babySpending { get; set; }
            public string tax { get; set; }
            public string spending { get; set; }
        }

        public class Month6
        {
            public string day { get; set; }
            public string product { get; set; }
            public string home { get; set; }
            public string spending { get; set; }
            public string comment { get; set; }
            public string anotherSpending { get; set; }
            public string comment2 { get; set; }
            public string cashBack { get; set; }
            public string babySpending { get; set; }
            public string tax { get; set; }
        }

        public class Month7
        {
            public string day { get; set; }
            public string product { get; set; }
            public string spending { get; set; }
            public string anotherSpending { get; set; }
            public string comment { get; set; }
            public string home { get; set; }
            public string comment2 { get; set; }
            public string cashBack { get; set; }
        }

        public class Month8
        {
            public string day { get; set; }
            public string product { get; set; }
            public string spending { get; set; }
            public string home { get; set; }
            public string comment2 { get; set; }
            public string anotherSpending { get; set; }
            public string cashBack { get; set; }
            public string comment { get; set; }
            public string babySpending { get; set; }
        }

        public class Root
        {
            public List<Month1> month1 { get; set; }
            public List<Month2> month2 { get; set; }
            public List<Month3> month3 { get; set; }
            public List<Month4> month4 { get; set; }
            public List<Month5> month5 { get; set; }
            public List<Month6> month6 { get; set; }
            public List<Month7> month7 { get; set; }
            public List<Month8> month8 { get; set; }
        }



    }
}
