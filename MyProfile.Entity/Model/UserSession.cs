using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class UserSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        [StringLength(64)]
        public string IP { get; set; }
        [StringLength(32)]
        public string City { get; set; }
        [StringLength(32)]
        public string Country { get; set; }
        [StringLength(64)]
        public string Location { get; set; }
        [StringLength(32)]
        public string BrowerName { get; set; }
        [StringLength(16)]
        public string BrowserVersion { get; set; }
        [StringLength(32)]
        public string OS_Name { get; set; }
        [StringLength(16)]
        public string Os_Version { get; set; }
        [StringLength(16)]
        public string ScreenSize { get; set; }
        public string Comment { get; set; }
        public bool IsPhone { get; set; }
        public bool IsTablet { get; set; }
        public bool IsLandingPage { get; set; }
        public DateTime EnterDate { get; set; }
        public DateTime? LogOutDate { get; set; }
        public string Referrer { get; set; }
        [StringLength(32)]
        public string ContinentCode { get; set; }
        [StringLength(32)]
        public string ContinentName { get; set; }
        [StringLength(32)]
        public string Index { get; set; }
        public string Info { get; set; }
        public string ProviderInfo { get; set; }
        public string Threat { get; set; }

        [ForeignKey("User")]
        public Guid? UserID { get; set; }


        public virtual User User { get; set; }
        public virtual IEnumerable<UserLog> UserLogs { get; set; }


        public UserSession()
        {
            this.UserLogs = new HashSet<UserLog>();
        }
    }
}
