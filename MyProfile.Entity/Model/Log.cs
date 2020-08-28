using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    /// <summary>
    /// Use this object for error on the site
    /// </summary>
    public class Log
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public DateTime CurrentDateTime { get; set; }
        public string Where { get; set; }
        public string Comment { get; set; }
        public string ErrorText { get; set; }

        [ForeignKey("User")]
        public Guid? UserID { get; set; }
        [ForeignKey("UserLog")]
        public int? UserLogID { get; set; }


        public virtual User User { get; set; }
        public virtual UserLog UserLog { get; set; }

    }
}
