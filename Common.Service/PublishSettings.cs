#define _PROD

namespace Common.Service
{
    public class PublishSettings
    {
#if PROD
        public readonly string ConnectionString = "PROD_Connection";
        public readonly string SiteName = "app.budgetto.org";
#else
        public readonly static string ConnectionString = "TestRegRuConnection";
        public readonly static string SiteName = "testmybudget.ru";

#endif

    }
}
