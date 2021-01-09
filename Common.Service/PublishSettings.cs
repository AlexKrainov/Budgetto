﻿#define _PROD

namespace Common.Service
{
    public class PublishSettings
    {
#if PROD
        public const string ConnectionString = "PROD_Connection";
        public const string SiteName = "app.budgetto.org";
#else
       // public const string ConnectionString = "TestRegRuConnection";
        public const string ConnectionString = "TestConnection";
        public const string SiteName = "testmybudget.ru";
#endif

    }
}
