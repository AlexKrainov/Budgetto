using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    /// <summary>
    /// SummaryType == Summary.ID
    /// </summary>
    public enum SummaryType
    {
        Undifined = 0,
        EarningsPerHour,
        ExpensesPerDay,//Spendings per day
        CashFlow,
        AllAccountsMoney,
        AllSubScriptionPrice
    }

    public class Summary
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        [Required]
        [MaxLength(64)]
        public string CodeName { get; set; }
        public DateTime CurrentDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsChart { get; set; }

        [ForeignKey("VisibleElement")]
        public int VisibleElementID { get; set; }

        public virtual VisibleElement VisibleElement { get; set; }

        public virtual ICollection<UserSummary> UserSummaries { get; set; }

        public Summary()
        {
            this.UserSummaries = new HashSet<UserSummary>();
        }

    }
}
