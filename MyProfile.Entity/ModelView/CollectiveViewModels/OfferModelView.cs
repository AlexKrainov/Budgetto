using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.CollectiveViewModels
{
    public class OfferModelView
    {
        public DateTime DateAdded { get; set; }
        public DateTime DateUpdate { get; set; }
        public Guid CollectiveBudgetID { get; set; }
        public string OwnerName { get; set; }
        public string OwnerLastName { get; set; }
        public string OwnerImageLink { get; set; }
        public string OwnerEmail { get; set; }
        public int OfferID { get; set; }
    }
}
