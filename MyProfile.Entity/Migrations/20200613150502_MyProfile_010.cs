using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_010 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectionGroupLimits_Goals_GoalID",
                table: "SectionGroupLimits");

            migrationBuilder.DropIndex(
                name: "IX_SectionGroupLimits_GoalID",
                table: "SectionGroupLimits");

            migrationBuilder.DropColumn(
                name: "GoalID",
                table: "SectionGroupLimits");

            migrationBuilder.RenameColumn(
                name: "TotalMoney",
                table: "Goals",
                newName: "ExpectationMoney");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpectationMoney",
                table: "Goals",
                newName: "TotalMoney");

            migrationBuilder.AddColumn<int>(
                name: "GoalID",
                table: "SectionGroupLimits",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SectionGroupLimits_GoalID",
                table: "SectionGroupLimits",
                column: "GoalID");

            migrationBuilder.AddForeignKey(
                name: "FK_SectionGroupLimits_Goals_GoalID",
                table: "SectionGroupLimits",
                column: "GoalID",
                principalTable: "Goals",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
