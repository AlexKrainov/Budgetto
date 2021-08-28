rUSE [u1127477_MyProfile]
GO
SET IDENTITY_INSERT [dbo].[UserTypes] ON 

INSERT [dbo].[UserTypes] ([ID], [CodeName]) VALUES (1, N'User')
INSERT [dbo].[UserTypes] ([ID], [CodeName]) VALUES (2, N'Admin')
INSERT [dbo].[UserTypes] ([ID], [CodeName]) VALUES (3, N'Tester')
INSERT [dbo].[UserTypes] ([ID], [CodeName]) VALUES (4, N'TelegramBot')
SET IDENTITY_INSERT [dbo].[UserTypes] OFF
GO

SET IDENTITY_INSERT [dbo].[ChartTypes] ON 

INSERT [dbo].[ChartTypes] ([ID], [Name], [CodeName], [IsUsing], [IsBig]) VALUES (1, N'Линейный график', N'line', 1, 1)
INSERT [dbo].[ChartTypes] ([ID], [Name], [CodeName], [IsUsing], [IsBig]) VALUES (2, N'Столбчатый график', N'bar', 1, 0)
INSERT [dbo].[ChartTypes] ([ID], [Name], [CodeName], [IsUsing], [IsBig]) VALUES (3, N'Круговая диаграмма', N'pie', 1, 0)
INSERT [dbo].[ChartTypes] ([ID], [Name], [CodeName], [IsUsing], [IsBig]) VALUES (4, N'Кольцевая диаграмма', N'doughnut', 1, 0)
INSERT [dbo].[ChartTypes] ([ID], [Name], [CodeName], [IsUsing], [IsBig]) VALUES (5, N'Линейный график', N'bubble', 1, 0)
INSERT [dbo].[ChartTypes] ([ID], [Name], [CodeName], [IsUsing], [IsB	ig]) VALUES (6, N'Столбчатый график', N'bar', 1, 1)
SET IDENTITY_INSERT [dbo].[ChartTypes] OFF
GO

SET IDENTITY_INSERT [dbo].[Currencies] ON 
--https://dotnetfiddle.net/ytW9rB
INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (1, N'Russian Ruble', N'RUB', N'ru-RU', N'₽', 1, NULL, NULL)
INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (2, N'Euro', N'EUR', N'de-DE', N'€', 0, N'R01239', 978)
INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (4, N'United States Dollar', N'USD', N'en-US', N'$', 0, N'R01235', 840)
INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (5, N'United Kingdom Pound', N'GBP', N'en-GB', N'£', 0, N'R01035', 826)
INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (6, N'Австралийский доллар', N'AUD', N'en-AU', N'$', 0, N'R01010', 036)
INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (7, N'Южноафриканских рэндов', N'ZAR', N'af-ZA', N'R', 0, N'R01810', 710)
INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (8, N'Швейцарский франк', N'CHF', N'de-CH', N'₣', 0, N'R01775', 756)
INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (9, N'Шведских крон', N'SEK', N'se-SE', N'kr', 0, N'R01770', 752)
INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (10, N'Чешских крон', N'CZK', N'cs-CZ', N'Kč', 0, N'R01760', 203)
INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (11, N'Украинских гривен', N'UAH', N'uk-UA', N'₴', 0, N'R01720', 980)
INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (12, N'Узбекских сумов', N'UZS', N'uz-Cyrl-UZ', N'So’m', 0, N'R01717', 860)
INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (13, N'Туркменский манат', N'TMT', N'tk-TM', N'T', 0, N'R01710A', 934)
INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (14, N'Турецких лир', N'TRY', N'tr-TR', N'₤', 0, N'R01700J', 949)
INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (15, N'Таджикских сомони', N'TJS', N'tg-Cyrl-TJ', N'SM', 0, N'R01670', 972)


SET IDENTITY_INSERT [dbo].[Currencies] OFF
GO

SET IDENTITY_INSERT [MailTypes] ON 

