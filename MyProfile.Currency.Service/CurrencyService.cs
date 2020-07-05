using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.Currency;
using MyProfile.Entity.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Currency.Service
{
    public class CurrencyService
    {
        private IBaseRepository repository;

        public CurrencyService(IBaseRepository repository)
        {
            this.repository = repository;
        }

        [Obsolete("ToDo will take this list from cache")]
        public async Task<object> GetCurrencyInfoForClient()
        {
            var currencies = await repository.GetAll<Entity.Model.Currency>()
                 .Select(x => new CurrencyClientModelView
                 {
                     CodeName = x.CodeName,
                     CodeName_CBR = x.CodeName_CBR,
                     CodeNumber_CBR = x.CodeNumber_CBR,
                     Icon = x.Icon,
                     ID = x.ID,
                     Name = x.Name,
                     SpecificCulture = x.SpecificCulture,
                     CBR_Link = "http://www.cbr.ru/scripts/XML_daily.asp?date_req=02/03/2002"
                 })
                 .ToListAsync();

            return currencies;
        }
    }
}
