using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public sealed class WebSiteThemeEnum
    {
        public static readonly string Light = "light";
        public static readonly string Dark = "dark";
    }

    public class UserSettings
    {
        [JsonIgnore]
        [Key]
        [ForeignKey("User")]
        public Guid ID { get; set; }

        /// <summary>
        /// The pages Budget/Month, Budget/Year ... consider money with collective or without
        /// </summary>
        public bool BudgetPages_WithCollective { get; set; }
        /// <summary>
        /// The pages Budget/Month, Budget/Year ... show/hide Earning Chart
        /// </summary>
        public bool BudgetPages_EarningChart { get; set; }
        /// <summary>
        /// The pages Budget/Month, Budget/Year ... show/hide Spending Chart
        /// </summary>
        public bool BudgetPages_SpendingChart { get; set; }
        /// <summary>
        /// The pages Budget/Month, Budget/Year ... show/hide Invest Chart
        /// </summary>
        public bool BudgetPages_InvestingChart { get; set; }

        #region Limit
        /// <summary>
        /// The pages Budget/Month, Budget/Year ... show/hide Limits 
        /// </summary>
        public bool BudgetPages_IsShow_Limits { get; set; }
        /// <summary>
        /// The pages /Limit/List, show/hide isFinished limits
        /// </summary>
        public bool LimitPage_Show_IsFinished { get; set; }
        /// <summary>
        /// The pages Limit/List ... show/hide collective limits 
        /// </summary>
        public bool LimitPage_IsShow_Collective { get; set; }
        #endregion

        /// <summary>
        /// The pages Budget/Month, Budget/Year ... show/hide goals 
        /// </summary>
        public bool BudgetPages_IsShow_Goals { get; set; }
        /// <summary>
        /// The pages Budget/Month, Budget/Year ... show/hide big charts 
        /// </summary>
        public bool BudgetPages_IsShow_BigCharts { get; set; }


        /// <summary>
        /// The pages Goal/List ... show/hide isFinished goals 
        /// </summary>
        public bool GoalPage_IsShow_Finished { get; set; }
        /// <summary>
        /// The pages Goal/List ... show/hide collective goals 
        /// </summary>
        public bool GoalPage_IsShow_Collective { get; set; }
        public string WebSiteTheme_CodeName { get; set; }

        public virtual User User { get; set; }

    }
}
