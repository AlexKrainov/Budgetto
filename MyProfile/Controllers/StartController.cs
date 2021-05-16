using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.Goal;
using MyProfile.Entity.ModelView.Limit;
using MyProfile.Entity.ModelView.TemplateModelView;
using MyProfile.Entity.Repository;
using MyProfile.Goal.Service;
using MyProfile.Identity;
using MyProfile.Limit.Service;
using MyProfile.Template.Service;
using MyProfile.User.Service;
using MyProfile.UserLog.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    [Authorize]
    public class StartController : Controller
    {
        private IBaseRepository repository;
        private TemplateService templateService;
        private SectionService sectionService;
        private UserService userService;
        private UserLogService userLogService;
        private LimitService limitService;
        private GoalService goalService;
        private SummaryService summaryService;

        public StartController(IBaseRepository repository,
            UserService userService,
            TemplateService templateService,
            LimitService limitService,
            GoalService goalService,
            SectionService sectionService,
            UserLogService userLogService,
            SummaryService summaryService)
        {
            this.repository = repository;
            this.templateService = templateService;
            this.sectionService = sectionService;
            this.userService = userService;
            this.userLogService = userLogService;
            this.limitService = limitService;
            this.goalService = goalService;
            this.summaryService = summaryService;
        }

        public IActionResult Index()
        {
            var currentUser = UserInfo.Current;

            if (currentUser.UserSettings.IsShowConstructor)
            {
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

                return View();
            }
            else
            {
                return RedirectToAction("Month", "Budget");
            }
        }

        [HttpGet]
        public IActionResult LoadUserInfo()
        {
            var currentUser = UserInfo.Current;
            int allWorkHours = 0;

            var userSummary = repository.GetAll<UserSummary>(x => x.UserID == currentUser.ID
                       && x.SummaryID == (int)SummaryType.EarningsPerHour
                       && x.IsActive
                       && x.Value != null)
                 .FirstOrDefault();

            if (userSummary != null && int.TryParse(userSummary.Value, out int hours))
            {
                allWorkHours = hours;
            }

            return Json(new { isOk = true, userInfo = new { name = currentUser.Name, allWorkHours, allWorkDays = 0 } });
        }
        [HttpPost]
        public async Task<IActionResult> SaveUserInfo([FromBody] UserInfoModel userInfo)
        {
            var currentUser = UserInfo.Current;
            currentUser.Name = userInfo.Name;
            currentUser.TimeZoneClient = userInfo.TimeZoneClient;

            await userService.UpdateUser(currentUser);
            await summaryService.SetWorkHoursAsync(userInfo.AllWorkHours, userInfo.AllWorkDays);
            await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Constructor_Step1_UserInfo);

            return Json(new { isOk = true, user = UserInfo.GetUserInfoModelForClient() });
        }

        [HttpPost]
        public async Task<IActionResult> SaveSections([FromBody] List<BudgetAreaModelView> areas)
        {
            var currentUser = UserInfo.Current;
            List<long> errorLogCreateIDs = new List<long>();
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
                            BaseSectionID = (int)section.ID,
                            CssBackground = section.CssBackground,
                            CssColor = section.CssColor,
                            CssIcon = section.CssIcon,
                            IsShowInCollective = true,
                            Description = section.Description,
                            IsShowOnSite = true,
                            Name = section.Name,
                            SectionTypeID = section.SectionTypeID,
                            IsCreatedByConstructor = true,
                            IsCashback = true,
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
                        IsCreatedByConstructor = true
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

            await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Constructor_Step2_Sections, errorLogIDs: errorLogCreateIDs);

            return Json(new { isOk = true, sections = sectionService.GetAllSectionForRecords(withoutCache: true) });
        }

        [HttpPost]
        public async Task<IActionResult> SaveTemplate([FromBody] TemplateViewModel template)
        {
            var currentUser = UserInfo.Current;
            List<long> errorLogCreateIDs = new List<long>();
            TemplateErrorModelView templateResult = new TemplateErrorModelView();

            try
            {
                template.PeriodTypeID = (int)PeriodTypesEnum.Month;
                template.Name = "Шаблон на месяц";
                template.IsShow = true;
                template.IsDefault = true;
                template.IsCreatedByConstructor = true;

                for (int i = 0; i < template.Columns.Count(); i++)
                {
                    template.Columns[i].Order = i;
                    template.Columns[i].IsShow = true;
                    template.Columns[i].TotalAction = FooterActionType.Sum;
                }

                templateResult = await templateService.SaveTemplate(template, false);

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

            await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Constructor_Step3_Template, errorLogIDs: errorLogCreateIDs);

            return Json(templateResult);
        }

        [HttpPost]
        public async Task<IActionResult> SaveLimits([FromBody] List<LimitModelView> limits)
        {
            var currentUser = UserInfo.Current;
            List<long> errorLogCreateIDs = new List<long>();

            try
            {
                foreach (var limit in limits)
                {
                    limit.ID = limit.ID < 0 ? 0 : limit.ID;
                    limit.IsCreatedByConstructor = true;

                    await limitService.UpdateOrCreate(limit);
                }
            }
            catch (System.Exception ex)
            {
                errorLogCreateIDs.Add(await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "Start_SaveLimits", ex));
            }

            await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Constructor_Step4_Limits, errorLogIDs: errorLogCreateIDs);

            return Json(new { isOk = true, sections = sectionService.GetAllSectionForRecords() });
        }

        [HttpPost]
        public async Task<IActionResult> SaveGoals([FromBody] List<GoalModelView> goals)
        {
            var currentUser = UserInfo.Current;
            List<long> errorLogCreateIDs = new List<long>();

            try
            {
                foreach (var goal in goals)
                {
                    goal.ID = goal.ID < 0 ? 0 : goal.ID;
                    goal.IsCreatedByConstructor = true;

                    await goalService.UpdateOrCreate(goal);
                }
            }
            catch (System.Exception ex)
            {
                errorLogCreateIDs.Add(await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "Start_SaveGoals", ex));
            }

            await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Constructor_Step5_Goals, errorLogIDs: errorLogCreateIDs);

            return Json(new { isOk = true, sections = sectionService.GetAllSectionForRecords() });
        }

        public async Task<IActionResult> Finish()
        {
            var currentUser = UserInfo.Current;
            List<long> errorLogCreateIDs = new List<long>();

            try
            {
                currentUser.UserSettings.IsShowConstructor = false;

                await userService.UpdateUser(currentUser, userSettingsSave: true);
            }
            catch (System.Exception ex)
            {
                errorLogCreateIDs.Add(await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "Start_Finish", ex));
            }

            await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Constructor_Step6_Finish, errorLogIDs: errorLogCreateIDs);

            return Json(new { isOk = true });
        }

        [HttpGet]
        public IActionResult LoadSections()
        {
            var baseSections = sectionService.GetBaseSections()
                .Select(x => new BudgetSectionModelView
                {
                    ID = x.SectionID,
                    Name = x.SectionName,
                    CssIcon = x.Icon,
                    CssColor = x.Color,
                    CssBackground = x.Background,
                    IsShowInCollective = true,
                    IsShowOnSite = true,
                    AreaID = x.AreaID,
                    AreaName = x.AreaName,
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeName = x.SectionTypeID == (int)SectionTypeEnum.Spendings ? "Расходы" : "Доходы",
                    SectionTypeID = x.SectionTypeID,
                    IsCashback = true,
                })
                .ToList();



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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeName = "Расходы",
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
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
                    AreaName = "Расходы",
                    IsSelected = false,
                    IsShow_Filtered = true,
                    IsShow = true,
                    SectionTypeID = (int)SectionTypeEnum.Spendings,
                    SectionTypeName = "Расходы",
                    IsCashback = true,
                },

               //Доходы
                new BudgetSectionModelView
                {
                    ID = 23,
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
                new BudgetSectionModelView
                {
                    ID = 24,
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
                    ID = 25,
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
                    ID = 26,
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

                ////Инвестиции
                //new BudgetSectionModelView
                //{
                //    ID = 27,
                //    Name = "Вклады",
                //    CodeName = "Deposit",
                //    CssIcon = "fas fa-donate",
                //    CssColor = "rgb(255, 255, 255)",
                //    CssBackground = "rgb(158, 158, 158)",
                //    IsShowInCollective = true,
                //    IsShowOnSite = true,
                //    AreaID = 2,
                //    AreaName = "Инвестиции",
                //    IsSelected = false,
                //    IsShow_Filtered = true,
                //    IsShow = true,
                //    SectionTypeID = (int)SectionTypeEnum.Investments,
                //    SectionTypeName = "Инвестиции",
                //},
                //new BudgetSectionModelView
                //{
                //    ID = 28,
                //    Name = "Инвестиции",
                //    CodeName = "Investment",
                //    CssIcon = "fas fa-donate",
                //    CssColor = "rgb(255, 255, 255)",
                //    CssBackground = "rgb(158, 158, 158)",
                //    IsShowInCollective = true,
                //    IsShowOnSite = true,
                //    AreaID = 2,
                //    AreaName = "Инвестиции",
                //    IsSelected = false,
                //    IsShow_Filtered = true,
                //    IsShow = true,
                //    SectionTypeID = (int)SectionTypeEnum.Investments,
                //    SectionTypeName = "Инвестиции",
                //},
            };

            //var userSections = await sectionService.GetAllSectionForRecords();

            return Json(new { isOk = true, sections = baseSections });
        }
    }
}
