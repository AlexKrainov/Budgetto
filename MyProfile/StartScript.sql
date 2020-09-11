USE [u1127477_MyProfile]
GO
SET IDENTITY_INSERT [dbo].[UserTypes] ON 

INSERT [dbo].[UserTypes] ([ID], [CodeName]) VALUES (1, N'User')
INSERT [dbo].[UserTypes] ([ID], [CodeName]) VALUES (2, N'Admin')
SET IDENTITY_INSERT [dbo].[UserTypes] OFF
GO

SET IDENTITY_INSERT [dbo].[ChartTypes] ON 

INSERT [dbo].[ChartTypes] ([ID], [Name], [CodeName], [IsUsing], [IsBig]) VALUES (1, N'Линейная диаграмма', N'line', 1, 1)
INSERT [dbo].[ChartTypes] ([ID], [Name], [CodeName], [IsUsing], [IsBig]) VALUES (2, N'Столбчатая диаграмма', N'bar', 1, 0)
INSERT [dbo].[ChartTypes] ([ID], [Name], [CodeName], [IsUsing], [IsBig]) VALUES (3, N'Круговая диаграмма', N'pie', 1, 0)
INSERT [dbo].[ChartTypes] ([ID], [Name], [CodeName], [IsUsing], [IsBig]) VALUES (4, N'Кольцевая диаграмма', N'doughnut', 1, 0)
INSERT [dbo].[ChartTypes] ([ID], [Name], [CodeName], [IsUsing], [IsBig]) VALUES (5, N'Линейная диаграмма', N'bubble', 1, 0)
INSERT [dbo].[ChartTypes] ([ID], [Name], [CodeName], [IsUsing], [IsBig]) VALUES (6, N'Столбчатая диаграмма', N'bar', 1, 1)
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

INSERT [dbo].[PeriodTypes] ([ID], [Name], [CodeName], [IsUsing]) VALUES (1, N'Бюджет на месяц', N'Days', 0)
INSERT [dbo].[PeriodTypes] ([ID], [Name], [CodeName], [IsUsing]) VALUES (2, N'Weeks for month', N'Week', 0)
INSERT [dbo].[PeriodTypes] ([ID], [Name], [CodeName], [IsUsing]) VALUES (3, N'Бюджет на год', N'Month', 0)
INSERT [dbo].[PeriodTypes] ([ID], [Name], [CodeName], [IsUsing]) VALUES (4, N'Years for 10 Year', N'10_Year', 0)
SET IDENTITY_INSERT [dbo].[PeriodTypes] OFF
GO

SET IDENTITY_INSERT [dbo].[SectionTypes] ON 

INSERT [dbo].[SectionTypes] ([ID], [Name], [CodeName]) VALUES (1, N'Доходы', N'Earnings')
INSERT [dbo].[SectionTypes] ([ID], [Name], [CodeName]) VALUES (2, N'Расходы', N'Spendings')
INSERT [dbo].[SectionTypes] ([ID], [Name], [CodeName]) VALUES (3, N'Инвестиции', N'Investments') 
SET IDENTITY_INSERT [dbo].[SectionTypes] OFF
GO