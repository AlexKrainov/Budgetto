using MyProfile.Entity.ModelView.AreaAndSection;
using MyProfile.Entity.ModelView.CommonViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView
{


    public class TimeLineViewModel
    {
        public List<YearsAndCount> Years { get; set; } = new List<YearsAndCount>();
        public IEnumerable<SectionLightModelView> Sections { get; set; } = new List<SectionLightModelView>();
    }

    public class YearsAndCount
    {
        public int year { get; set; }
        public int count { get; set; }
    }
    public class DateForCalendar
    {
        public DateTime date { get; set; }
        public decimal count { get; set; }
    }

    public class CalendarFilterModels
    {
        public int Year { get; set; }
        public List<long> Sections { get; set; } = new List<long>();
        public List<long> Tags { get; set; } = new List<long>();
        public bool IsAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsConsiderCollection { get; set; } = true;
        public Guid UserID { get; set; }
        /// <summary>
        /// Search records by section or tags
        /// </summary>
        public bool IsSection { get; set; } = true;

        public CalendarFilterModels()
        {
            this.Sections = new List<long>();
        }
    }
}
