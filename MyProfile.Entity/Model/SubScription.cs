using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class SubScription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; }
        public string Description { get; set; }
        [MaxLength(512)]
        public string Site { get; set; }
        [MaxLength(512)]
        public string LogoBig { get; set; }
        [MaxLength(512)]
        public string LogoSmall { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("SubScriptionCategory")]
        public int SubScriptionCategoryID { get; set; }
        [ForeignKey("Parent")]
        public int? ParentID { get; set; }

        public virtual SubScriptionCategory SubScriptionCategory { get; set; }
        public virtual SubScription Parent { get; set; }

        public virtual ICollection<SubScriptionOption> SubScriptionOptions { get; set; }

        public SubScription()
        {
            this.SubScriptionOptions = new HashSet<SubScriptionOption>();
        }
    }
}
