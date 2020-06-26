using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Charts_PeriodTypes_PeriodTypeID",
                table: "Charts");

            migrationBuilder.DropForeignKey(
                name: "FK_SectionGroupCharts_ChartFields_PartChartID",
                table: "SectionGroupCharts");

            migrationBuilder.RenameColumn(
                name: "PartChartID",
                table: "SectionGroupCharts",
                newName: "ChartFieldID");

            migrationBuilder.RenameIndex(
                name: "IX_SectionGroupCharts_PartChartID",
                table: "SectionGroupCharts",
                newName: "IX_SectionGroupCharts_ChartFieldID");

            migrationBuilder.AlterColumn<bool>(
                name: "BudgetPages_IsShow_BigCharts",
                table: "UserSettings",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<int>(
                name: "PeriodTypeID",
                table: "Charts",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Charts_PeriodTypes_PeriodTypeID",
                table: "Charts",
                column: "PeriodTypeID",
                principalTable: "PeriodTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SectionGroupCharts_ChartFields_ChartFieldID",
                table: "SectionGroupCharts",
                column: "ChartFieldID",
                principalTable: "ChartFields",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Charts_PeriodTypes_PeriodTypeID",
                table: "Charts");

            migrationBuilder.DropForeignKey(
                name: "FK_SectionGroupCharts_ChartFields_ChartFieldID",
                table: "SectionGroupCharts");

            migrationBuilder.RenameColumn(
                name: "ChartFieldID",
                table: "SectionGroupCharts",
                newName: "PartChartID");

            migrationBuilder.RenameIndex(
                name: "IX_SectionGroupCharts_ChartFieldID",
                table: "SectionGroupCharts",
                newName: "IX_SectionGroupCharts_PartChartID");

            migrationBuilder.AlterColumn<bool>(
                name: "BudgetPages_IsShow_BigCharts",
                table: "UserSettings",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<int>(
                name: "PeriodTypeID",
                table: "Charts",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Charts_PeriodTypes_PeriodTypeID",
                table: "Charts",
                column: "PeriodTypeID",
                principalTable: "PeriodTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SectionGroupCharts_ChartFields_PartChartID",
                table: "SectionGroupCharts",
                column: "PartChartID",
                principalTable: "ChartFields",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
