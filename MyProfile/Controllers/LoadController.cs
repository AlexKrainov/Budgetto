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

        public IActionResult NewDashboard()
        {
            return View();
        }

        public IActionResult ParsingTable()
        {
            return View();
        }

        public IActionResult ParsingCard()
        {
            return View();
        }

        public IActionResult ParsingCreditCard()
        {
            return View();
        }
        
        public IActionResult ParsingRasCard()
        {
            return View();
        }


        [HttpPost]
        public JsonResult Load([FromBody] List<Month1> months)
        {
            int currentMonth = 0;
            var now = DateTime.Now.ToUniversalTime();

            List<Record> records = new List<Record>();
            //Guid userID = Guid.Parse("0C499EA3-9749-4879-F9C1-08D8530A928F");//test
            Guid userID = Guid.Parse("5A4454D6-3706-473D-57CA-08D88D635029");//prod

            foreach (var day in months)
            {
                if (day.day == "day")
                {
                    currentMonth = currentMonth + 1;
                    continue;
                }
                var date = new DateTime(2016, currentMonth, int.Parse(day.day), 13, 00, 00);
                //"1265.00 ₽"
                if (!string.IsNullOrEmpty(day.product))
                {
                    day.product = day.product.Replace(" ₽", "");

                    records.Add(new Record
                    {
                        UserID = userID,
                        CurrencyID = 1,
                        BudgetSectionID = 29,
                        CurrencyNominal = 1,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = date,
                        Total = Decimal.Parse(day.product),
                        RawData = day.product,
                        Description = string.IsNullOrEmpty(day.anotherSpending) ? day.comment : null,
                    });
                }

                if (!string.IsNullOrEmpty(day.spending) && !string.IsNullOrEmpty(day.comment) && day.comment.Contains("Материалы для ремонта"))
                {
                    day.spending = day.spending.Replace(" ₽", "");

                    records.Add(new Record
                    {
                        UserID = userID,
                        CurrencyID = 1,
                        BudgetSectionID = 40,//ремонт
                        CurrencyNominal = 1,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = date,
                        Total = Decimal.Parse(day.spending),
                        RawData = day.spending,
                        Description = day.comment
                    });
                }else if (!string.IsNullOrEmpty(day.spending))
                {
                    day.spending = day.spending.Replace(" ₽", "");

                    records.Add(new Record
                    {
                        UserID = userID,
                        CurrencyID = 1,
                        BudgetSectionID = 66,
                        CurrencyNominal = 1,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = date,
                        Total = Decimal.Parse(day.spending),
                        RawData = day.spending
                    });
                }

                if (!string.IsNullOrEmpty(day.babySpending))
                {
                    day.babySpending = day.babySpending.Replace(" ₽", "");

                    records.Add(new Record
                    {
                        UserID = userID,
                        CurrencyID = 1,
                        BudgetSectionID = 56,
                        CurrencyNominal = 1,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = date,
                        Total = Decimal.Parse(day.babySpending),
                        RawData = day.babySpending
                    });
                }

                if (!string.IsNullOrEmpty(day.anotherSpending))
                {
                    day.anotherSpending = day.anotherSpending.Replace(" ₽", "");

                    records.Add(new Record
                    {
                        UserID = userID,
                        CurrencyID = 1,
                        BudgetSectionID = 66,
                        CurrencyNominal = 1,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = date,
                        Total = Decimal.Parse(day.anotherSpending),
                        RawData = day.anotherSpending,
                        Description = day.comment
                    });
                }

                if (!string.IsNullOrEmpty(day.home)) 
                {
                    day.home = day.home.Replace(" ₽", "");
                    var m = Decimal.Parse(day.home);
                    var BudgetSectionID = 36;

                    if (m <= 2300 && m >= 1900 && m == 700)
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
                    else if (m >= 1100 && m <= 1900 || m >= 550 && m <= 1000)
                    {
                        BudgetSectionID = 39; //электричество
                    }
                    //else if (m == 169)
                    //{
                    //    BudgetSectionID = 74; //яндекс плюс
                    //}
                    //else if (m == 299)
                    //{
                    //    BudgetSectionID = 74; //ютуб
                    //}
                    else if (m <= 150)
                    {
                        BudgetSectionID = 57; //банкинг
                    }

                    records.Add(new Record
                    {
                        UserID = userID,
                        CurrencyID = 1,
                        BudgetSectionID = BudgetSectionID,
                        CurrencyNominal = 1,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = date,
                        Total = m,
                        RawData = day.home,
                        Description = day.comment2
                    });
                }

                if (!string.IsNullOrEmpty(day.selery))
                {
                    day.selery = day.selery.Replace(" ₽", "");

                    records.Add(new Record
                    {
                        UserID = userID,
                        CurrencyID = 1,
                        BudgetSectionID = 47,//??
                        CurrencyNominal = 1,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = date,
                        Total = Decimal.Parse(day.selery),
                        RawData = day.selery
                    });
                }

                if (!string.IsNullOrEmpty(day.anotherSelery))
                {
                    day.anotherSelery = day.anotherSelery.Replace(" ₽", "");

                    records.Add(new Record
                    {
                        UserID = userID,
                        CurrencyID = 1,
                        BudgetSectionID = 46,//??
                        CurrencyNominal = 1,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = date,
                        Total = Decimal.Parse(day.anotherSelery),
                        RawData = day.anotherSelery
                    });
                }

                if (!string.IsNullOrEmpty(day.cashback))
                {
                    day.cashback = day.cashback.Replace(" ₽", "");

                    records.Add(new Record
                    {
                        UserID = userID,
                        CurrencyID = 1,
                        BudgetSectionID = 45,//??
                        CurrencyNominal = 1,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = date,
                        Total = Decimal.Parse(day.cashback),
                        RawData = day.cashback
                    });
                }

                if (!string.IsNullOrEmpty(day.investing))
                {
                    day.investing = day.investing.Replace(" ₽", "");

                    records.Add(new Record
                    {
                        UserID = userID,
                        CurrencyID = 1,
                        BudgetSectionID = 102,//Вклад
                        CurrencyNominal = 1,
                        DateTimeCreate = now,
                        DateTimeEdit = now,
                        DateTimeOfPayment = date,
                        Total = Decimal.Parse(day.investing),
                        RawData = day.investing
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
            public string cashback { get; set; }
            public string anotherSelery { get; set; }
            public string selery { get; set; }
            public string investing { get; set; }
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
