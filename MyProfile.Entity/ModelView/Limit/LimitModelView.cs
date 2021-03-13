using MyProfile.Entity.ModelView.Notification;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Limit
{
    public class LimitModelView : Model.Limit
    {
        public IEnumerable<BudgetSectionModelView> Sections { get; set; } = new List<BudgetSectionModelView>();
        public IEnumerable<BudgetSectionModelView> NewSections { get; set; } = new List<BudgetSectionModelView>();
        public string PeriodName { get; set; }

        public bool IsOwner { get; set; }
        public string UserName { get; set; }
        public string ImageLink { get; set; }
        public bool IsShowInCollective { get; set; }
        public bool IsShowOnDashboard { get; set; }
        public bool IsShow { get; set; } = true;
        public List<NotificationUserViewModel> Notifications { get; set; } = new List<NotificationUserViewModel>();
    }
}
