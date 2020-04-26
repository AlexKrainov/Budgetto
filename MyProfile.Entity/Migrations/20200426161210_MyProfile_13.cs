using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastSeenDate",
                table: "Templates",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<bool>(
                name: "IsAllowCollectiveBudget",
                table: "People",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SectionTypeCodeName",
                table: "BudgetSections",
                maxLength: 16,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Limits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Money = table.Column<decimal>(type: "Money", nullable: false),
                    PersonID = table.Column<Guid>(nullable: false),
                    PeriodTypeID = table.Column<int>(nullable: false),
                    BudgetAreaID = table.Column<int>(nullable: true),
                    BudgetSectionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Limits", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Limits_BudgetAreas_BudgetAreaID",
                        column: x => x.BudgetAreaID,
                        principalTable: "BudgetAreas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
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
                        name: "FK_Limits_People_PersonID",
                        column: x => x.PersonID,
                        principalTable: "People",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Limits_BudgetAreaID",
                table: "Limits",
                column: "BudgetAreaID");

            migrationBuilder.CreateIndex(
                name: "IX_Limits_BudgetSectionID",
                table: "Limits",
                column: "BudgetSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_Limits_PeriodTypeID",
                table: "Limits",
                column: "PeriodTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Limits_PersonID",
                table: "Limits",
                column: "PersonID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Limits");

            migrationBuilder.DropColumn(
                name: "IsAllowCollectiveBudget",
                table: "People");

            migrationBuilder.DropColumn(
                name: "SectionTypeCodeName",
                table: "BudgetSections");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastSeenDate",
                table: "Templates",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
