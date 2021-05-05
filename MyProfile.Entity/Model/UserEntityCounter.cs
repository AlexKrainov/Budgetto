using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class UserEntityCounter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public int AddedCount { get; set; }
        public DateTime LastChanges { get; set; }

        [ForeignKey("User")]
        public Guid UserID { get; set; }
        [ForeignKey("EntityType")]
        public int EntityTypeID { get; set; }

        public virtual EntityType EntityType { get; set; }
        public virtual User User { get; set; }

    }
}
