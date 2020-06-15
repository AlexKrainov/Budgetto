using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_013 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "BudgetPages_IsShow_Goals",
                table: "UserSettings",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool));

            migrationBuilder.CreateTable(
                name: "ChartTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 16, nullable: false),
                    CodeName = table.Column<string>(maxLength: 16, nullable: false),
                    IsUsing = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Charts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    LastDateEdit = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    PeriodTypeID = table.Column<int>(nullable: false),
                    ChartTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Charts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Charts_ChartTypes_ChartTypeID",
                        column: x => x.ChartTypeID,
                        principalTable: "ChartTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Charts_PeriodTypes_PeriodTypeID",
                        column: x => x.PeriodTypeID,
                        principalTable: "PeriodTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Charts_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartCharts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    CssColor = table.Column<string>(maxLength: 24, nullable: true),
                    ChartID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartCharts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PartCharts_Charts_ChartID",
                        column: x => x.ChartID,
                        principalTable: "Charts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionGroupCharts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PartChartID = table.Column<int>(nullable: false),
                    BudgetSectionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionGroupCharts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SectionGroupCharts_BudgetSections_BudgetSectionID",
                        column: x => x.BudgetSectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionGroupCharts_PartCharts_PartChartID",
                        column: x => x.PartChartID,
                        principalTable: "PartCharts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Charts_ChartTypeID",
                table: "Charts",
                column: "ChartTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Charts_PeriodTypeID",
                table: "Charts",
                column: "PeriodTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Charts_UserID",
                table: "Charts",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_PartCharts_ChartID",
                table: "PartCharts",
                column: "ChartID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionGroupCharts_BudgetSectionID",
                table: "SectionGroupCharts",
                column: "BudgetSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionGroupCharts_PartChartID",
                table: "SectionGroupCharts",
                column: "PartChartID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SectionGroupCharts");

            migrationBuilder.DropTable(
                name: "PartCharts");

            migrationBuilder.DropTable(
                name: "Charts");

            migrationBuilder.DropTable(
                name: "ChartTypes");

            migrationBuilder.AlterColumn<bool>(
                name: "BudgetPages_IsShow_Goals",
                table: "UserSettings",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);
        }
    }
}
