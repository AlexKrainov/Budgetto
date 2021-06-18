using MyProfile.Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.User
{
    public class UserInfoClientSide
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsConfirmEmail { get; set; }
        public string ImageLink { get; set; }
        public bool IsAllowCollectiveBudget { get; set; }
        public int CurrencyID { get; set; }
        public string ImageBase64 { get; set; }

        public virtual UserSettingsClientSide UserSettings { get; set; }
        public virtual CurrencyClientSide Currency { get; set; }
        public EarningsPerHourModelView EarningsPerHour { get; set; }
        public string UserType { get; set; }
        public Guid CollectiveBudgetID { get; set; }
        public DateTime DateCreate { get; set; }
        public bool IsAvailable { get; set; }
        public PaymentClientSide Payment { get; set; }
        public Guid ID { get; set; }
        public Guid UserSessionID { get; set; }
        public bool IsHelpRecord { get; set; }

        public string TelegramLogin { get; set; }
        public List<TelegramAccountClientSide> TelegramAccounts { get; set; }
    }

    public class UserSettingsClientSide
    {
        public bool Dashboard_Month_IsShow_EarningChart { get; set; }
        public bool Dashboard_Month_IsShow_SpendingChart { get; set; }
        public bool Dashboard_Month_IsShow_InvestingChart { get; set; }

        public bool Dashboard_Month_IsShow_LimitCharts { get; set; }

        public bool Dashboard_Month_IsShow_GoalCharts { get; set; }

        public bool Dashboard_Month_IsShow_BigCharts { get; set; }
        public bool Dashboard_Year_IsShow_BigCharts { get; set; }
        public bool Dashboard_Year_IsShow_EarningChart { get; set; }
        public bool Dashboard_Year_IsShow_GoalCharts { get; set; }
        public bool Dashboard_Year_IsShow_InvestingChart { get; set; }
        public bool Dashboard_Year_IsShow_LimitCharts { get; set; }
        public bool Dashboard_Year_IsShow_SpendingChart { get; set; }
        public string WebSiteTheme { get; set; }
        public bool Mail_News { get; set; }
        public bool IsShowHints { get; set; }
        public bool IsShowFirstEnterHint { get; set; }
        public bool IsShowConstructor { get; set; }
        public bool IsShowCookie { get; set; }
        public bool Mail_Reminders { get; set; }
        public bool CanUseAlgorithm { get; set; }
        public bool Dashboard_Month_IsShow_Accounts { get; set; }
        public bool Dashboard_Month_IsShow_Summary { get; set; }
        public bool Dashboard_Year_IsShow_Accounts { get; set; }
        public bool Dashboard_Year_IsShow_Summary { get; set; }
        public bool Dashboard_Month_IsShow_ToDoLists { get; set; }
        public bool Dashboard_Year_IsShow_ToDoLists { get; set; }
        public bool Dashboard_Month_IsShow_ProgressBar { get; set; }
        public bool Dashboard_Month_IsShow_Statistics { get; set; }
        public bool Dashboard_Year_IsShow_Statistics { get; set; }
    }
    public class CurrencyClientSide
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string CodeName { get; set; }
        public string SpecificCulture { get; set; }
        public string Icon { get; set; }
        public string CodeName_CBR { get; set; }
        public int? CodeNumber_CBR { get; set; }
    }

    public class PaymentClientSide
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public PaymentTariffTypes TariffType { get; set; }
    }

    public class TelegramAccountClientSide
    {
        public long TelegramID { get; set; }
        public string Username { get; set; }
        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public int ID { get; set; }
    }
}
