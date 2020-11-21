using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    /// <summary>
    /// For block user by ip (if it's DDoS attack)
    /// </summary>
    public class IPSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [StringLength(64)]
        public string IP { get; set; }
        public bool IsBlock { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastVisit { get; set; }
        public int Counter { get; set; }
    }
}
