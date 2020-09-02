using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class HelpArticleUserView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public DateTime DateView { get; set; }

        [ForeignKey("HelpArticle")]
        public int HelpArticleID { get; set; }
        [ForeignKey("User")]
        public Guid? UserID { get; set; }

        public virtual User User { get; set; }
        public virtual HelpArticle HelpArticle { get; set; }
    }
}
