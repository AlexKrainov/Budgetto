#define PROD

namespace Common.Service
{
    public class PublishSettings
    {
#if PROD
        public const string ConnectionString = "PROD_Connection";
        public const string SiteName = "app.budgetto.org";
        public const bool IsOnlyProdTask = true;
        public const bool IsLocalServer = false;//For menu
        public const string TelegramApi = "1661757766:AAESOVnmDrT2ZKjZ7NUZG7R8wnw3Amm0c_w";//Budgetto_bot
#else
        //public const string ConnectionString = "TestRegRuConnection";
        public const string ConnectionString = "TestConnection";
        public const string SiteName = "testmybudget.ru";
        public const bool IsOnlyProdTask = false;
        public const bool IsLocalServer = true;//For menu
        //public const string TelegramApi = "1661757766:AAESOVnmDrT2ZKjZ7NUZG7R8wnw3Amm0c_w";//Budgetto_bot
        public const string TelegramApi = "1542224035:AAFVAAoC1AV5k0eEKsozrBBrjKhgxlRLJMk";//MyProjectTest_bot
#endif

    }
}
