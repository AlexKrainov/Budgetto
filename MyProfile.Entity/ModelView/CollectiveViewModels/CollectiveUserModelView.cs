using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.CollectiveViewModels
{
    public class CollectiveUserModelView
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ImageLink { get; set; }
        public string Status { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateUpdate { get; set; }
        public Guid CollectiveBudgetID { get; set; }
    }
}
