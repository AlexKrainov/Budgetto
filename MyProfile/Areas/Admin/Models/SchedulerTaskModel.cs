using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Areas.Admin.Models
{
    public class SchedulerTaskModel
    {
        public string Name { get; internal set; }
        public DateTime? FirstStart { get; internal set; }
        public DateTime? LastStart { get; internal set; }
        public string TaskStatus { get; internal set; }
        public string CronComment { get; internal set; }
        public string CronExpression { get; internal set; }
        public string Comment { get; internal set; }
        public int ID { get; internal set; }
        public DateTime? NextStart { get; internal set; }
        public bool IsMissed { get; internal set; }
    }

    public class SchedulerTaskFilter
    {

    }
}
