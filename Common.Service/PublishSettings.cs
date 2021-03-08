#define _PROD

namespace Common.Service
{
    public class PublishSettings
    {
#if PROD
        public const string ConnectionString = "PROD_Connection";
        public const string SiteName = "app.budgetto.org";
        public const bool IsOnlyProdTask = true;
#else
        //public const string ConnectionString = "TestRegRuConnection";
        public const string ConnectionString = "TestConnection";
        public const string SiteName = "testmybudget.ru";
        public const bool IsOnlyProdTask = false;
#endif
        public const string TelegramApi = "1661757766:AAESOVnmDrT2ZKjZ7NUZG7R8wnw3Amm0c_w";

    }
}
