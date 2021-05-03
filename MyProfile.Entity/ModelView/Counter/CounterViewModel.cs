using MyProfile.Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Counter
{
    public class CounterViewModel
    {
        public BudgettoEntityType EntityType { get; set; }
        public int AddedCount { get; set; }
        public DateTime LastChanges { get; set; }
        public int CanBeCountByTariff { get; set; }
    }
}
