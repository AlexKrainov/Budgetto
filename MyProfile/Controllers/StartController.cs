using Microsoft.AspNetCore.Mvc;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.Goal;
using MyProfile.Entity.ModelView.Limit;
using MyProfile.Entity.Repository;
using MyProfile.Goal.Service;
using MyProfile.Identity;
using MyProfile.Limit.Service;
using MyProfile.Template.Service;
using MyProfile.User.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    public class StartController : Controller
    {
        private IBaseRepository repository;
        private TemplateService templateService;
        private BudgetService budgetService;
        private SectionService sectionService;
        private UserService userService;
        private UserLogService userLogService;
        private LimitService limitService;
        private GoalService goalService;

        public StartController(IBaseRepository repository,
            UserService userService,
            BudgetService budgetService,
            TemplateService templateService,
            LimitService limitService,
            GoalService goalService,
            SectionService sectionService,
            UserLogService userLogService)
        {
            this.repository = repository;
            this.templateService = templateService;
            this.budgetService = budgetService;
            this.sectionService = sectionService;
            this.userService = userService;
            this.userLogService = userLogService;
            this.limitService = limitService;
            this.goalService = goalService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveUserInfo([FromBody] UserInfoModel userInfo)
        {
            var currentUser = UserInfo.Current;
            currentUser.Name = userInfo.Name;

            await userService.UpdateUser(currentUser);
            await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Constructor_Step1_UserInfo);

            return Json(new { isOk = true, user = UserInfo.GetUserInfoModelForClient() });
        }

        [HttpPost]
        public async Task<IActionResult> SaveSections([FromBody] List<BudgetAreaModelView> areas)
        {
            var currentUser = UserInfo.Current;
            List<int> errorLogCreateIDs = new List<int>();
            List<BudgetArea> budgetAreas = new List<BudgetArea>();
            //List<BudgetSectionModelView> sectionForUpdate = new List<BudgetSectionModelView>();

            //todo: update
            try
            {
                foreach (var area in areas)
                {
                    List<BudgetSection> budgetSections = new List<BudgetSection>();

                    foreach (var section in area.Sections)
                    {
                        budgetSections.Add(new BudgetSection
                        {
                            CodeName = section.CodeName,
                            CssBackground = section.CssBackground,
                            CssColor = section.CssColor,
                            CssIcon = section.CssIcon,
                            IsShowInCollective = true,
                            Description = section.Description,
                            IsShowOnSite = true,
                            Name = section.Name,
                            SectionTypeID = section.SectionTypeID,
                        });
                    }


                    budgetAreas.Add(new BudgetArea
                    {
                        CodeName = area.CodeName,
                        Name = area.Name,
                        IsShowOnSite = true,
                        IsShowInCollective = true,
                        UserID = currentUser.ID,
                        BudgetSectinos = budgetSections,
                    });
                }

                await repository.CreateRangeAsync(budgetAreas, true);

                //foreach (var section in sectionForUpdate)
                //{
                //    var dbsection = await repository.GetAll<BudgetSection>(x => x.ID == section.ID && x.BudgetArea.UserID == currentUser.ID)
                //        .FirstOrDefaultAsync();

                //    if (dbsection != null)
                //    {
                //        dbsection.Name = section.Name;
                //        dbsection.CssBackground = section.CssBackground;
                //        dbsection.CssColor = section.CssColor;
                //        dbsection.CssIcon = section.CssIcon;
                //        dbsection.SectionTypeID = section.SectionTypeID;

                //        await repository.SaveAsync();
                //    }
                //}
            }
            catch (System.Exception ex)
            {
                errorLogCreateIDs.Add(await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "Start_SaveSections", ex));
            }

            await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Constructor_Step2_Sections, errorLogIDs: errorLogCreateIDs);

            return Json(new { isOk = true, sections = await sectionService.GetAllSectionForRecords() });
        }

        [HttpPost]
        public async Task<IActionResult> SaveTemplate([FromBody] TemplateViewModel template)
        {
            var currentUser = UserInfo.Current;
            List<int> errorLogCreateIDs = new List<int>();

            try
            {
                template.PeriodTypeID = (int)PeriodTypesEnum.Month;
                template.Name = "Шаблон на месяц";

                for (int i = 0; i < template.Columns.Count(); i++)
                {
                    template.Columns[i].Order = i;
                }

                var templateResult = await templateService.SaveTemplate(template, false);

                template.ID = 0;
                template.Name = "Шаблон на год";
                template.PeriodTypeID = (int)PeriodTypesEnum.Year;
                template.Columns[0] = new Column
                {
                    TemplateColumnType = TemplateColumnType.MonthsForYear,
                    IsShow = true,
                    Name = "Месяц",
                    Order = 0,
                    Formula = new List<FormulaItem>(),
                };
                await templateService.SaveTemplate(template, false);

            }
            catch (System.Exception ex)
            {
                errorLogCreateIDs.Add(await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "Start_SaveTemplate", ex));
            }

            await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Constructor_Step3_Template, errorLogIDs: errorLogCreateIDs);

            return Json(new { isOk = true, sections = await sectionService.GetAllSectionForRecords() });
        }

        [HttpPost]
        public async Task<IActionResult> SaveLimits([FromBody] List<LimitModelView> limits)
        {
            var currentUser = UserInfo.Current;
            List<int> errorLogCreateIDs = new List<int>();

            try
            {
                foreach (var limit in limits)
                {
                    limit.ID = limit.ID < 0 ? 0 : limit.ID;

                    await limitService.UpdateOrCreate(limit);
                }
            }
            catch (System.Exception ex)
            {
                errorLogCreateIDs.Add(await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "Start_SaveLimits", ex));
            }

            await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Constructor_Step4_Limits, errorLogIDs: errorLogCreateIDs);

            return Json(new { isOk = true, sections = await sectionService.GetAllSectionForRecords() });
        }

        [HttpPost]
        public async Task<IActionResult> SaveGoals([FromBody] List<GoalModelView> goals)
        {
            var currentUser = UserInfo.Current;
            List<int> errorLogCreateIDs = new List<int>();

            try
            {
                foreach (var goal in goals)
                {
                    goal.ID = goal.ID < 0 ? 0 : goal.ID;

                    await goalService.UpdateOrCreate(goal);
                }
            }
            catch (System.Exception ex)
            {
                errorLogCreateIDs.Add(await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "Start_SaveGoals", ex));
            }

            await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Constructor_Step5_Goals, errorLogIDs: errorLogCreateIDs);

            return Json(new { isOk = true, sections = await sectionService.GetAllSectionForRecords() });
        }

        [HttpGet]
        public async Task<IActionResult> LoadSections()
        {
            var sections = new List<BudgetSectionModelView>
            {
                //Расходы
                new BudgetSectionModelView
                {
                    ID = 1,
                    Name = "Продукты",
                    CodeName = "Products",
                    CssIcon = "fas fa-shopping-cart",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeName = "Расходы",
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                },
                new BudgetSectionModelView
                {
                    ID = 2,
                    Name = "Фастфуд",
                    CodeName = "FastFood",
                    CssIcon = "fas fa-hamburger",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 3,
                    Name = "Рестораны",
                    CodeName = "Restorans",
                    CssIcon = "fas fa-glass-cheers",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 4,
                    Name = "Подарки",
                    Description = "",
                    CodeName = "Gifts",
                    CssIcon = "fas fa-birthday-cake",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 5,
                    Name = "Налоги",
                    CodeName = "Taxes",
                    CssIcon = "fas fa-landmark",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 6,
                    Name = "Одежда",
                    CodeName = "Clothes",
                    CssIcon = "fas fa-tshirt",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 7,
                    Name = "Красота",
                    Description="Парикмахерские, салоны красоты и тд.",
                    CodeName = "Beauty",
                    CssIcon = "far fa-eye",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 8,
                    Name = "Развлечения",
                    CodeName = "Entertainment",
                    CssIcon = "fas fa-biking",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 9,
                    Name = "Ремонт",
                    Description = "",
                    CodeName = "Renovation",
                    CssIcon = "fas fa-paint-roller",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 10,
                    Name = "Крупная бытовая тех.",
                    Description = "Телевизор, пылесос, стиральная машинка и тд.",
                    CodeName = "LargeHouseholdAppliances",
                    CssIcon = "fas fa-tv",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 11,
                    Name = "Все для дома",
                    Description = "Телевизор, пылесос, стиральная машинка и тд.",
                    CodeName = "AllForHouse",
                    CssIcon = "fas fa-couch",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 12,
                    Name = "Электричество",
                    CodeName = "Electricity",
                    CssIcon = "fas fa-plug",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 13,
                    Name = "За квартиру",
                    CodeName = "ForTheApartment",
                    CssIcon = "far fa-building",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 14,
                    Name = "Связь",
                    CodeName = "Communication",
                    CssIcon = "fas fa-phone",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 15,
                    Name = "Интернет",
                    CodeName = "Internet",
                    CssIcon = "fas fa-wifi",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 16,
                    Name = "Стоматология",
                    CodeName = "Dentistry",
                    CssIcon = "fas fa-tooth",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 17,
                    Name = "Посещение врача",
                    Description = "Прием врача, консультация, массаж, все возможные обследования",
                    CodeName = "VisitDoctor",
                    CssIcon = "fas fa-user-md",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 18,
                    Name = "Аптеки",
                    CodeName = "Pharmacy",
                    CssIcon = "fas fa-prescription-bottle-alt",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 19,
                    Name = "Путешествия",
                    CodeName = "Travels",
                    CssIcon = "fas fa-plane",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 20,
                    Name = "Страховка",
                    Description = "Осаго, каско и тд",
                    CodeName = "CarInsurance",
                    CssIcon = "fas fa-car",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 21,
                    Name = "Бензин",
                    CodeName = "Petrol",
                    CssIcon = "fas fa-fill-drip",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },
                new BudgetSectionModelView
                {
                    ID = 22,
                    Name = "Ремонт авто",
                    CodeName = "AutoRepair",
                    CssIcon = "fas fa-hammer",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(244, 67, 54)",
                    IsShowOnSite = true,
                    AreaID = 0,
                    AreaName = "Раходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                },

               //Доходы
                new BudgetSectionModelView
                {
                    ID = 23,
                    Name = "Дивиденды",
                    Description = "Дивиденды от депозитов, акций, облигаций и тд",
                    CodeName = "Dividends",
                    CssIcon = "fas fa-piggy-bank",
                    CssColor = "rgba(24, 28, 33, 0.8)",
                    CssBackground = "rgb(0, 230, 118)",
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = 1,
                    AreaName = "Доходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Earnings,
                    SectionTypeName = "Доходы",
                },
                new BudgetSectionModelView
                {
                    ID = 24,
                    Name = "Кэшбэки",
                    CodeName = "Cashback",
                    CssIcon = "fas fa-ruble-sign",
                    CssColor = "rgba(24, 28, 33, 0.8)",
                    CssBackground = "rgb(0, 230, 118)",
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = 1,
                    AreaName = "Доходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Earnings,
                    SectionTypeName = "Доходы",
                },
                new BudgetSectionModelView
                {
                    ID = 25,
                    Name = "Доп. доходы",
                    CodeName = "OtherIncome",
                    CssIcon = "fas fa-donate",
                    CssColor = "rgba(24, 28, 33, 0.8)",
                    CssBackground = "rgb(0, 230, 118)",
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = 1,
                    AreaName = "Доходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Earnings,
                    SectionTypeName = "Доходы",
                },
                new BudgetSectionModelView
                {
                    ID = 26,
                    Name = "Зарплата",
                    CodeName = "Salary",
                    CssIcon = "fas fa-wallet",
                    CssColor = "rgba(24, 28, 33, 0.8)",
                    CssBackground = "rgb(0, 230, 118)",
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = 1,
                    AreaName = "Доходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Earnings,
                    SectionTypeName = "Доходы",
                },

                //Инвестиции
                new BudgetSectionModelView
                {
                    ID = 27,
                    Name = "Инвестиции",
                    CodeName = "Investment",
                    CssIcon = "fas fa-donate",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(158, 158, 158)",
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = 2,
                    AreaName = "Инвестиции",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Investments,
                    SectionTypeName = "Инвестиции",
                },
                new BudgetSectionModelView
                {
                    ID = 28,
                    Name = "Вклады",
                    CodeName = "deposit",
                    CssIcon = "fas fa-donate",
                    CssColor = "rgb(255, 255, 255)",
                    CssBackground = "rgb(158, 158, 158)",
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = 2,
                    AreaName = "Инвестиции",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Investments,
                    SectionTypeName = "Инвестиции",
                },
            };

            var userSections = await sectionService.GetAllSectionForRecords();

            return Json(new { isOk = true, sections, userSections });
        }
    }
}
