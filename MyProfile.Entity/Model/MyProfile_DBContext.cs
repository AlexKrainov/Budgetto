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

            #region Default value

            modelBuilder.Entity<UserSettings>()
                .Property(b => b.BudgetPages_WithCollective)
                .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
                .Property(b => b.BudgetPages_EarningChart)
                .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
                .Property(b => b.BudgetPages_SpendingChart)
                .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
                .Property(b => b.BudgetPages_InvestingChart)
                .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
                .Property(b => b.BudgetPages_IsShow_Limits)
                .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
                .Property(b => b.BudgetPages_IsShow_Goals)
                .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
                .Property(b => b.BudgetPages_IsShow_BigCharts)
                .HasDefaultValue(true);
            modelBuilder.Entity<UserSettings>()
               .Property(b => b.LimitPage_Show_IsFinished)
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
            modelBuilder.Entity<VisibleElement>()
              .Property(b => b.IsShow_BudgetMonth)
              .HasDefaultValue(true);
            modelBuilder.Entity<VisibleElement>()
              .Property(b => b.IsShow_BudgetYear)
              .HasDefaultValue(true);
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


    }
}
