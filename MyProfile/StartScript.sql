USE [u1127477_MyProfile]
GO
SET IDENTITY_INSERT [dbo].[UserTypes] ON 

INSERT [dbo].[UserTypes] ([ID], [CodeName]) VALUES (1, N'User')
INSERT [dbo].[UserTypes] ([ID], [CodeName]) VALUES (2, N'Admin')
INSERT [dbo].[UserTypes] ([ID], [CodeName]) VALUES (3, N'Tester')
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

INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (1, N'Russian Ruble', N'RUB', N'ru-RU', N'₽', 1, NULL, NULL)
INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (2, N'Euro', N'EUR', N'de-DE', N'€', 0, N'R01239', 978)
INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (4, N'United States Dollar', N'USD', N'en-US', N'$', 0, N'R01235', 840)
INSERT [dbo].[Currencies] ([ID], [Name], [CodeName], [SpecificCulture], [Icon], [CanBeUser], [CodeName_CBR], [CodeNumber_CBR]) VALUES (5, N'United Kingdom Pound', N'GBP', N'en-GB', N'£', 0, N'R01035', 826)
SET IDENTITY_INSERT [dbo].[Currencies] OFF
GO

SET IDENTITY_INSERT [dbo].[MailTypes] ON 

INSERT [dbo].[MailTypes] ([ID], [CodeName]) VALUES (1, N'ConfirmEmail')
INSERT [dbo].[MailTypes] ([ID], [CodeName]) VALUES (2, N'ResetPassword')
INSERT [dbo].[MailTypes] ([ID], [CodeName]) VALUES (3, N'Login')
INSERT [dbo].[MailTypes] ([ID], [CodeName]) VALUES (4, N'ResendByUser')
SET IDENTITY_INSERT [dbo].[MailTypes] OFF
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
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (2, 'Финансы на месяц/год' ,'pe-7s-display1' ,1 ,0)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (3, 'Лимиты' ,'lnr lnr-frame-expand' ,1 ,1)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (4, 'Цели' ,'lnr lnr-rocket' ,1 ,2)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (5, 'Графики' ,'lnr lnr-pie-chart' ,1 ,3)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (6, 'Шаблоны' ,'lnr lnr-layers' ,1 ,4)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (7, 'Категории и папки' ,'pe-7s-albums' ,1 ,5)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (8, 'Календарь' ,'lnr lnr-calendar-full' ,1 ,6)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (9, 'Общие вопросы' ,'lnr lnr-question-circle' ,1 ,7)
INSERT INTO [HelpMenus] ([ID],[Title] ,[Icon] ,[IsVisible] ,[Order]) VALUES (10, 'Записи' ,'oi oi-dollar' ,1 ,8)
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
VALUES (6, 'Создание категори' ,'2020-12-01 21:40:32.8652966', '2020-12-01 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/AddSection.cshtml' ,7, 'создание категории; категории; категория; ')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (7, 'Смена папки у категории' ,'2020-12-01 21:40:32.8652966', '2020-12-01 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/ChangeFolder.cshtml' ,7, 'смена папки, поменять папку; папка; категория; ')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (8, 'Создание напоминания' ,'2020-12-02 21:40:32.8652966', '2020-12-02 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/AddReminder.cshtml' ,2, 'создать напоминание, добавить напоминание; напоминани; ')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (9, 'Добавление записи' ,'2020-12-02 21:40:32.8652966', '2020-12-02 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/AddRecord.cshtml' ,10, 'создать записи, добавление записей; записи; запись; добавить расход; добавить доход; доход; расход; инвестиции; ')
INSERT INTO HelpArticles (ID, [Title] ,DateCreate, DateEdit, Link, HelpMenuID, KeyWords) 
VALUES (10, 'Добавление записи в валюте' ,'2020-12-02 21:40:32.8652966', '2020-12-02 21:40:32.8652966' ,'~/Areas/Help/Views/Articles/AddCurrencyRecord.cshtml' ,10, 'создать записи, добавление записей; записи; запись; добавить расход; добавить доход; доход; расход; инвестиции; валюта; доллар; евро; ')

SET IDENTITY_INSERT [HelpArticles] OFF
GO
