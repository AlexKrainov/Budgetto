using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class ResourceMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [ForeignKey("Resource")]
        public long ResourceID { get; set; }
        [ForeignKey("Message")]
        public long MessageID { get; set; }

        public virtual Resource Resource { get; set; }
        public virtual Message Message { get; set; }

    }
}