INSERT [MailTypes] ([ID], [CodeName]) VALUES (1, N'Registration')
INSERT [MailTypes] ([ID], [CodeName]) VALUES (2, N'EmailUpdate')
INSERT [MailTypes] ([ID], [CodeName]) VALUES (3, N'PasswordReset')
INSERT [MailTypes] ([ID], [CodeName]) VALUES (4, N'LoginLimitEnter')
INSERT [MailTypes] ([ID], [CodeName]) VALUES (5, N'ResendByUser')
INSERT [MailTypes] ([ID], [CodeName]) VALUES (6, N'NotificationLimit')
INSERT [MailTypes] ([ID], [CodeName]) VALUES (7, N'NotificationReminder')
INSERT [MailTypes] ([ID], [CodeName]) VALUES (8, N'NotificationSystemMailing')
SET IDENTITY_INSERT [MailTypes] OFF
GO

SET IDENTITY_INSERT [dbo].[PeriodTypes] ON 

INSERT [dbo].[PeriodTypes] ([ID], [Name], [CodeName], [IsUsing]) VALUES (1, N'Финансы на месяц', N'Days', 1)
INSERT [dbo].[PeriodTypes] ([ID], [Name], [CodeName], [IsUsing]) VALUES (2, N'Weeks for month', N'Week', 0)
INSERT [dbo].[PeriodTypes] ([ID], [Name], [CodeName], [IsUsing]) VALUES (3, N'Финансы на год', N'Month', 1)
INSERT [dbo].[PeriodTypes] ([ID], [Name], [CodeName], [IsUsing]) VALUES (4, N'Years for 10 Year', N'10_Year', 0)
SET IDENTITY_INSERT [dbo].[PeriodTypes] OFF
GO

SET IDENTITY_INSERT [dbo].[SectionTypes] ON 

INSERT [dbo].[SectionTypes] ([ID], [Name], [CodeName]) VALUES (1, N'Доходы', N'Earnings')
INSERT [dbo].[SectionTypes] ([ID], [Name], [CodeName]) VALUES (2, N'Расходы', N'Spendings')
SET IDENTITY_INSERT [dbo].[SectionTypes] OFF
GO



SET IDENTITY_INSERT [HelpMenus] ON 
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (2, 'Финансы на месяц/год' ,'pe-7s-display1' ,1 ,3)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (3, 'Лимиты' ,'lnr lnr-frame-expand' ,1 ,4)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (4, 'Цели' ,'lnr lnr-rocket' ,1 ,5)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (5, 'Графики' ,'lnr lnr-pie-chart' ,1 ,6)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (6, 'Шаблоны' ,'lnr lnr-layers' ,1 ,7)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (7, 'Категории и группы' ,'pe-7s-albums' ,1 ,2)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (8, 'Календарь' ,'lnr lnr-calendar-full' ,1 ,8)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (9, 'Общие вопросы' ,'lnr lnr-question-circle' ,1 ,0)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (10, 'Записи' ,'oi oi-dollar' ,1 ,1)
SET IDENTITY_INSERT [HelpMenus] OFF

GO

