using MyProfile.Entity.ModelView.AreaAndSection;
using MyProfile.Entity.ModelView.Currency;
using System.Collections.Generic;
using System.Globalization;

namespace MyProfile.Entity.ModelView.Limit
{
    public class LimitChartModelView
    {
        public string Name { get; set; }
        public List<SectionLightModelView> Sections { get; set; } = new List<SectionLightModelView>();
        public decimal LimitMoney { get; set; }
        public string LimitMoneyString
        {
            get
            {
                return LimitMoney.ToString("C0", CultureInfo.CreateSpecificCulture(Currency.specificCulture));
            }
        }
        public decimal SpendedMoney { get; set; }
        public string SpendedMoneyString
        {
            get
            {
                return SpendedMoney.ToString("C0", CultureInfo.CreateSpecificCulture(Currency.specificCulture));
            }
        }
        public decimal LeftMoneyInADay { get; set; }
        public string LeftMoneyInADayString
        {
            get
            {
                return LeftMoneyInADay.ToString("C0", CultureInfo.CreateSpecificCulture(Currency.specificCulture));
            }
        }
        public string ChartID { get; set; }
        public decimal Percent1 { get; set; }
        public decimal Percent2 { get; set; }
        /// <summary>
        /// Is DateTime.Now == current view date 
        /// </summary>
        public bool IsThis { get; set; }
        public bool IsPast { get; set; }
        public bool IsFuture { get; set; }
        public bool IsShow { get; set; }
        public int PeriodTypeID { get; set; }
        public string Text { get; set; }
        public long ID { get; set; }
        public CurrencyClientModelView _currency { get; set; }
        public CurrencyClientModelView Currency
        {
            get
            {
                if (_currency != null)
                {
                    return _currency;
                }
                else
                {
                    return new CurrencyClientModelView
                    {
                        id = 1,
                        codeName = "RUB",
                        specificCulture = "ru-RU",
                        icon = "₽",
                    };
                }
            }
            set
            {
                _currency = value;
            }
        }
    }
}
