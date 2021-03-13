using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class MyTimeZone
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[MaxLength(64)]
        public string WindowsTimezoneID { get; set; }
		[MaxLength(256)]
		public string WindowsDisplayName { get; set; }
        public decimal UTCOffsetHours { get; set; }
        public int UTCOffsetMinutes { get; set; }
        public bool IsDST { get; set; }
		[MaxLength(8)]
		public string Abreviatura { get; set; }

		public virtual IEnumerable<OlsonTZID> OlsonTZIDs { get; set; }

		public MyTimeZone()
		{
			this.OlsonTZIDs = new HashSet<OlsonTZID>();
		}

	}
}
