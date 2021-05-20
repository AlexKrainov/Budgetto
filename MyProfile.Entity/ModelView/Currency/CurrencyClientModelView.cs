namespace MyProfile.Entity.ModelView.Currency
{
    public class CurrencyClientModelView
    {
        public int id { get; set; }
        public string name { get; set; }
        public string codeName { get; set; }
        public string specificCulture { get; set; }
        public string icon { get; set; }
        public string codeName_CBR { get; set; }
        public int? codeNumber_CBR { get; set; }
    }

    public class CurrencyLightModelView
    {
        public int? ID { get; set; }
        public int Nominal { get; set; }
        public decimal? Rate { get; set; }
        public string SpecificCulture { get; set; }
        public string CodeName { get; set; }
    }
}
