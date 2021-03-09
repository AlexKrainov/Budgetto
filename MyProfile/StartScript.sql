USE [u1127477_MyProfile]
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
INSERT [dbo].[ChartTypes] ([ID], [Name], [CodeName], [IsUsing], [IsBig]) VALUES (6, N'Столбчатый график', N'bar', 1, 1)
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
INSERT [dbo].[SectionTypes] ([ID], [Name], [CodeName]) VALUES (3, N'Инвестиции', N'Investments') 
SET IDENTITY_INSERT [dbo].[SectionTypes] OFF
GO



SET IDENTITY_INSERT [HelpMenus] ON 
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (2, 'Финансы на месяц/год' ,'pe-7s-display1' ,1 ,3)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (3, 'Лимиты' ,'lnr lnr-frame-expand' ,1 ,4)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (4, 'Цели' ,'lnr lnr-rocket' ,1 ,5)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (5, 'Графики' ,'lnr lnr-pie-chart' ,1 ,6)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (6, 'Шаблоны' ,'lnr lnr-layers' ,1 ,7)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (7, 'Категории и папки' ,'pe-7s-albums' ,1 ,2)
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
VALUES (5, 'Создание папки' ,'2020-11-26 21:40:32.8652966', '2020-11-26 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/AddFolder.cshtml' ,7, 'создание папка; папки; папка; ')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (6, 'Создание категории' ,'2020-12-01 21:40:32.8652966', '2020-12-01 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/AddSection.cshtml' ,7, 'создание категории; категории; категория; ')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (7, 'Смена папки у категории' ,'2020-12-01 21:40:32.8652966', '2020-12-01 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/ChangeFolder.cshtml' ,7, 'смена папки, поменять папку; папка; категория; ')
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

INSERT [AccountTypes] ([ID], [Name], [CodeName], [Icon]) VALUES (1, N'Наличные', N'Cash', 'ion ion-ios-cash')
INSERT [AccountTypes] ([ID], [Name], [CodeName], [Icon]) VALUES (2, N'Дебетовый счет', N'Debed', 'fas fa-credit-card')
INSERT [AccountTypes] ([ID], [Name], [CodeName], [Icon]) VALUES (3, N'Кредитный счет', N'Credit', 'fas fa-credit-card')
INSERT [AccountTypes] ([ID], [Name], [CodeName], [Icon]) VALUES (4, N'Карта рассрочки', N'Installment', 'fas fa-credit-card')
SET IDENTITY_INSERT [AccountTypes] OFF
GO

SET IDENTITY_INSERT [Banks] ON 

INSERT [Banks] ([ID], [Name], [ImageSrc]) VALUES (1, N'Сбер', '/resources/banks/sber.svg')
INSERT [Banks] ([ID], [Name], [ImageSrc]) VALUES (2, N'ВТБ', '/resources/banks/vtb.svg')
INSERT [Banks] ([ID], [Name], [ImageSrc]) VALUES (3, N'Газпромбанк', '/resources/banks/gasprom.svg')
INSERT [Banks] ([ID], [Name], [ImageSrc]) VALUES (4, N'Альфа-Банк', '/resources/banks/alfa-logo.svg')
INSERT [Banks] ([ID], [Name], [ImageSrc]) VALUES (5, N'Банк Открытие', '/resources/banks/open.svg')
INSERT [Banks] ([ID], [Name], [ImageSrc]) VALUES (6, N'Тинькофф Банк', '/resources/banks/tinkoff-bank.png')
INSERT [Banks] ([ID], [Name], [ImageSrc]) VALUES (7, N'Национальный Клиринговый Центр', '/resources/banks/moex.svg')
INSERT [Banks] ([ID], [Name], [ImageSrc]) VALUES (8, N'Россельхозбанк', '/resources/banks/rosselhoz.jfif')
INSERT [Banks] ([ID], [Name], [ImageSrc]) VALUES (9, N'Московский Кредитный Банк', '/resources/banks/mkb.svg')
INSERT [Banks] ([ID], [Name], [ImageSrc]) VALUES (10, N'Совкомбанк', '/resources/banks/sovkombank.svg')
INSERT [Banks] ([ID], [Name], [ImageSrc]) VALUES (11, N'Росбанк', '/resources/banks/ros.svg')
INSERT [Banks] ([ID], [Name], [ImageSrc]) VALUES (12, N'Райффайзенбанк', '/resources/banks/rasf.svg')
INSERT [Banks] ([ID], [Name], [ImageSrc]) VALUES (13, N'Ситибанк', '/resources/banks/citi.svg')
INSERT [Banks] ([ID], [Name], [ImageSrc]) VALUES (2500, N'Другой банк', '/resources/banks/bank.svg')

SET IDENTITY_INSERT [Banks] OFF
GO

SET IDENTITY_INSERT [SchedulerTasks] ON

INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment])
VALUES (1,'AccountRemoveCachback', 'New', 'AccountRemoveCachback', '0 0 12 1 * ?', 'Every month on the 1st, at noon')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment])
VALUES (2,'SetDoneToReminderDates', 'New', 'SetDoneToReminderDates', '0 0 1 * * ?', 'Every day at 1am')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment])
VALUES (3,'CurrencyHistoryTask', 'New', 'CurrencyHistoryTask', '0 0 * ? * *', 'Every hour')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment])
VALUES (4,'ResetHubConnectTask', 'New', 'ResetHubConnectTask','0 0 4 * * ?', 'Every day at 4am')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment])
VALUES (5,'NotificationLimitCheckerTask', 'New', 'NotificationLimitCheckerTask','0 * * ? * *', 'Every minuts')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment])
VALUES (6,'NotificationLimitSiteTask', 'New', 'NotificationSiteTask','0/17 * * ? * * *', 'Every 17 seconds')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment])
VALUES (7,'NotificationTelegramTask', 'New', 'NotificationTelegramTask','0 * * ? * *', 'Every minuts')
INSERT INTO [SchedulerTasks] ([ID], [Name], [TaskStatus], [TaskType], [CronExpression], [CronComment])
VALUES (8,'NotificationMailTask', 'New', 'NotificationMailTask','0 * * ? * *', 'Every minuts')

SET IDENTITY_INSERT [SchedulerTasks] OFF