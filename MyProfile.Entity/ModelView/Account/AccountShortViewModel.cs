using MyProfile.Entity.Model;

namespace MyProfile.Entity.ModelView.Account
{
    public class AccountShortViewModel
	{
        public int ID { get; set; }
        public AccountTypesEnum AccountType { get; set; }
		public string Name { get; set; }
		public bool IsDefault { get; set; }

		public int? CurrencyID { get; set; }
		public int? BankID { get; set; }
        public string BankImage { get; set; }
        public string BankName { get; set; }
        public string CurrencySpecificCulture { get; set; }
        public string CurrencyCodeName { get; set; }
        public string AccountIcon { get; set; }
        public bool IsDeleted { get; set; }
    }
}
