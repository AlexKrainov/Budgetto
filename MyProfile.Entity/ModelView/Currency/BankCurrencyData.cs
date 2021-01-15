using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Currency
{
    public class BankCurrencyData
    {
        public int CurrencyID { get; set; }
        public DateTime Date { get; set; }

        public decimal? Rate { get; set; }
        public int Nominal { get; set; }
        public string NumCode { get; set; }
        public string CharCode { get; set; }
        public string Name { get; set; }
        public string CodeName_CBR { get; set; }
    }
}
