using MyProfile.Entity.ModelView.User;
using System;

namespace MyProfile.Entity.ModelView
{
    public class UserInfoModel : Model.User
    {
        public Guid UserSessionID { get; set; }
        public Guid CollectiveBudgetID { get; set; }
        public string ImageBase64 { get; set; }
        public bool IsAvailable { get; set; }
        public int WorkHours { get; set; }

        public UserInfoModel()
        {
            //this.CollectiveBudgetID = Guid.Parse("599AD733-8DB6-4689-E345-08D7EAB4CFD5");
            //this.ID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D");
            //this.IsAllowCollectiveBudget = true;
        }
    }
}
