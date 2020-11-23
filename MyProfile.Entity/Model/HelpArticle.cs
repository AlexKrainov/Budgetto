using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class HelpArticle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [MaxLength(512)]
        public string Title { get; set; }
        public bool IsVisible { get; set; } = true;
        [Required]
        public string KeyWords { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateEdit { get; set; }
        [Required]
        [MaxLength(512)]
        public string Link { get; set; }

        [ForeignKey("User")]
        public Guid? OwnerID { get; set; }
        [ForeignKey("HelpMenu")]
        public int HelpMenuID { get; set; }

        public virtual User User { get; set; }
        public virtual HelpMenu HelpMenu { get; set; }

        public virtual ICollection<HelpArticleUserView> HelpArticleUserViews { get; set; }

        public HelpArticle()
        {
            this.HelpArticleUserViews = new HashSet<HelpArticleUserView>();
        }
    }
}
