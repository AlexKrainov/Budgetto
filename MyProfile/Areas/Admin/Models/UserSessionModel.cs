using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Areas.Admin.Models
{
    public class UserSessionModel
    {
        public Guid ID { get; internal set; }
        public DateTime EnterDate { get; internal set; }
        public string IP { get; internal set; }
        public bool IsLandingPage { get; internal set; }
        public string UserName { get; internal set; }
        public string Email { get; internal set; }
        public bool IsPhone { get; internal set; }
        public bool IsTablet { get; internal set; }
        public string Place { get; internal set; }
        public string BrowerName { get; internal set; }
        public string OS_Name { get; internal set; }
        public string ScreenSize { get; internal set; }
        public string Referrer { get; internal set; }
        public int IPCounter { get; internal set; }
    }

    public class UserSessionFilterModel
    {
        public DateTime RangeStart { get; set; }
        public DateTime RangeEnd { get; set; }
        public List<Guid> UserIDs { get; set; }
    }
}
