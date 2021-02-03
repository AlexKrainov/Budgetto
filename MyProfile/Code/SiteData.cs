using Common.Service;
using MyProfile.Entity.ModelView.Currency;
using MyProfile.Identity;
using System.Collections.Generic;

namespace MyProfile.Code
{
    public class SiteData
    {

        public Metadata GetMetadata()
        {
            Metadata metadata = new Metadata();

            using (var serviceScope = ServiceActivator.GetScope())
            {
                CurrencyService service = (CurrencyService)serviceScope.ServiceProvider.GetService(typeof(CurrencyService));

                metadata.currencies = service.GetCurrencyInfo();
            }

            return metadata;
        }
    }

    public class Metadata
    {
        public List<CurrencyClientModelView> currencies = new List<CurrencyClientModelView>();
    }
}
