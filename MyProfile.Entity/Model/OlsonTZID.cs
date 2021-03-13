using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class OlsonTZID
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[MaxLength(128)]
        public string Name { get; set; }

        [ForeignKey("TimeZone")]
		public int TimeZoneID { get; set; }
		
		public virtual MyTimeZone TimeZone { get; set; }
		
	}
}
