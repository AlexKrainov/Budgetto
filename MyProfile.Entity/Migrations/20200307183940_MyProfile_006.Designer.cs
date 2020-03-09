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
    [Migration("20200307183940_MyProfile_006")]
    partial class MyProfile_006
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

                    b.Property<string>("Color");

                    b.Property<string>("CssIcon");

                    b.Property<string>("Currency");

                    b.Property<decimal>("CurrencyPrice")
                        .HasColumnType("Money");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid?>("PersonID");

                    b.HasKey("ID");

                    b.HasIndex("PersonID");

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

                    b.Property<bool>("IsConsider");

                    b.Property<bool>("IsDeleted");

                    b.Property<Guid>("PersonID");

                    b.Property<decimal>("Total")
                        .HasColumnType("Money");

                    b.HasKey("ID");

                    b.HasIndex("BudgetSectionID");

                    b.HasIndex("PersonID");

                    b.ToTable("BudgetRecords");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.BudgetSection", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BudgetAreaID");

                    b.Property<string>("CodeName");

                    b.Property<string>("CssIcon");

                    b.Property<bool>("IsByDefault");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<Guid?>("PersonID");

                    b.Property<string>("Type_RecordType");

                    b.HasKey("ID");

                    b.HasIndex("BudgetAreaID");

                    b.HasIndex("PersonID");

                    b.ToTable("BudgetSections");
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

            modelBuilder.Entity("MyProfile.Entity.Model.PeriodType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CodeName")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("PeriodTypes");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.Person", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("CollectiveBudgetID");

                    b.Property<DateTime>("DateCreate");

                    b.Property<DateTime?>("DateDelete");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("ImageLink");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("LastName");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("CollectiveBudgetID");

                    b.ToTable("People");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.Template", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CodeName")
                        .IsRequired();

                    b.Property<string>("CurrentPeriod");

                    b.Property<DateTime>("DateCreate");

                    b.Property<DateTime?>("DateDelete");

                    b.Property<DateTime>("DateEdit");

                    b.Property<bool>("IsCountCollectiveBudget");

                    b.Property<bool>("IsDelete");

                    b.Property<int>("MaxRowInAPage");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("PeriodTypeID");

                    b.Property<Guid>("PersonID");

                    b.HasKey("ID");

                    b.HasIndex("PeriodTypeID");

                    b.HasIndex("PersonID");

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

            modelBuilder.Entity("MyProfile.Entity.Model.BudgetArea", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.Person", "Person")
                        .WithMany("BudgetAreas")
                        .HasForeignKey("PersonID");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.BudgetRecord", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.BudgetSection", "BudgetSection")
                        .WithMany("BudgetRecords")
                        .HasForeignKey("BudgetSectionID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyProfile.Entity.Model.Person", "Person")
                        .WithMany("BudgetRecords")
                        .HasForeignKey("PersonID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyProfile.Entity.Model.BudgetSection", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.BudgetArea", "BudgetArea")
                        .WithMany("BudgetSectinos")
                        .HasForeignKey("BudgetAreaID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyProfile.Entity.Model.Person", "Person")
                        .WithMany("BudgetSections")
                        .HasForeignKey("PersonID");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.Person", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.CollectiveBudget", "CollectiveBudget")
                        .WithMany("People")
                        .HasForeignKey("CollectiveBudgetID");
                });

            modelBuilder.Entity("MyProfile.Entity.Model.Template", b =>
                {
                    b.HasOne("MyProfile.Entity.Model.PeriodType", "PeriodType")
                        .WithMany()
                        .HasForeignKey("PeriodTypeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyProfile.Entity.Model.Person", "Person")
                        .WithMany("Templates")
                        .HasForeignKey("PersonID")
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
#pragma warning restore 612, 618
        }
    }
}
