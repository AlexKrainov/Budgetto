namespace MyProfile.Entity.ModelView.Account
{
    public class TransferMoney
    {
        public int AccountFromID { get; set; }
        public int AccountToID { get; set; }
        public decimal Value { get; set; }
        public decimal CurrencyValue { get; set; }
        public decimal EndValue { get; set; }
        public string Comment { get; set; }
    }
}
