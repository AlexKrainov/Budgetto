﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyProfile.Entity.Model;

namespace MyProfile.Entity.Migrations
{
    [DbContext(typeof(MyProfile_DBContext))]
    [Migration("20200615155648_MyProfile_013")]
    partial class MyProfile_013
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MyProfile.Entity.Model.BudgetArea", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CodeName");

                    b.Property<string>("CssIcon")
                        .HasMaxLength(64);

                    b.Property<string>("Description");

                    b.Property<bool>("IsPrivate");

                    b.Property<bool>("IsShow");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid?>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("BudgetAreas");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.BudgetRecord", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BudgetSectionID");

                    b.Property<DateTime>("DateTimeCreate");

                    b.Property<DateTime?>("DateTimeDelete");

                    b.Property<DateTime>("DateTimeEdit");

                    b.Property<DateTime>("DateTimeOfPayment");

                    b.Property<string>("Description");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsHide");

                    b.Property<bool>("IsHideForCollection");

                    b.Property<string>("RawData");

                    b.Property<decimal>("Total")
                        .HasColumnType("Money");

                    b.Property<Guid>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("BudgetSectionID");

                    b.HasIndex("DateTimeOfPayment");

                    b.HasIndex("UserID");

                    b.ToTable("BudgetRecords");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.BudgetSection", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BudgetAreaID");

                    b.Property<string>("CodeName");

                    b.Property<string>("CssColor")
                        .HasMaxLength(24);

                    b.Property<string>("CssIcon")
                        .HasMaxLength(64);

                    b.Property<string>("Description");

                    b.Property<bool>("IsPrivate");

                    b.Property<bool>("IsShow");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int?>("SectionTypeID");

                    b.Property<Guid?>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("BudgetAreaID");

                    b.HasIndex("SectionTypeID");

                    b.HasIndex("UserID");

                    b.ToTable("BudgetSections");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.Chart", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ChartTypeID");

                    b.Property<DateTime>("DateCreate");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime>("LastDateEdit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<int>("PeriodTypeID");

                    b.Property<Guid>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("ChartTypeID");

                    b.HasIndex("PeriodTypeID");

                    b.HasIndex("UserID");

                    b.ToTable("Charts");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.ChartType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CodeName")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<bool>("IsUsing");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.HasKey("ID");

                    b.ToTable("ChartTypes");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.CollectiveArea", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AreaID");

                    b.Property<int?>("ChildAreaID");

                    b.HasKey("ID");

                    b.HasIndex("AreaID");

                    b.HasIndex("ChildAreaID");

                    b.ToTable("CollectiveAreas");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.CollectiveBudget", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreate");

                    b.Property<DateTime?>("DateDelete");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("CollectiveBudgets");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.CollectiveSection", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ChildSectionID");

                    b.Property<int?>("SectionID");

                    b.HasKey("ID");

                    b.HasIndex("ChildSectionID");

                    b.HasIndex("SectionID");

                    b.ToTable("CollectiveSections");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.Goal", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateEnd");

                    b.Property<DateTime?>("DateStart");

                    b.Property<string>("Description");

                    b.Property<decimal?>("ExpectationMoney")
                        .HasColumnType("Money");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsFinished");

                    b.Property<bool>("IsShowInCollective");

                    b.Property<bool>("IsShowOnDashBoard")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Goals");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.GoalRecord", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreateDateTime");

                    b.Property<DateTime?>("DateTimeOfPayment");

                    b.Property<int>("GoalID");

                    b.Property<decimal>("Total")
                        .HasColumnType("Money");

                    b.Property<Guid?>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("GoalID");

                    b.HasIndex("UserID");

                    b.ToTable("GoalRecords");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.Limit", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateEnd");

                    b.Property<DateTime?>("DateStart");

                    b.Property<string>("Description");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsShow")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<decimal>("LimitMoney")
                        .HasColumnType("Money");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("PeriodTypeID");

                    b.Property<Guid>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("PeriodTypeID");

                    b.HasIndex("UserID");

                    b.ToTable("Limits");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.PartChart", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ChartID");

                    b.Property<string>("CssColor")
                        .HasMaxLength(24);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.HasKey("ID");

                    b.HasIndex("ChartID");

                    b.ToTable("PartCharts");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.PeriodType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CodeName")
                        .IsRequired();

                    b.Property<bool>("IsUsing");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("PeriodTypes");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.PersonSetting", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("SpecificCulture");

                    b.Property<Guid>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("PersonSettings");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.SectionGroupChart", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BudgetSectionID");

                    b.Property<int>("PartChartID");

                    b.HasKey("ID");

                    b.HasIndex("BudgetSectionID");

                    b.HasIndex("PartChartID");

                    b.ToTable("SectionGroupCharts");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.SectionGroupLimit", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BudgetSectionID");

                    b.Property<int>("LimitID");

                    b.HasKey("ID");

                    b.HasIndex("BudgetSectionID");

                    b.HasIndex("LimitID");

                    b.ToTable("SectionGroupLimits");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.SectionType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CodeName")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.HasKey("ID");

                    b.ToTable("SectionTypes");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.SectionTypeView", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsShow");

                    b.Property<int>("PeriodTypeID");

                    b.Property<int>("SectionTypeID");

                    b.Property<Guid>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("PeriodTypeID");

                    b.HasIndex("SectionTypeID");

                    b.HasIndex("UserID");

                    b.ToTable("SectionTypeViews");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.Template", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CurrentPeriod");

                    b.Property<DateTime>("DateCreate");

                    b.Property<DateTime?>("DateDelete");

                    b.Property<DateTime>("DateEdit");

                    b.Property<string>("Description");

                    b.Property<bool>("IsCountCollectiveBudget");

                    b.Property<bool>("IsDelete");

                    b.Property<DateTime?>("LastSeenDate");

                    b.Property<int>("MaxRowInAPage");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("PeriodTypeID");

                    b.Property<Guid>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("PeriodTypeID");

                    b.HasIndex("UserID");

                    b.ToTable("Templates");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.TemplateBudgetSection", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BudgetSectionID");

                    b.Property<int>("TemplateColumnID");

                    b.HasKey("ID");

                    b.HasIndex("BudgetSectionID");

                    b.HasIndex("TemplateColumnID");

                    b.ToTable("TemplateBudgetSections");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.TemplateColumn", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ColumnTypeID");

                    b.Property<int?>("FooterActionTypeID");

                    b.Property<string>("Format");

                    b.Property<string>("Formula")
                        .IsRequired();

                    b.Property<bool>("IsShow");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Order");

                    b.Property<int?>("PlaceAfterCommon");

                    b.Property<int>("TemplateID");

                    b.HasKey("ID");

                    b.HasIndex("TemplateID");

                    b.ToTable("TemplateColumns");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.User", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("CollectiveBudgetID");

                    b.Property<DateTime>("DateCreate")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime?>("DateDelete");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("ImageLink");

                    b.Property<bool>("IsAllowCollectiveBudget");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("LastName");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("CollectiveBudgetID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.UserLog", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActionCodeName")
                        .HasMaxLength(16);

                    b.Property<string>("BrowerName")
                        .HasMaxLength(32);

                    b.Property<string>("BrowserVersion")
                        .HasMaxLength(16);

                    b.Property<string>("City")
                        .HasMaxLength(32);

                    b.Property<string>("Comment");

                    b.Property<string>("Country")
                        .HasMaxLength(32);

                    b.Property<DateTime>("CurrentDateTime");

                    b.Property<string>("IP")
                        .HasMaxLength(64);

                    b.Property<string>("Location")
                        .HasMaxLength(64);

                    b.Property<string>("OS_Name")
                        .HasMaxLength(32);

                    b.Property<string>("Os_Version")
                        .HasMaxLength(16);

                    b.Property<int?>("ParentUserLogID");

                    b.Property<string>("PostCode")
                        .HasMaxLength(16);

                    b.Property<string>("ScreenSize")
                        .HasMaxLength(16);

                    b.Property<string>("SessionID")
                        .HasMaxLength(32);

                    b.Property<Guid?>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("ParentUserLogID");

                    b.HasIndex("UserID");

                    b.ToTable("UserLogs");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.UserSettings", b =>
                {
                    b.Property<Guid>("ID");

                    b.Property<bool>("BudgetPages_EarningChart")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<bool>("BudgetPages_InvestingChart")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<bool>("BudgetPages_IsShow_Goals")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<bool>("BudgetPages_IsShow_Limits")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<bool>("BudgetPages_SpendingChart")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<bool>("BudgetPages_WithCollective")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<bool>("GoalPage_IsShow_Collective");

                    b.Property<bool>("GoalPage_IsShow_Finished");

                    b.HasKey("ID");

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.BudgetArea", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.User", "User")
                        .WithMany("BudgetAreas")
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.BudgetRecord", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.BudgetSection", "BudgetSection")
                        .WithMany("BudgetRecords")
                        .HasForeignKey("BudgetSectionID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyProfile.Entity.Model.User", "User")
                        .WithMany("BudgetRecords")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyProfile.Entity.Model.BudgetSection", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.BudgetArea", "BudgetArea")
                        .WithMany("BudgetSectinos")
                        .HasForeignKey("BudgetAreaID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyProfile.Entity.Model.SectionType", "SectionType")
                        .WithMany()
                        .HasForeignKey("SectionTypeID");

                    b.HasOne("MyProfile.Entity.Model.User", "User")
                        .WithMany("BudgetSections")
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.Chart", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.ChartType", "ChartType")
                        .WithMany()
                        .HasForeignKey("ChartTypeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyProfile.Entity.Model.PeriodType", "PeriodType")
                        .WithMany()
                        .HasForeignKey("PeriodTypeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyProfile.Entity.Model.User", "User")
                        .WithMany("Charts")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyProfile.Entity.Model.CollectiveArea", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.BudgetArea", "Area")
                        .WithMany("CollectiveAreas")
                        .HasForeignKey("AreaID");

                    b.HasOne("MyProfile.Entity.Model.BudgetArea", "ChildArea")
                        .WithMany()
                        .HasForeignKey("ChildAreaID");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.CollectiveSection", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.BudgetSection", "ChildSection")
                        .WithMany()
                        .HasForeignKey("ChildSectionID");

                    b.HasOne("MyProfile.Entity.Model.BudgetSection", "Section")
                        .WithMany("CollectiveSections")
                        .HasForeignKey("SectionID");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.Goal", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyProfile.Entity.Model.GoalRecord", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.Goal", "Goal")
                        .WithMany("GoalRecords")
                        .HasForeignKey("GoalID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyProfile.Entity.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.Limit", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.PeriodType", "PeriodType")
                        .WithMany()
                        .HasForeignKey("PeriodTypeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyProfile.Entity.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyProfile.Entity.Model.PartChart", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.Chart", "Chart")
                        .WithMany("PartCharts")
                        .HasForeignKey("ChartID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyProfile.Entity.Model.PersonSetting", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyProfile.Entity.Model.SectionGroupChart", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.BudgetSection", "BudgetSection")
                        .WithMany()
                        .HasForeignKey("BudgetSectionID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyProfile.Entity.Model.PartChart", "PartChart")
                        .WithMany("SectionGroupCharts")
                        .HasForeignKey("PartChartID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyProfile.Entity.Model.SectionGroupLimit", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.BudgetSection", "BudgetSection")
                        .WithMany("SectionGroupLimits")
                        .HasForeignKey("BudgetSectionID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyProfile.Entity.Model.Limit", "Limit")
                        .WithMany("SectionGroupLimits")
                        .HasForeignKey("LimitID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyProfile.Entity.Model.SectionTypeView", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.PeriodType", "PeriodType")
                        .WithMany()
                        .HasForeignKey("PeriodTypeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyProfile.Entity.Model.SectionType", "SectionType")
                        .WithMany()
                        .HasForeignKey("SectionTypeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyProfile.Entity.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyProfile.Entity.Model.Template", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.PeriodType", "PeriodType")
                        .WithMany()
                        .HasForeignKey("PeriodTypeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyProfile.Entity.Model.User", "User")
                        .WithMany("Templates")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyProfile.Entity.Model.TemplateBudgetSection", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.BudgetSection", "BudgetSection")
                        .WithMany()
                        .HasForeignKey("BudgetSectionID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyProfile.Entity.Model.TemplateColumn", "TemplateColumn")
                        .WithMany("TemplateBudgetSections")
                        .HasForeignKey("TemplateColumnID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyProfile.Entity.Model.TemplateColumn", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.Template", "Template")
                        .WithMany("TemplateColumns")
                        .HasForeignKey("TemplateID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyProfile.Entity.Model.User", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.CollectiveBudget", "CollectiveBudget")
                        .WithMany("Users")
                        .HasForeignKey("CollectiveBudgetID");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.UserLog", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.UserLog", "ParentUserLog")
                        .WithMany()
                        .HasForeignKey("ParentUserLogID");

                    b.HasOne("MyProfile.Entity.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.UserSettings", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.User", "User")
                        .WithOne("UserSettings")
                        .HasForeignKey("MyProfile.Entity.Model.UserSettings", "ID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
