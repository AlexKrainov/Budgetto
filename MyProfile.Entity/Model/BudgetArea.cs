using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class BudgetArea
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }
        /// <summary>
        /// Hide but not delete
        /// </summary>
        public bool IsShowOnSite { get; set; } = true;
        public string Description { get; set; }
        /// <summary>
        /// Can see only owner or all budget group
        /// </summary>
        public bool IsShowInCollective { get; set; }
        /// <summary>
        /// Created by constructor after registration
        /// </summary>
        public bool IsCreatedByConstructor { get; set; }

        [ForeignKey("User")]
        public Guid? UserID { get; set; }
        [ForeignKey("BaseArea")]
        public int? BaseAreaID { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<BudgetSection> BudgetSectinos { get; set; }
        public virtual BaseArea BaseArea { get; set; }

        public BudgetArea()
        {
            this.BudgetSectinos = new HashSet<BudgetSection>();
        }
    }
}
