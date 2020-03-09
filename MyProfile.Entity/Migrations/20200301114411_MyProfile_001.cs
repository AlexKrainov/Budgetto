using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CollectiveBudgets",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateDelete = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectiveBudgets", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PeriodTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    CodeName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    ImageLink = table.Column<string>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateDelete = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CollectiveBudgetID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.ID);
                    table.ForeignKey(
                        name: "FK_People_CollectiveBudgets_CollectiveBudgetID",
                        column: x => x.CollectiveBudgetID,
                        principalTable: "CollectiveBudgets",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BudgetAreas",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    CodeName = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    CurrencyPrice = table.Column<decimal>(type: "Money", nullable: false),
                    Color = table.Column<string>(nullable: true),
                    CssIcon = table.Column<string>(nullable: true),
                    PersonID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAreas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BudgetAreas_People_PersonID",
                        column: x => x.PersonID,
                        principalTable: "People",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    CodeName = table.Column<string>(nullable: false),
                    CurrentPeriod = table.Column<string>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateEdit = table.Column<DateTime>(nullable: false),
                    DateDelete = table.Column<DateTime>(nullable: true),
                    IsCountCollectiveBudget = table.Column<bool>(nullable: false),
                    MaxRowInAPage = table.Column<int>(nullable: false),
                    IsDelete = table.Column<bool>(nullable: false),
                    PersonID = table.Column<Guid>(nullable: false),
                    PeriodTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Templates_PeriodTypes_PeriodTypeID",
                        column: x => x.PeriodTypeID,
                        principalTable: "PeriodTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Templates_People_PersonID",
                        column: x => x.PersonID,
                        principalTable: "People",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BundgetSections",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    CodeName = table.Column<string>(nullable: true),
                    Type_RecordType = table.Column<string>(nullable: true),
                    CssIcon = table.Column<string>(nullable: true),
                    PersonID = table.Column<Guid>(nullable: true),
                    BudgetAreaID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BundgetSections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BundgetSections_BudgetAreas_BudgetAreaID",
                        column: x => x.BudgetAreaID,
                        principalTable: "BudgetAreas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BundgetSections_People_PersonID",
                        column: x => x.PersonID,
                        principalTable: "People",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TemplateColumns",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    IsShow = table.Column<bool>(nullable: false),
                    Formula = table.Column<string>(nullable: false),
                    TemplateID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateColumns", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TemplateColumns_Templates_TemplateID",
                        column: x => x.TemplateID,
                        principalTable: "Templates",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetRecords",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Total = table.Column<decimal>(type: "Money", nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DateTimeOfPayment = table.Column<DateTime>(nullable: false),
                    DateTimeCreate = table.Column<DateTime>(nullable: false),
                    DateTimeEdit = table.Column<DateTime>(nullable: false),
                    DateTimeDelete = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsConsider = table.Column<bool>(nullable: false),
                    PersonID = table.Column<Guid>(nullable: false),
                    BudgetSectionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetRecords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BudgetRecords_BundgetSections_BudgetSectionID",
                        column: x => x.BudgetSectionID,
                        principalTable: "BundgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetRecords_People_PersonID",
                        column: x => x.PersonID,
                        principalTable: "People",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateBudgetSections",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BudgetSectionID = table.Column<int>(nullable: false),
                    TemplateColumnID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateBudgetSections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TemplateBudgetSections_BundgetSections_BudgetSectionID",
                        column: x => x.BudgetSectionID,
                        principalTable: "BundgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateBudgetSections_TemplateColumns_TemplateColumnID",
                        column: x => x.TemplateColumnID,
                        principalTable: "TemplateColumns",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAreas_PersonID",
                table: "BudgetAreas",
                column: "PersonID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRecords_BudgetSectionID",
                table: "BudgetRecords",
                column: "BudgetSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRecords_PersonID",
                table: "BudgetRecords",
                column: "PersonID");

            migrationBuilder.CreateIndex(
                name: "IX_BundgetSections_BudgetAreaID",
                table: "BundgetSections",
                column: "BudgetAreaID");

            migrationBuilder.CreateIndex(
                name: "IX_BundgetSections_PersonID",
                table: "BundgetSections",
                column: "PersonID");

            migrationBuilder.CreateIndex(
                name: "IX_People_CollectiveBudgetID",
                table: "People",
                column: "CollectiveBudgetID");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateBudgetSections_BudgetSectionID",
                table: "TemplateBudgetSections",
                column: "BudgetSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateBudgetSections_TemplateColumnID",
                table: "TemplateBudgetSections",
                column: "TemplateColumnID");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateColumns_TemplateID",
                table: "TemplateColumns",
                column: "TemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_PeriodTypeID",
                table: "Templates",
                column: "PeriodTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_PersonID",
                table: "Templates",
                column: "PersonID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetRecords");

            migrationBuilder.DropTable(
                name: "TemplateBudgetSections");

            migrationBuilder.DropTable(
                name: "BundgetSections");

            migrationBuilder.DropTable(
                name: "TemplateColumns");

            migrationBuilder.DropTable(
                name: "BudgetAreas");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.DropTable(
                name: "PeriodTypes");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "CollectiveBudgets");
        }
    }
}
