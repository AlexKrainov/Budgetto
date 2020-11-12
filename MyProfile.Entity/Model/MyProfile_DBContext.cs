using Microsoft.EntityFrameworkCore;
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
            modelBuilder.Entity<BudgetRecord>()
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
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.NewsLetter)
              .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.IsShowHints)
              .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
              .Property(b => b.IsShowFirstEnterHint)
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

            modelBuilder.Entity<BudgetRecord>()
                .Property(b => b.CurrencyID)
                .HasDefaultValue(1);
            modelBuilder.Entity<BudgetRecord>()
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

            #endregion
        }

        public virtual DbSet<BudgetArea> BudgetAreas { get; set; }
        public virtual DbSet<BudgetSection> BudgetSections { get; set; }
        public virtual DbSet<BudgetRecord> BudgetRecords { get; set; }
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
        public virtual DbSet<PromoCodeHistory> PromoCodeHistories { get; set; }
        public virtual DbSet<UserErrorLog> UserErrorLogs { get; set; }

    }
}
