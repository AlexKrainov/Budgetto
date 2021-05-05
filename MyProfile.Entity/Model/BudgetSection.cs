using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class BudgetSection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// It needs for isolate the section created by the system
        /// </summary>
        public string CodeName { get; set; }
        public string Description { get; set; }
        [MaxLength(64)]
        public string CssIcon { get; set; }
        [MaxLength(24)]
        public string CssColor { get; set; }
        [MaxLength(24)]
        public string CssBackground { get; set; }
        [MaxLength(24)]
        public string CssBorder { get; set; }
        /// <summary>
        /// Hide in record add
        /// </summary>
        public bool IsShowOnSite { get; set; } = true;
        /// <summary>
        /// Can see only owner
        /// </summary>
        public bool IsShowInCollective { get; set; }
        /// <summary>
        /// Created by constructor after registration
        /// </summary>
        public bool IsCreatedByConstructor { get; set; }
        /// <summary>
        /// Take into account cashback
        /// </summary>
        public bool IsCashback { get; set; }

        [ForeignKey("BudgetArea")]
        public int BudgetAreaID { get; set; }
        [ForeignKey("SectionType")]
        public int? SectionTypeID { get; set; }


        public virtual BudgetArea BudgetArea { get; set; }
        public virtual SectionType SectionType { get; set; }


        public virtual IEnumerable<Record> BudgetRecords { get; set; }
        public virtual IEnumerable<CollectiveSection> CollectiveSections { get; set; }
        public virtual IEnumerable<SectionGroupLimit> SectionGroupLimits { get; set; }
        public virtual IEnumerable<TemplateBudgetSection> TemplateBudgetSections { get; set; }

        public BudgetSection()
        {
            this.SectionGroupLimits = new HashSet<SectionGroupLimit>();
            this.BudgetRecords = new HashSet<Record>();
            this.CollectiveSections = new HashSet<CollectiveSection>();
            this.TemplateBudgetSections = new HashSet<TemplateBudgetSection>();
        }
    }
}
