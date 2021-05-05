using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class HubConnect
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public DateTime DateConnect { get; set; }
        [StringLength(128)]
        public string ConnectionID { get; set; }

        [ForeignKey("UserConnect")]
        public Guid UserConnectID { get; set; }


        public virtual UserConnect UserConnect { get; set; }
        
    }
}
