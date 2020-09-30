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
    public class ErrorLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public DateTime CurrentDate { get; set; }
        public string Where { get; set; }
        public string Comment { get; set; }
        public string ErrorText { get; set; }

        [ForeignKey("UserSession")]
        public Guid UserSessionID { get; set; }


        public virtual UserSession UserSession { get; set; }

    }
}
