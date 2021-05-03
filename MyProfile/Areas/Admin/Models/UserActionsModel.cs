using MyProfile.Entity.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Areas.Admin.Models
{
    public class UserActionsModel
    {
        public long ID { get; set; }
        public string Comment { get; internal set; }
        public DateTime CurrentDateTime { get; internal set; }
        public string Email { get; internal set; }
        public string UserName { get; internal set; }
        public string ActionName { get; internal set; }
    }

    public class UserActionsFilterModel
    {
        public DateTime RangeStart { get; set; }
        public DateTime RangeEnd { get; set; }
        public List<Guid> UserIDs { get; set; }
    }
    public class EmailModel 
    {
        public Guid id { get; set; }
        public string text { get; set; }

        public string UserName { get; set; }
    }
}