SET IDENTITY_INSERT [HelpArticles] ON 
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (1, 'Создание лимита' ,'2020-11-24 21:40:32.8652966', '2020-11-23 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/AddLimit.cshtml' ,3, 'создание лимита; лимит')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (2, 'Создание цели' ,'2020-11-25 21:40:32.8652966', '2020-11-25 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/AddGoal.cshtml' ,4, 'создание цели; цель; цели')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (3, 'Создание шаблона' ,'2020-11-26 21:40:32.8652966', '2020-11-26 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/AddTemplate.cshtml' ,6, 'создание шаблона; шаблоны; шаблон')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (4, 'Создание графика' ,'2020-11-26 21:40:32.8652966', '2020-11-26 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/AddChart.cshtml' ,5, 'создание графика; графики; график; диаграммы; диаграмма')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (5, 'Создание группы категорий' ,'2020-11-26 21:40:32.8652966', '2020-11-26 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/AddFolder.cshtml' ,7, 'создание; создание групп; групп; категори; ')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (6, 'Создание категории' ,'2020-12-01 21:40:32.8652966', '2020-12-01 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/AddSection.cshtml' ,7, 'создание категории; категории; категория; ')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (7, 'Смена группы у категории' ,'2020-12-01 21:40:32.8652966', '2020-12-01 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/ChangeFolder.cshtml' ,7, 'смена групп, поменять групп; групп; категори;')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (8, 'Создание напоминания' ,'2020-12-02 21:40:32.8652966', '2020-12-02 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/AddReminder.cshtml' ,2, 'создать напоминание, добавить напоминание; напоминани; ')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (9, 'Добавление записи' ,'2020-12-02 21:40:32.8652966', '2020-12-02 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/AddRecord.cshtml' ,10, 'создать записи, добавление записей; записи; запись; добавить расход; добавить доход; доход; расход; инвестиции; ')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (10, 'Добавление записи в валюте' ,'2020-12-02 21:40:32.8652966', '2020-12-02 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/AddCurrencyRecord.cshtml' ,10, 'создать записи, добавление записей; записи; запись; добавить расход; добавить доход; доход; расход; инвестиции; валюта; доллар; евро; ')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (11, 'Работа с таблицей' ,'2020-12-03 21:40:32.8652966', '2020-12-03 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/Table.cshtml' ,2, 'создать записи, добавление записей; записи; запись; доход; расход; инвестиции; история; найти; напоминани; таблиц ')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (12, 'Дополнительные настройки страницы' ,'2020-12-04 21:40:32.8652966', '2020-12-04 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/PageSettings.cshtml' ,9, 'виджет;настройки;скрыть;показать;завершен;')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (13, 'Поиск записей' ,'2020-12-04 21:40:32.8652966', '2020-12-04 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/SearchRecord.cshtml' ,10, 'поиск;записи;трат;доход;расход;история;')

SET IDENTITY_INSERT [HelpArticles] OFF
GO

SET IDENTITY_INSERT [AccountTypes] ON 

INSERT [AccountTypes] ([ID], [Name], [CodeName], [Icon], [IsVisible], [IsPaymentSystem], [BankTypeID]) VALUES (1, N'Наличные', N'Cash', 'pe-7s-cash', 1,0, null)
INSERT [AccountTypes] ([ID], [Name], [CodeName], [Icon], [IsVisible], [IsPaymentSystem], [BankTypeID]) VALUES (2, N'Дебетовый счет', N'Debed', 'pe-7s-credit',1,1,1)
INSERT [AccountTypes] ([ID], [Name], [CodeName], [Icon], [IsVisible], [IsPaymentSystem], [BankTypeID]) VALUES (3, N'Кредитный счет', N'Credit', 'pe-7s-credit',0,1,1)
INSERT [AccountTypes] ([ID], [Name], [CodeName], [Icon], [IsVisible], [IsPaymentSystem], [BankTypeID]) VALUES (4, N'Карта рассрочки', N'Installment', 'pe-7s-credit',0,1,1)
INSERT [AccountTypes] ([ID], [Name], [CodeName], [Icon], [IsVisible], [IsPaymentSystem], [BankTypeID]) VALUES (5, N'Электронный кошелек', N'OnlineWallet', 'pe-7s-wallet',0,1, 3)
INSERT [AccountTypes] ([ID], [Name], [CodeName], [Icon], [IsVisible], [IsPaymentSystem], [BankTypeID]) VALUES (6, N'Брокерский счет', N'Investments', 'pe-7s-culture',1,0,2)
INSERT [AccountTypes] ([ID], [Name], [CodeName], [Icon], [IsVisible], [IsPaymentSystem], [BankTypeID]) VALUES (7, N'Вклад', N'Deposit', 'pe-7s-credit',0,0,1)
INSERT [AccountTypes] ([ID], [Name], [CodeName], [Icon], [IsVisible], [IsPaymentSystem], [BankTypeID]) VALUES (8, N'Брокерский счет (ИИС)', N'InvestmentsIIS', 'pe-7s-culture',1,0,2)
SET IDENTITY_INSERT [AccountTypes] OFF
GO
 update AccountTypes set IsVisible = 1 where ID = 6

