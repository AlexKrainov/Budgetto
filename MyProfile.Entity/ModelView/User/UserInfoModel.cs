using MyProfile.Entity.ModelView.Counter;
using System;
using System.Collections.Generic;

namespace MyProfile.Entity.ModelView
{
    public class UserInfoModel : Model.User
    {
        public Guid UserSessionID { get; set; }
        public Guid CollectiveBudgetID { get; set; }
        public string ImageBase64 { get; set; }
        
        public int AllWorkHours { get; set; }
        public int AllWorkDays { get; set; }
        /// <summary>
        /// Doesn't have any records
        /// </summary>
        public bool IsHelpRecord { get; set; }
        public string TimeZoneClient { get; set; }
        public bool IsCompleteIntroductoryProgress { get; set; }

        public List<CounterViewModel> Counters { get; set; }

        public UserInfoModel()
        {
            //this.CollectiveBudgetID = Guid.Parse("599AD733-8DB6-4689-E345-08D7EAB4CFD5");
            //this.ID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D");
            //this.IsAllowCollectiveBudget = true;
        }
    }
}
