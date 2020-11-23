using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class HelpMenu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [MaxLength(32)]
        public string Title { get; set; }
        [Required]
        [MaxLength(32)]
        public string Icon { get; set; }
        public bool IsVisible { get; set; } = true;
        public int Order { get; set; }

        public virtual ICollection<HelpArticle> HelpArticles { get; set; }

        public HelpMenu()
        {
            this.HelpArticles= new HashSet<HelpArticle>();
        }
    }
}