SET IDENTITY_INSERT [BankTypes] ON 

INSERT [BankTypes] ([ID], [Name], [CodeName], [IsVisible]) VALUES (1, N'Банк', N'Bank', 1)
INSERT [BankTypes] ([ID], [Name], [CodeName], [IsVisible]) VALUES (2, N'Брокер', N'Broker', 1)
INSERT [BankTypes] ([ID], [Name], [CodeName], [IsVisible]) VALUES (3, N'Электронный кошелек', N'OnlineWallet', 0)
INSERT [BankTypes] ([ID], [Name], [CodeName], [IsVisible]) VALUES (4, N'Микрозаймы', N'Microloans', 0)
INSERT [BankTypes] ([ID], [Name], [CodeName], [IsVisible]) VALUES (5, N'Форех', N'Forex', 0)
SET IDENTITY_INSERT [BankTypes] OFF
GO

SET IDENTITY_INSERT [Banks] ON 
--debed
INSERT [Banks] ([ID], [Name], [LogoCircle], [BankTypeID]) VALUES (1, N'Сбер', '/resources/banks/sber_circle.svg',1)
INSERT [Banks] ([ID], [Name], [LogoCircle], [BankTypeID]) VALUES (2, N'ВТБ', '/resources/banks/vtb.svg',1)
INSERT [Banks] ([ID], [Name], [LogoCircle], [BankTypeID]) VALUES (3, N'Газпромбанк', '/resources/banks/gasprom.svg',1)
INSERT [Banks] ([ID], [Name], [LogoCircle], [BankTypeID]) VALUES (4, N'Альфа-Банк', '/resources/banks/alfa-logo.svg',1)
INSERT [Banks] ([ID], [Name], [LogoCircle], [BankTypeID]) VALUES (5, N'Банк Открытие', '/resources/banks/open.svg',1)
INSERT [Banks] ([ID], [Name], [LogoCircle], [BankTypeID]) VALUES (6, N'Тинькофф Банк', '/resources/banks/tinkoff-bank.png',1)
INSERT [Banks] ([ID], [Name], [LogoCircle], [BankTypeID]) VALUES (7, N'Национальный Клиринговый Центр', '/resources/banks/moex.svg',1)
INSERT [Banks] ([ID], [Name], [LogoCircle], [BankTypeID]) VALUES (8, N'Россельхозбанк', '/resources/banks/rosselhoz.jfif',1)
INSERT [Banks] ([ID], [Name], [LogoCircle], [BankTypeID]) VALUES (9, N'Московский Кредитный Банк', '/resources/banks/mkb.svg',1)
INSERT [Banks] ([ID], [Name], [LogoCircle], [BankTypeID]) VALUES (10, N'Совкомбанк', '/resources/banks/sovkombank.svg',1)
INSERT [Banks] ([ID], [Name], [LogoCircle], [BankTypeID]) VALUES (11, N'Росбанк', '/resources/banks/ros.svg',1)
INSERT [Banks] ([ID], [Name], [LogoCircle], [BankTypeID]) VALUES (12, N'Райффайзенбанк', '/resources/banks/rasf.svg',1)
INSERT [Banks] ([ID], [Name], [LogoCircle], [BankTypeID]) VALUES (13, N'Ситибанк', '/resources/banks/citi.svg',1)
--Investments									
INSERT [Banks] ([Name], [LogoCircle],[LogoRectangle], [BankTypeID], [BorderColor]) VALUES (N'БКС', '/resources/banks/bcs_circle.svg','/resources/banks/bcs_rectangle.svg',2, '#016ef2')
INSERT [Banks] ([Name], [LogoCircle],[LogoRectangle], [BankTypeID], [BorderColor]) VALUES (N'Тинькофф Инвестиции','/resources/banks/tinkoff_circle.svg', '/resources/banks/tinkoff_bank.svg',2, '#ffdd2d')
INSERT [Banks] ([Name], [LogoCircle],[LogoRectangle], [BankTypeID], [BorderColor]) VALUES (N'Интерактив брокер', '/resources/banks/ib_circle.png', '/resources/banks/ib_rectangle.png',2, '#DB1222')
INSERT [Banks] ([Name], [LogoCircle],[LogoRectangle], [BankTypeID], [BorderColor]) VALUES (N'Атон', null, '/resources/banks/aton_rectangle.png',2, '#d8003f')
INSERT [Banks] ([Name], [LogoCircle],[LogoRectangle], [BankTypeID], [BorderColor]) VALUES (N'Финам', '/resources/banks/finam_circle.png', '/resources/banks/finame_rectangle.png',2, '#FFCB3F')
INSERT [Banks] ([Name], [LogoCircle],[LogoRectangle], [BankTypeID], [BorderColor]) VALUES (N'Фридом Финанс', '/resources/banks/freedom_finance_circle.svg', '/resources/banks/freedom_finance_rectangle.png',2, '#00b32e')
INSERT [Banks] ([Name], [LogoCircle],[LogoRectangle], [BankTypeID], [BorderColor]) VALUES (N'Сбер', '/resources/banks/sber_circle.svg', '/resources/banks/sber_rectangle.svg', 2,'#21A038')
INSERT [Banks] ([Name], [LogoCircle],[LogoRectangle], [BankTypeID], [BorderColor]) VALUES (N'ВТБ', '/resources/banks/vtb_circle.svg', '/resources/banks/vtb_rectangle.svg',2, '#009fdf')
INSERT [Banks] ([Name], [LogoCircle],[LogoRectangle], [BankTypeID], [BorderColor]) VALUES (N'Газпромбанк', '/resources/banks/gasprom_circle.svg', '/resources/banks/gasprom_rectangle.svg',2, '#0d356c')
INSERT [Banks] ([Name], [LogoCircle],[LogoRectangle], [BankTypeID], [BorderColor]) VALUES (N'Банк Открытие', '/resources/banks/open_circle.svg', '/resources/banks/open_broker_rectangle.svg',2, '#09ccff')

