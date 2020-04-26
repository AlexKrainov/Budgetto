using Microsoft.EntityFrameworkCore;

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
			//	base.OnModelCreating(modelBuilder);

			//modelBuilder.Entity<BudgetArea>();
			//modelBuilder.Entity<BudgetRecord>()
			//.HasOne(x => x.BudgetSection)
			//.WithMany(x => x.BudgetRecords)
			//.OnDelete(DeleteBehavior.Cascade);
			//modelBuilder.Entity<BudgetSection>();
			//modelBuilder.Entity<Template>();
			//modelBuilder.Entity<TemplateAreaType>()
			//	.HasOne(x => x.TemplateColumn)
			//	.WithMany(x => x.TemplateAreaTypes)
			//	.OnDelete(DeleteBehavior.Cascade);
			//modelBuilder.Entity<CollectiveBudget>();
			//modelBuilder.Entity<Person>();
			//modelBuilder.Entity<TemplateColumn>();
			//modelBuilder.Entity<PeriodType>();
		}

		public virtual DbSet<BudgetArea> BudgetAreas { get; set; }
		public virtual DbSet<BudgetSection> BudgetSections { get; set; }
		public virtual DbSet<BudgetRecord> BudgetRecords { get; set; }
		public virtual DbSet<Limit> Limits { get; set; }
		public virtual DbSet<Template> Templates { get; set; }
		public virtual DbSet<TemplateBudgetSection> TemplateBudgetSections { get; set; }
		public virtual DbSet<CollectiveBudget> CollectiveBudgets { get; set; }
		public virtual DbSet<Person> People { get; set; }
		public virtual DbSet<PersonSetting> PersonSettings { get; set; }
		public virtual DbSet<TemplateColumn> TemplateColumns { get; set; }
		public virtual DbSet<PeriodType> PeriodTypes { get; set; }
	}
}
