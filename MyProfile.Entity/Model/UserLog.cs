using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class UserLog
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[StringLength(64)]
		public string IP { get; set; }
		[StringLength(32)]
		public string City { get; set; }
		[StringLength(32)]
		public string Country { get; set; }
		[StringLength(64)]
		public string Location { get; set; }
		[StringLength(16)]
		public string PostCode { get; set; }
		[StringLength(32)]
		public string BrowerName { get; set; }
		[StringLength(16)]
		public string BrowserVersion { get; set; }
		[StringLength(32)]
		public string OS_Name { get; set; }
		[StringLength(16)]
		public string Os_Version { get; set; }
		[StringLength(16)]
		public string ScreenSize { get; set; }
		public DateTime CurrentDateTime { get; set; }
		[StringLength(16)]
		public string ActionCodeName { get; set; }
		public string Comment { get; set; }
		[StringLength(32)]
		public string SessionID { get; set; }


		[ForeignKey("ParentUserLog")]
		public int? ParentUserLogID { get; set; }
		[ForeignKey("User")]
		public Guid? UserID { get; set; }


		public virtual User User { get; set; }
		public virtual UserLog ParentUserLog { get; set; }
	}
}
