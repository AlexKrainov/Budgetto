using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class ResourceMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [ForeignKey("Resource")]
        public int ResourceID { get; set; }
        [ForeignKey("Message")]
        public int MessageID { get; set; }

        public virtual Resource Resource { get; set; }
        public virtual Message Message { get; set; }

    }
}
