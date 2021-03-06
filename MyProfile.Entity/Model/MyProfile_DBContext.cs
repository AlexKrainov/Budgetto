using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography.X509Certificates;

namespace MyProfile.Entity.Model
{
    public partial class MyProfile_DBContext : DbContext
    {
        protected MyProfile_DBContext()
        {
        }

        public MyProfile_DBContext(DbContextOptions<MyProfile_DBContext> options)
        : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging(true);
            optionsBuilder.UseLazyLoadingProxies();
        }
        ////Инициализация БД начальными данными
        ////https://metanit.com/sharp/entityframeworkcore/2.14.php
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Indexes
            modelBuilder.Entity<Record>()
             .HasIndex(x => x.DateTimeOfPayment);

            #endregion

            modelBuilder.Entity<CollectiveSection>()
                .HasOne(x => x.Section)
                .WithMany(y => y.CollectiveSections);

            //modelBuilder.Entity<ChatUser>()
            //    .HasOne(x => x.User)
            //    .WithMany(y => y.use);

            #region Default value

            modelBuilder.Entity<UserSettings>()
                .Property(b => b.BudgetPages_WithCollective)
                .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
                .Property(b => b.Month_EarningWidget)
                .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
                .Property(b => b.Month_SpendingWidget)
                .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
                .Property(b => b.Month_InvestingWidget)
                .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
                .Property(b => b.Month_LimitWidgets)
                .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
                .Property(b => b.Month_GoalWidgets)
                .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
                .Property(b => b.Month_BigCharts)
                .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
               .Property(b => b.LimitPage_Show_IsFinished)
               .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.Year_LimitWidgets)
              .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.Year_BigCharts)
              .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.Year_EarningWidget)
              .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.Year_GoalWidgets)
              .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.Year_InvestingWidget)
              .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.Year_SpendingWidget)
              .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.WebSiteTheme)
              .HasDefaultValue(WebSiteThemeEnum.Light);
            //modelBuilder.Entity<UserSettings>()
            //  .Property(b => b.Mail_News)
            //  .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.IsShowHints)
              .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.IsShowFirstEnterHint)
              .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
             .Property(b => b.IsShowCookie)
             .HasDefaultValue(true);
            //modelBuilder.Entity<UserSettings>()
            //.Property(b => b.Mail_Reminders)
            //.HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
            .Property(b => b.CanUseAlgorithm)
            .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.Month_Accounts)
              .HasDefaultValue(false);
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.Month_Summary)
              .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.Year_Accounts)
              .HasDefaultValue(false);
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.Year_Summary)
              .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.Month_Statistics)
              .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.Year_Statistics)
              .HasDefaultValue(true);

            //modelBuilder.Entity<VisibleElement>()
            //    .Property(b => b.IsShow_BudgetMonth)
            //    .HasDefaultValue(true);
            //modelBuilder.Entity<VisibleElement>()
            //    .Property(b => b.IsShow_BudgetYear)
            //    .HasDefaultValue(true);


            modelBuilder.Entity<User>()
                .Property(b => b.UserTypeID)
                .HasDefaultValue(1);
            modelBuilder.Entity<User>()
               .Property(b => b.CurrencyID)
               .HasDefaultValue(1);

            modelBuilder.Entity<Record>()
                .Property(b => b.CurrencyID)
                .HasDefaultValue(1);
            modelBuilder.Entity<Record>()
                .Property(b => b.CurrencyNominal)
                .HasDefaultValue(1);

            modelBuilder.Entity<BudgetSection>()
               .Property(b => b.IsShowInCollective)
               .HasDefaultValue(true);
            modelBuilder.Entity<BudgetSection>()
                .Property(b => b.IsShowOnSite)
                .HasDefaultValue(true);
            modelBuilder.Entity<BudgetSection>()
                .Property(b => b.CssBackground)
                .HasDefaultValue("#eeeeee");
            modelBuilder.Entity<BudgetSection>()
               .Property(b => b.CssColor)
               .HasDefaultValue("#rgba(24,28,33,0.8)");

            modelBuilder.Entity<BudgetArea>()
               .Property(b => b.IsShowInCollective)
               .HasDefaultValue(true);
            modelBuilder.Entity<BudgetArea>()
                .Property(b => b.IsShowOnSite)
                .HasDefaultValue(true);


            modelBuilder.Entity<VisibleElement>()
              .Property(b => b.IsShowInCollective)
              .HasDefaultValue(false);
            modelBuilder.Entity<VisibleElement>()
              .Property(b => b.IsShowOnDashboards)
              .HasDefaultValue(true);
            //modelBuilder.Entity<VisibleElement>()
            //  .Property(b => b.IsShow_BudgetMonth)
            //  .HasDefaultValue(true);
            //modelBuilder.Entity<VisibleElement>()
            //  .Property(b => b.IsShow_BudgetYear)
            //  .HasDefaultValue(true);


            modelBuilder.Entity<HelpMenu>()
             .Property(b => b.IsVisible)
             .HasDefaultValue(true);

            modelBuilder.Entity<HelpArticle>()
             .Property(b => b.IsVisible)
             .HasDefaultValue(true);

            modelBuilder.Entity<TelegramAccount>()
            .Property(b => b.StatusID)
            .HasDefaultValue((int)TelegramAccountStatusEnum.New);

            modelBuilder.Entity<Account>()
                   .HasMany(x => x.AccountHistories)
                   .WithOne(x => x.Account)
                   .HasForeignKey(x => x.AccountID);

            modelBuilder.Entity<Account>()
                   .HasMany(x => x.AccountHistories2)
                   .WithOne(x => x.AccountFrom)
                   .HasForeignKey(x => x.AccountIDFrom);

            modelBuilder.Entity<Limit>()
                .Property(b => b.CurrencyID)
                .HasDefaultValue(1);

            modelBuilder.Entity<SubScriptionOption>()
               .Property(b => b.IsActive)
               .HasDefaultValue(true);


            modelBuilder.Entity<SubScription>()
               .Property(b => b.IsActive)
               .HasDefaultValue(true);

            modelBuilder.Entity<Progress>()
             .Property(b => b.IsActive)
             .HasDefaultValue(true);

            #endregion
        }

        public virtual DbSet<BudgetArea> BudgetAreas { get; set; }
        public virtual DbSet<BudgetSection> BudgetSections { get; set; }
        public virtual DbSet<Record> BudgetRecords { get; set; }
        public virtual DbSet<Limit> Limits { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<TemplateBudgetSection> TemplateBudgetSections { get; set; }
        public virtual DbSet<CollectiveBudget> CollectiveBudgets { get; set; }
        public virtual DbSet<CollectiveSection> CollectiveSections { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<UserSession> UserSessions { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<UserLog> UserLogs { get; set; }
        public virtual DbSet<TemplateColumn> TemplateColumns { get; set; }
        public virtual DbSet<PeriodType> PeriodTypes { get; set; }
        public virtual DbSet<SectionGroupLimit> SectionGroupLimits { get; set; }
        public virtual DbSet<SectionType> SectionTypes { get; set; }
        public virtual DbSet<SectionTypeView> SectionTypeViews { get; set; }
        public virtual DbSet<UserSettings> UserSettings { get; set; }
        public virtual DbSet<Goal> Goals { get; set; }
        public virtual DbSet<GoalRecord> GoalRecords { get; set; }
        public virtual DbSet<Chart> Charts { get; set; }
        public virtual DbSet<ChartType> ChartTypes { get; set; }
        public virtual DbSet<ChartField> ChartFields { get; set; }
        public virtual DbSet<SectionGroupChart> SectionGroupCharts { get; set; }
        public virtual DbSet<VisibleElement> VisibleElements { get; set; }
        public virtual DbSet<MailLog> MailLogs { get; set; }
        public virtual DbSet<MailType> MailTypes { get; set; }
        public virtual DbSet<CollectiveBudgetRequest> CollectiveBudgetRequests { get; set; }
        public virtual DbSet<CollectiveBudgetRequestOwner> CollectiveBudgetRequestOwners { get; set; }
        public virtual DbSet<CollectiveBudgetUser> CollectiveBudgetUsers { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<Resource> Resources { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<ChatUser> ChatUsers { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<ResourceMessage> ResourceMessages { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<SiteSettings> SiteSettings { get; set; }
        public virtual DbSet<Reminder> Reminders { get; set; }
        public virtual DbSet<ReminderDate> ReminderDates { get; set; }
        public virtual DbSet<ToDoList> ToDoLists { get; set; }
        public virtual DbSet<ToDoListItem> ToDoListItems { get; set; }
        public virtual DbSet<ToDoListFolder> ToDoListFolders { get; set; }
        public virtual DbSet<HelpArticle> HelpArticles { get; set; }
        public virtual DbSet<HelpArticleUserView> HelpArticleUserViews { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentHistory> PaymentHistories { get; set; }
        public virtual DbSet<PromoCode> PromoCodes { get; set; }
        public virtual DbSet<PromoCodeLog> PromoCodeLogs { get; set; }
        public virtual DbSet<UserErrorLog> UserErrorLogs { get; set; }
        public virtual DbSet<IPSetting> IPSettings { get; set; }
        public virtual DbSet<HelpMenu> HelpMenus { get; set; }
        public virtual DbSet<RecordTag> RecordTags { get; set; }
        public virtual DbSet<UserTag> UserTags { get; set; }
        public virtual DbSet<AccountType> AccountTypes { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Bank> Banks { get; set; }
        public virtual DbSet<RecordHistory> RecordHistories { get; set; }
        public virtual DbSet<AccountHistory> AccountHistories { get; set; }
        public virtual DbSet<CurrencyRateHistory> CurrencyRateHistories { get; set; }
        public virtual DbSet<Summary> Summaries { get; set; }
        public virtual DbSet<UserSummary> UserSummaries { get; set; }
        public virtual DbSet<UserSummarySection> UserSummarySections { get; set; }
        public virtual DbSet<UserSummarySectionType> UserSummarySectionTypes { get; set; }
        public virtual DbSet<SchedulerTask> SchedulerTasks { get; set; }
        public virtual DbSet<SchedulerTaskLog> SchedulerTaskLogs { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<UserConnect> UserConnects { get; set; }
        public virtual DbSet<HubConnect> HubConnects { get; set; }
        public virtual DbSet<TelegramAccount> TelegramAccounts { get; set; }
        public virtual DbSet<TelegramAccountStatus> TelegramAccountStatuses { get; set; }
        public virtual DbSet<MyTimeZone> TimeZones { get; set; }
        public virtual DbSet<OlsonTZID> OlsonTZIDs { get; set; }
        public virtual DbSet<PaymentSystem> PaymentSystems { get; set; }
        public virtual DbSet<BankType> BankTypes { get; set; }
        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<CardPaymentSystem> CardPaymentSystems { get; set; }
        public virtual DbSet<AccountInfo> AccountInfos { get; set; }
        public virtual DbSet<SubScription> SubScriptions { get; set; }
        public virtual DbSet<SubScriptionCategory> SubScriptionCategories { get; set; }

        public virtual DbSet<SubScriptionOption> SubScriptionOptions { get; set; }
        public virtual DbSet<SubScriptionPricing> SubScriptionPricings { get; set; }
        public virtual DbSet<UserSubScription> UserSubScriptions { get; set; }
        public virtual DbSet<PaymentTariff> PaymentTariffs { get; set; }
        public virtual DbSet<PaymentCounter> PaymentCounters { get; set; }
        public virtual DbSet<EntityType> EntityTypes { get; set; }
        public virtual DbSet<UserEntityCounter> UserEntityCounters { get; set; }
        public virtual DbSet<BaseArea> BaseAreas { get; set; }
        public virtual DbSet<BaseSection> BaseSections { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<MccCategory> MccCategories { get; set; }
        public virtual DbSet<MccCode> MccCodes { get; set; }
        public virtual DbSet<Progress> Progresses { get; set; }
        public virtual DbSet<ProgressLog> ProgressLogs { get; set; }
        public virtual DbSet<ProgressItemType> ProgressItemTypes { get; set; }
        public virtual DbSet<ProgressType> ProgressTypes { get; set; }
        public virtual DbSet<PaymentLog> PaymentLogs { get; set; }
        public virtual DbSet<SystemMailing> SystemMailings { get; set; }

    }
}
