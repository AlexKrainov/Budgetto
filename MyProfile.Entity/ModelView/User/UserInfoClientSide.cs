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

        public virtual UserSettingsClientSide UserSettings { get; set; }
        public virtual CurrencyClientSide Currency { get; set; }
    }

    public class UserSettingsClientSide
    {
        public bool Dashboard_Month_IsShow_EarningChart { get; set; }
        public bool Dashboard_Month_IsShow_SpendingChart { get; set; }
        public bool Dashboard_Month_IsShow_InvestingChart { get; set; }

        public bool Dashboard_Month_IsShow_LimitCharts { get; set; }

        public bool Dashboard_Month_IsShow_GoalCharts { get; set; }

        public bool Dashboard_Month_IsShow_BigCharts { get; set; }
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
}
