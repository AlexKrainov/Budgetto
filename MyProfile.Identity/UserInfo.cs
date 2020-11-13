using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.User;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyProfile.Identity
{
    public static class UserInfo
    {

        public static readonly string USER_INFO = "UserInfo";
        public static Microsoft.AspNetCore.Http.HttpContext HttpContext => _accessor.HttpContext;

        /// <summary>
        /// Authorized user, if user not authorized than return NULL.
        /// </summary>
        public static UserInfoModel Current
        {
            get
            {
                //return new UserInfoModel();
                if (_accessor.HttpContext == null)
                {
                    return null;
                }
                Claim claim = _accessor.HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType);

                try
                {
                    if (claim != null && claim.Properties[USER_INFO] != null)
                    {
                        var info = claim.Properties[USER_INFO];
                        return (UserInfoModel)JsonConvert.DeserializeObject(info, typeof(UserInfoModel)); //do we need it  ? (?? new ApplicationUser())
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "ERROR, when try to get user!");
                }
                return null;
            }
        }

        private static IHttpContextAccessor _accessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _accessor = httpContextAccessor;
        }


        public static async Task AddOrUpdate_Authenticate(UserInfoModel user)
        {
            var claim = HttpContext.User.FindFirst(x => x.Value == user.Email);

            if (claim == null)
            {
                claim = new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email);
            }
            else
            {
                claim.Properties.Remove(USER_INFO);
            }

            claim.Properties.Add(USER_INFO, JsonConvert.SerializeObject(user));

            // создаем один claim
            var claims = new List<Claim>
            {
               claim// new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)
                , properties: new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddDays(3),
                    IsPersistent = true
                });
        }

        public static async Task ReSignInAsync(UserInfoModel user)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await AddOrUpdate_Authenticate(user);
        }

        /// <summary>
        /// Return UserModel for user in js in client side
        /// </summary>
        /// <returns></returns>
        public static UserInfoClientSide GetUserInfoModelForClient()
        {
            var currentUser = UserInfo.Current;
            return new UserInfoClientSide
            {
                ID =currentUser.ID,
                UserSessionID = currentUser.UserSessionID,
                CurrencyID = currentUser.Currency.ID,
                Email = currentUser.Email,
                Name = currentUser.Name,
                ImageLink = currentUser.ImageLink,
                IsAllowCollectiveBudget = currentUser.IsAllowCollectiveBudget,
                IsConfirmEmail = currentUser.IsConfirmEmail,
                LastName = currentUser.LastName,
                UserType = currentUser.UserType.CodeName,
                IsAvailable = currentUser.IsAvailable,
                Payment = new PaymentClientSide
                {
                    DateFrom = currentUser.Payment.DateFrom,
                    DateTo = currentUser.Payment.DateTo,
                    //ID = currentUser.Payment.ID,
                    Tariff = currentUser.Payment.Tariff
                },
                Currency = new CurrencyClientSide
                {
                    ID = currentUser.Currency.ID,
                    CodeName = currentUser.Currency.CodeName,
                    Icon = currentUser.Currency.Icon,
                    Name = currentUser.Currency.Name,
                    SpecificCulture = currentUser.Currency.SpecificCulture,
                    CodeName_CBR = currentUser.Currency.CodeName_CBR,
                    CodeNumber_CBR = currentUser.Currency.CodeNumber_CBR,
                },
                UserSettings = new UserSettingsClientSide
                {
                    Dashboard_Month_IsShow_BigCharts = currentUser.UserSettings.Month_BigCharts,
                    Dashboard_Month_IsShow_EarningChart = currentUser.UserSettings.Month_EarningWidget,
                    Dashboard_Month_IsShow_GoalCharts = currentUser.UserSettings.Month_GoalWidgets,
                    Dashboard_Month_IsShow_InvestingChart = currentUser.UserSettings.Month_InvestingWidget,
                    Dashboard_Month_IsShow_LimitCharts = currentUser.UserSettings.Month_LimitWidgets,
                    Dashboard_Month_IsShow_SpendingChart = currentUser.UserSettings.Month_SpendingWidget,

                    Dashboard_Year_IsShow_BigCharts = currentUser.UserSettings.Year_BigCharts,
                    Dashboard_Year_IsShow_EarningChart = currentUser.UserSettings.Year_EarningWidget,
                    Dashboard_Year_IsShow_GoalCharts = currentUser.UserSettings.Year_GoalWidgets,
                    Dashboard_Year_IsShow_InvestingChart = currentUser.UserSettings.Year_InvestingWidget,
                    Dashboard_Year_IsShow_LimitCharts = currentUser.UserSettings.Year_LimitWidgets,
                    Dashboard_Year_IsShow_SpendingChart = currentUser.UserSettings.Year_SpendingWidget,

                    WebSiteTheme = currentUser.UserSettings.WebSiteTheme,
                    IsShowHints = currentUser.UserSettings.IsShowHints,
                    IsShowFirstEnterHint = currentUser.UserSettings.IsShowFirstEnterHint,
                    IsShowConstructor = currentUser.UserSettings.IsShowConstructor,
                }
            };
        }

        public static List<MenuItemViewModel> GetMenuItems()
        {
            return new List<MenuItemViewModel>
            {
                new MenuItemViewModel
                {
                    Title = "Финансы на месяц",
                    Area ="",
                    Controller = "Budget",
                    Action = "Month",
                    Icon = "pe-7s-display1",
                    IsLastBeforeLine = false,
                },
                new MenuItemViewModel
                {
                    Title = "Финансы на год",
                    Area ="",
                    Controller = "Budget",
                    Action = "Year",
                    Icon = "pe-7s-display1",
                    IsLastBeforeLine = true,
                },
                new MenuItemViewModel
                {
                    Title = "Лимиты",
                    Area ="",
                    Controller = "Limit",
                    Action = "List",
                    Icon = "lnr lnr-frame-expand",
                    IsLastBeforeLine = false,
                },
                new MenuItemViewModel
                {
                    Title = "Цели",
                    Area ="",
                    Controller = "Goal",
                    Action = "List",
                    Icon = "lnr lnr-rocket",
                    IsLastBeforeLine = false,
                },
               
                 new MenuItemViewModel
                {
                    Title = "Шаблоны",
                    Area ="",
                    Controller = "Template",
                    Action = "List",
                    Icon = "lnr lnr-layers",
                    IsLastBeforeLine = false,
                },
                  new MenuItemViewModel
                {
                    Title = "Графики",
                    Area ="",
                    Controller = "Chart",
                    Action = "List",
                    Icon = "lnr lnr-pie-chart",
                    IsLastBeforeLine = true,
                },
                  new MenuItemViewModel
                {
                    Title = "Категории",
                    Area ="",
                    Controller = "Section",
                    Action = "Edit",
                    Icon = "pe-7s-albums",
                    IsLastBeforeLine = false,
                },
                   new MenuItemViewModel
                {
                    Title = "История",
                    Area ="",
                    Controller = "Budget",
                    Action = "TimeLine",
                    Icon = "lnr lnr-calendar-full",
                    IsLastBeforeLine = false,
                },
                    new MenuItemViewModel
                {
                    Title = "Списки",
                    Area ="",
                    Controller = "ToDoList",
                    Action = "List",
                    Icon = "lnr lnr lnr-list",
                    IsLastBeforeLine = false,
                },
                      new MenuItemViewModel
                {
                    Title = "Help center",
                    Area ="Help",
                    Controller = "Center",
                    Action = "Index",
                    Icon = "lnr lnr-question-circle",
                    IsLastBeforeLine = false,
                    ClassElement = "margin-top-auto",
                }

            };
        }
    }
}
