using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyProfile.Entity.Model
{
    public class SiteSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// this is property with emails like gmail@gmail.com ,test@ya.ru 
        /// it's need if user send Feedback, we copy message for these emails
        /// if this property is empty, it means don't send
        /// </summary>
        public string EmailsForFeedback { get; set; }
    }
}
