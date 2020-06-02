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
                    CodeName = table.Column<string>(nullable: false),
                    IsUsing = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SectionTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 16, nullable: false),
                    CodeName = table.Column<string>(maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
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
                    IsAllowCollectiveBudget = table.Column<bool>(nullable: false),
                    CollectiveBudgetID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Users_CollectiveBudgets_CollectiveBudgetID",
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
                    IsShow = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CssIcon = table.Column<string>(maxLength: 64, nullable: true),
                    IsPrivate = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAreas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BudgetAreas_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonSettings",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    SpecificCulture = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonSettings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PersonSettings_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionTypeViews",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsShow = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    PeriodTypeID = table.Column<int>(nullable: false),
                    SectionTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionTypeViews", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SectionTypeViews_PeriodTypes_PeriodTypeID",
                        column: x => x.PeriodTypeID,
                        principalTable: "PeriodTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionTypeViews_SectionTypes_SectionTypeID",
                        column: x => x.SectionTypeID,
                        principalTable: "SectionTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionTypeViews_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CurrentPeriod = table.Column<string>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateEdit = table.Column<DateTime>(nullable: false),
                    LastSeenDate = table.Column<DateTime>(nullable: true),
                    DateDelete = table.Column<DateTime>(nullable: true),
                    IsCountCollectiveBudget = table.Column<bool>(nullable: false),
                    MaxRowInAPage = table.Column<int>(nullable: false),
                    IsDelete = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
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
                        name: "FK_Templates_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_ID",
                        column: x => x.ID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetSections",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    CodeName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CssIcon = table.Column<string>(maxLength: 64, nullable: true),
                    CssColor = table.Column<string>(maxLength: 24, nullable: true),
                    IsShow = table.Column<bool>(nullable: false),
                    IsPrivate = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: true),
                    BudgetAreaID = table.Column<int>(nullable: false),
                    SectionTypeID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetSections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BudgetSections_BudgetAreas_BudgetAreaID",
                        column: x => x.BudgetAreaID,
                        principalTable: "BudgetAreas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetSections_SectionTypes_SectionTypeID",
                        column: x => x.SectionTypeID,
                        principalTable: "SectionTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetSections_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CollectiveAreas",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AreaID = table.Column<int>(nullable: true),
                    ChildAreaID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectiveAreas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CollectiveAreas_BudgetAreas_AreaID",
                        column: x => x.AreaID,
                        principalTable: "BudgetAreas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CollectiveAreas_BudgetAreas_ChildAreaID",
                        column: x => x.ChildAreaID,
                        principalTable: "BudgetAreas",
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
                    ColumnTypeID = table.Column<int>(nullable: false),
                    Format = table.Column<string>(nullable: true),
                    FooterActionTypeID = table.Column<int>(nullable: true),
                    PlaceAfterCommon = table.Column<int>(nullable: true),
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
                    RawData = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DateTimeOfPayment = table.Column<DateTime>(nullable: false),
                    DateTimeCreate = table.Column<DateTime>(nullable: false),
                    DateTimeEdit = table.Column<DateTime>(nullable: false),
                    DateTimeDelete = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsHide = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    BudgetSectionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetRecords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BudgetRecords_BudgetSections_BudgetSectionID",
                        column: x => x.BudgetSectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetRecords_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectiveSections",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SectionID = table.Column<int>(nullable: true),
                    ChildSectionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectiveSections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CollectiveSections_BudgetSections_ChildSectionID",
                        column: x => x.ChildSectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CollectiveSections_BudgetSections_SectionID",
                        column: x => x.SectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Limits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    LimitMoney = table.Column<decimal>(type: "Money", nullable: false),
                    DateStart = table.Column<DateTime>(nullable: true),
                    DateEnd = table.Column<DateTime>(nullable: true),
                    IsShow = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    PeriodTypeID = table.Column<int>(nullable: false),
                    BudgetSectionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Limits", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Limits_BudgetSections_BudgetSectionID",
                        column: x => x.BudgetSectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Limits_PeriodTypes_PeriodTypeID",
                        column: x => x.PeriodTypeID,
                        principalTable: "PeriodTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Limits_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
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
                        name: "FK_TemplateBudgetSections_BudgetSections_BudgetSectionID",
                        column: x => x.BudgetSectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateBudgetSections_TemplateColumns_TemplateColumnID",
                        column: x => x.TemplateColumnID,
                        principalTable: "TemplateColumns",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionGroupLimits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LimitID = table.Column<int>(nullable: false),
                    BudgetSectionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionGroupLimits", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SectionGroupLimits_BudgetSections_BudgetSectionID",
                        column: x => x.BudgetSectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SectionGroupLimits_Limits_LimitID",
                        column: x => x.LimitID,
                        principalTable: "Limits",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAreas_UserID",
                table: "BudgetAreas",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRecords_BudgetSectionID",
                table: "BudgetRecords",
                column: "BudgetSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRecords_DateTimeOfPayment",
                table: "BudgetRecords",
                column: "DateTimeOfPayment");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRecords_UserID",
                table: "BudgetRecords",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetSections_BudgetAreaID",
                table: "BudgetSections",
                column: "BudgetAreaID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetSections_SectionTypeID",
                table: "BudgetSections",
                column: "SectionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetSections_UserID",
                table: "BudgetSections",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveAreas_AreaID",
                table: "CollectiveAreas",
                column: "AreaID");

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveAreas_ChildAreaID",
                table: "CollectiveAreas",
                column: "ChildAreaID");

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveSections_ChildSectionID",
                table: "CollectiveSections",
                column: "ChildSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveSections_SectionID",
                table: "CollectiveSections",
                column: "SectionID");

            migrationBuilder.CreateIndex(
                name: "IX_Limits_BudgetSectionID",
                table: "Limits",
                column: "BudgetSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_Limits_PeriodTypeID",
                table: "Limits",
                column: "PeriodTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Limits_UserID",
                table: "Limits",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonSettings_UserID",
                table: "PersonSettings",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionGroupLimits_BudgetSectionID",
                table: "SectionGroupLimits",
                column: "BudgetSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionGroupLimits_LimitID",
                table: "SectionGroupLimits",
                column: "LimitID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionTypeViews_PeriodTypeID",
                table: "SectionTypeViews",
                column: "PeriodTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionTypeViews_SectionTypeID",
                table: "SectionTypeViews",
                column: "SectionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionTypeViews_UserID",
                table: "SectionTypeViews",
                column: "UserID");

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
                name: "IX_Templates_UserID",
                table: "Templates",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CollectiveBudgetID",
                table: "Users",
                column: "CollectiveBudgetID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetRecords");

            migrationBuilder.DropTable(
                name: "CollectiveAreas");

            migrationBuilder.DropTable(
                name: "CollectiveSections");

            migrationBuilder.DropTable(
                name: "PersonSettings");

            migrationBuilder.DropTable(
                name: "SectionGroupLimits");

            migrationBuilder.DropTable(
                name: "SectionTypeViews");

            migrationBuilder.DropTable(
                name: "TemplateBudgetSections");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "Limits");

            migrationBuilder.DropTable(
                name: "TemplateColumns");

            migrationBuilder.DropTable(
                name: "BudgetSections");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.DropTable(
                name: "BudgetAreas");

            migrationBuilder.DropTable(
                name: "SectionTypes");

            migrationBuilder.DropTable(
                name: "PeriodTypes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "CollectiveBudgets");
        }
    }
}
