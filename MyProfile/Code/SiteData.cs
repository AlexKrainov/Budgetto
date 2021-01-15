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
                CommonService service = (CommonService)serviceScope.ServiceProvider.GetService(typeof(CommonService));

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
