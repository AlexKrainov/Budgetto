using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class AccountHistoryActionType
    {
		public static readonly string Create = "Create";
		public static readonly string Edit = "Edit";
		public static readonly string ShowHide = "ShowHide";
		public static readonly string Delete = "Delete";
		public static readonly string Recovery = "Recovery";
	}
	public class AccountHistory
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[MaxLength(16)]
		public string ActionType { get; set; }
        public string OldAccountStateJson { get; set; }
        public string NewAccountStateJson { get; set; }
        public DateTime CurrentDate { get; set; }

		[ForeignKey("AccountID")]
		public int AccountID { get; set; }

		public virtual Account Account { get; set; }

	}
}