INSERT [Banks] ([ID], [Name], [LogoCircle], [BankTypeID]) VALUES (2000, N'Другой брокер', '/resources/banks/bank.svg', 2)
											 
INSERT [Banks] ([ID], [Name], [LogoCircle], [BankTypeID]) VALUES (2500, N'Другой банк', '/resources/banks/bank.svg', 1)

SET IDENTITY_INSERT [Banks] OFF
GO

update Banks  set LogoRectangle = '/resources/banks/open_rectangle.svg', BrandColor = '#09ccff' where ID = 5
update Banks  set LogoRectangle = '/resources/banks/tinkoff_bank.svg', BrandColor = '#ffdd2d' where ID = 6
update Banks set LogoCircle = '/resources/banks/sber_circle.svg', LogoRectangle = '/resources/banks/sber_rectangle.svg', BrandColor = '#21A038' where ID = 1
update Banks  set LogoRectangle = '/resources/banks/alfa_bank_rectangle.png', BrandColor = '#ef3124' where ID = 4
update Banks  set LogoRectangle = '/resources/banks/vtb_rectangle.svg', LogoCircle = '/resources/banks/vtb_cicle.svg', BrandColor = '#09ccff' where ID = 2
update Banks  set LogoRectangle = '/resources/banks/gasprom_rectangle.svg', LogoCircle = '/resources/banks/gasprom_circle.svg', BrandColor = '#0d356c' where ID = 3
Update Banks set Name = N'Открытие' where Name like '%Банк Открытие%'

SET IDENTITY_INSERT [SchedulerTasks] ON

INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment])
VALUES (1,'AccountRemoveCachback', 'New', 'AccountRemoveCachback', '0 0 4 1 * ?', 'At 01:00:00am, on the 1st day, every month')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment])
VALUES (2,'SetDoneToReminderDates', 'New', 'SetDoneToReminderDates', '0 0 4 * * ?', 'Every day at 1am')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment])
VALUES (3,'CurrencyHistoryTask', 'New', 'CurrencyHistoryTask', '0 0 * ? * *', 'Every hour')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment])
VALUES (4,'ResetHubConnectTask', 'New', 'ResetHubConnectTask','0 0 4 * * ?', 'Every day at 4am')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment])
VALUES (5,'NotificationLimitCheckerTask', 'New', 'NotificationLimitCheckerTask','0 * * ? * *', 'Every minuts')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment])
VALUES (6,'NotificationSiteTask', 'New', 'NotificationSiteTask','0/17 * * ? * * *', 'Every 17 seconds')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment])
VALUES (7,'NotificationTelegramTask', 'New', 'NotificationTelegramTask','0 * * ? * *', 'Every minuts')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment])
VALUES (8,'NotificationMailTask', 'New', 'NotificationMailTask','0 * * ? * *', 'Every minuts')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment])
VALUES (9,'NotificationReminderCheckerTask', 'New', 'NotificationReminderCheckerTask','0 */5 * ? * *', 'Every 5 minuts')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment], [Comment])
VALUES (10,'NotificationReset', 'New', 'NotificationReset','0 0 5 1 * ?', 'At 2:00:00am, on the 1st day, every month', 'Обнуляем или пересоздаем все нужные нам уведомления (например, лимиты)')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment], [Comment])
VALUES (11,'AccountDailyWork', 'New', 'AccountDailyWork','0 0 10 * * ?', 'At 07:00:00am every day', 'Обновление данных счетов, например начисление процентов по вкладам.')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment], [Comment])
VALUES (12,'ProgressMonthly', 'New', 'ProgressMonthly','0 30 4 1 * ?', 'At 01:30:00am, on the 1st day, every month', 'В начале каждого месяца переносим данные из прогресс бара в историю')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment], [Comment])
VALUES (13,'SystemMailing', 'New', 'SystemMailing','0 */30 * ? * *', 'Every 30 minutes', 'Каждые 30 минут запускает и проеверяет не появились ли новые SystemMailings и не подошла ли следующая дата экспирации')

SET IDENTITY_INSERT [SchedulerTasks] OFF

SET IDENTITY_INSERT [TelegramAccountStatuses] ON 
INSERT INTO TelegramAccountStatuses ( ID, Name, CodeName) Values (1,'Новый','New')
INSERT INTO TelegramAccountStatuses ( ID, Name, CodeName) Values (2,'Подключен','Connected')
INSERT INTO TelegramAccountStatuses ( ID, Name, CodeName) Values (3,'На паузе','InPause')
INSERT INTO TelegramAccountStatuses ( ID, Name, CodeName) Values (4,'Заблокирован','Locked')


SET IDENTITY_INSERT [TelegramAccountStatuses] OFF



INSERT INTO PaymentSystems (Name, CodeName, Logo, IsVisible) values ('Visa', 'Visa', '/resources/payment_system/visa.png', 1)
INSERT INTO PaymentSystems (Name, CodeName, Logo, IsVisible) values ('Mastercard', 'Mastercard', '/resources/payment_system/mastercard.svg', 1)
INSERT INTO PaymentSystems (Name, CodeName, Logo, IsVisible) values ('Maestro', 'Maestro', '/resources/payment_system/maestro.png', 1)
INSERT INTO PaymentSystems (Name, CodeName, Logo, IsVisible) values ('American Express', 'AmericanExpress', '/resources/payment_system/american-express.png', 0)
INSERT INTO PaymentSystems (Name, CodeName, Logo, IsVisible) values ('PayPal', 'Paypal', '/resources/payment_system/paypal.png', 0)
INSERT INTO PaymentSystems (Name, CodeName, Logo, IsVisible) values ('Western Union', 'WesternUnion', '/resources/payment_system/western-union.png', 0)
INSERT INTO PaymentSystems (Name, CodeName, Logo, IsVisible) values ('Mir', 'Mir', '/resources/payment_system/mir.svg', 1)



