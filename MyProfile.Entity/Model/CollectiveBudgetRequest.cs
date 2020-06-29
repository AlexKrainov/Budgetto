using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public enum RequestStatusType
    {
        Awaiting,// the CollectiveBudgetRequest.UserID has not yet accepted or rejected
        Accepted,
        Rejected,//it means an user clicked button "Would you like use collective budget ? - No"
    }
    public class CollectiveBudgetRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [MaxLength(16)]
        public string Status { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateUpdate { get; set; }
        public bool IsDeleted { get; set; }
        /// <summary>
        /// whom ask
        /// </summary>
        [ForeignKey("User")]
        public Guid? UserID { get; set; }
        /// <summary>
        /// who ask
        /// </summary>
        [ForeignKey("CollectiveBudgetRequestOwner")]
        public int CollectiveBudgetRequestOwnerID { get; set; }
        [ForeignKey("CollectiveBudget")]
        public Guid CollectiveBudgetID { get; set; }

        public virtual User User { get; set; }
        public virtual CollectiveBudgetRequestOwner CollectiveBudgetRequestOwner { get; set; }
        public virtual CollectiveBudget CollectiveBudget { get; set; }


    }

}
