using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    /// <summary>
    /// Use this object for error on the site
    /// </summary>
    public class UserErrorLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }


        [ForeignKey("ErrorLog")]
        public int? ErrorLogID { get; set; }
        [ForeignKey("UserLog")]
        public long? UserLogID { get; set; }


        public virtual ErrorLog ErrorLog { get; set; }
        public virtual UserLog UserLog { get; set; }


    }
}