SET IDENTITY_INSERT [SystemMailings] ON 

INSERT [SystemMailings] ([ID],[Name], [Tooltip], [CodeName], [CronExpression], [CronComment], [IsActive], [IsMail]) 
	VALUES (1, N'Обратная связь от пользователя','1 раз в месяц', 'FeedbackMonth', '0 0 12 L * ?', 'Every month on the last day of the month, at noon', 1, 1)
INSERT [SystemMailings] ([ID],[Name], [CodeName], [IsActive], [TotalMinutes], [IsMail]) 
	VALUES (2, N'Пользователь не активен в течении 1 дня после регистрации', 'NotActive1DayAfterRegistration', 0, 1440, 1)

INSERT [SystemMailings] ([ID],[Name], [CodeName], [IsActive], [TotalMinutes], [IsMail]) 
	VALUES (3, N'Пользователь не активен в течении 2 дней после регистрации', 'NotActive2DaysAfterRegistration', 0, 2880, 1)
INSERT [SystemMailings] ([ID],[Name], [CodeName], [IsActive], [TotalMinutes], [IsMail]) 
	VALUES (4, N'Пользователь не активен в течении 3 дней после регистрации', 'NotActive3DaysAfterRegistration', 0, 4320, 1)
INSERT [SystemMailings] ([ID],[Name], [CodeName], [IsActive], [TotalMinutes], [IsMail]) 
	VALUES (5, N'Пользователь не активен в течении 4 дней после регистрации', 'NotActive4DaysAfterRegistration', 0, 5760, 1)
INSERT [SystemMailings] ([ID],[Name], [CodeName], [IsActive], [TotalMinutes], [IsMail]) 
	VALUES (6, N'Пользователь не активен в течении 5 дней после регистрации', 'NotActive5DaysAfterRegistration', 0, 7200, 1)
INSERT [SystemMailings] ([ID],[Name], [CodeName], [IsActive], [TotalMinutes], [IsMail]) 
	VALUES (7, N'Пользователь не активен в течении 6 дней после регистрации', 'NotActive6DaysAfterRegistration', 0, 8640, 1)
INSERT [SystemMailings] ([ID],[Name], [CodeName], [IsActive], [TotalMinutes], [IsMail]) 
	VALUES (8, N'Пользователь не активен в течении 7 дней после регистрации', 'NotActive7DaysAfterRegistration', 0, 10080, 1)

INSERT [SystemMailings] ([ID],[Name], [CodeName], [CronExpression], [CronComment], [IsActive], [IsMail]) 
	VALUES (9, N'Статистика личных финансов за неделю', 'StatisticsWeek', '0 0 23 ? * SUN *', 'Every Sunday at noon', 0, 1)
INSERT [SystemMailings] ([ID],[Name], [CodeName], [CronExpression], [CronComment], [IsActive], [IsMail]) 
	VALUES (10, N'Статистика личных финансов за неделю', 'StatisticsWeek', '0 0 23 L * ?', 'Every month on the last day of the month, at 10pm', 0, 1)

INSERT [SystemMailings] ([ID],[Name], [CodeName], [IsActive], [TotalMinutes], [IsMail]) 
	VALUES (11, N'Пользователь не заполнял записи в течении 2 дней', 'NotEnterRecords2Days', 0, 2880, 1)
INSERT [SystemMailings] ([ID],[Name], [CodeName], [IsActive], [TotalMinutes], [IsMail]) 
	VALUES (12, N'Пользователь не заполнял записи в течении 3 дней', 'NotEnterRecords3Days', 0, 4320, 1)
INSERT [SystemMailings] ([ID],[Name], [CodeName], [IsActive], [TotalMinutes], [IsMail]) 
	VALUES (13, N'Пользователь не заполнял записи в течении 4 дней', 'NotEnterRecords4Days', 0, 5760, 1)

SET IDENTITY_INSERT [SystemMailings] OFF
GO