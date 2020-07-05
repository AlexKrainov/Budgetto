namespace MyProfile.Entity.ModelView.Currency
{
    public class CurrencyClientModelView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string CodeName { get; set; }
        public string SpecificCulture { get; set; }
        public string Icon { get; set; }
        public string CodeName_CBR { get; set; }
        public int? CodeNumber_CBR { get; set; }
        public string CBR_Link { get; set; }
    }
}
