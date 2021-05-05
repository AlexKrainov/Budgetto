using MyProfile.Entity.Model;
using System;
using System.Collections.Generic;

namespace MyProfile.Entity.ModelView
{
    public class SummaryFilter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<int>  SectionTypes { get; set; }
        public List<long> Sections { get; set; }
        public Guid UserID { get; set; }
        public PeriodTypesEnum PeriodType { get; set; }
    }
}
