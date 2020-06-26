using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Charts_PeriodTypes_PeriodTypeID",
                table: "Charts");

            migrationBuilder.DropIndex(
                name: "IX_Charts_PeriodTypeID",
                table: "Charts");

            migrationBuilder.DropColumn(
                name: "PeriodTypeID",
                table: "Charts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PeriodTypeID",
                table: "Charts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Charts_PeriodTypeID",
                table: "Charts",
                column: "PeriodTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Charts_PeriodTypes_PeriodTypeID",
                table: "Charts",
                column: "PeriodTypeID",
                principalTable: "PeriodTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
