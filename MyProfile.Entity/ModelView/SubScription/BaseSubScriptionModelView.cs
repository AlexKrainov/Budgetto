using System.Collections.Generic;
using System.Globalization;

namespace MyProfile.Entity.ModelView.SubScription
{
    public class BaseSubScriptionModelView
    {
        public string LogoBig { get; set; }
        public string Site { get; set; }
        public string CategoryName { get; set; }
        public string Title { get; set; }
        public List<SubScriptionOptionModelView> Options { get; set; }
        public string DiapasonPrice { get; set; }
        public int ID { get; set; }

        public string UserTitle { get; set; }
        public decimal UserPrice { get; set; }
        public decimal UserPriceForMonth { get; set; }
        public int UserPricingID { get; set; } = -1;
        public int UserOptionID { get; set; }
        public int UserSubScriptionID { get; set; }
        public int UserPricingPeriodTypeID { get; set; }
        public string UserPriceString
        {
            get
            {
                return UserPrice.ToString("C0", CultureInfo.CreateSpecificCulture("ru-RU"));
            }
        }
        public string UserPriceForMonthString
        {
            get
            {
                return UserPriceForMonth.ToString("C0", CultureInfo.CreateSpecificCulture("ru-RU"));
            }
        }
        public string UserPricingPeriodTypeString
        {
            get
            {
                if (UserPricingPeriodTypeID == 1 || UserPricingPeriodTypeID == 21 || UserPricingPeriodTypeID == 31)
                {
                    return UserPricingPeriodTypeID + " месяц";
                }
                else if (UserPricingPeriodTypeID > 1 && UserPricingPeriodTypeID < 5 || UserPricingPeriodTypeID > 21 && UserPricingPeriodTypeID < 25 || UserPricingPeriodTypeID > 31 && UserPricingPeriodTypeID < 35)
                {
                    return UserPricingPeriodTypeID + " месяца";
                }
                else if (UserPricingPeriodTypeID > 4 && UserPricingPeriodTypeID < 21 || UserPricingPeriodTypeID > 24 && UserPricingPeriodTypeID < 31 || UserPricingPeriodTypeID > 34 && UserPricingPeriodTypeID < 41)
                {
                    return UserPricingPeriodTypeID + " месяцев";
                }
                return UserPricingPeriodTypeID.ToString();
            }
        }
        public bool IsDeleted { get; set; }
        public bool IsShow { get; set; } = true;
    }

    public class SubScriptionOptionModelView
    {
        public List<SubScriptionPricingModelView> Pricings { get; set; }
        public bool IsBoth { get; set; }
        public bool IsFamaly { get; set; }
        public bool IsPersonally { get; set; }
        public bool IsStudent { get; set; }
        public string Title { get; set; }
        public bool IsSelected { get; set; }
        public string DiapasonPrice { get; set; }
        public int ID { get; set; }

        public string OptionVariant
        {
            get
            {
                if (IsPersonally)
                {
                    return "Персональная подписка";
                }
                else if (IsBoth)
                {
                    return "Подписка для двоих";
                }
                else if (IsFamaly)
                {
                    return "Семейная подписка";
                }
                else if (IsStudent)
                {
                    return "Студенческая подписка";
                }
                return "";
            }
        }
    }

    public class SubScriptionPricingModelView
    {
        public int ID { get; set; }
        public bool IsSelected { get; set; }
        public decimal Price { get; set; }
        public decimal PricePerMonth { get; set; }
        public int PeriodTypeID { get; set; }

        public string PriceString
        {
            get
            {
                return Price.ToString("C0", CultureInfo.CreateSpecificCulture("ru-RU"));
            }
        }
        public string PricePerMonthString
        {
            get
            {
                return PricePerMonth.ToString("C0", CultureInfo.CreateSpecificCulture("ru-RU"));
            }
        }

        public string PeriodString
        {
            get
            {
                if (PeriodTypeID == 1 || PeriodTypeID == 21 || PeriodTypeID == 31)
                {
                    return PeriodTypeID + " месяц";
                }
                else if (PeriodTypeID > 1 && PeriodTypeID < 5 || PeriodTypeID > 21 && PeriodTypeID < 25 || PeriodTypeID > 31 && PeriodTypeID < 35)
                {
                    return PeriodTypeID + " месяца";
                }
                else if (PeriodTypeID > 4 && PeriodTypeID < 21 || PeriodTypeID > 24 && PeriodTypeID < 31 || PeriodTypeID > 34 && PeriodTypeID < 41)
                {
                    return PeriodTypeID + " месяцев";
                }
                return PeriodTypeID.ToString();
            }
        }
    }
}
