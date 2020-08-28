﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class UserLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public DateTime CurrentDateTime { get; set; }
        [StringLength(64)]
        public string ActionCodeName { get; set; }
        public string Comment { get; set; }

        [ForeignKey("UserSession")]
        public Guid UserSessionID { get; set; }


        public virtual UserSession UserSession { get; set; }

    }
}
