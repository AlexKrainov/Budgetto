using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_049 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShowInCollective",
                table: "Goals");

            migrationBuilder.AddColumn<bool>(
                name: "IsShowInCollective",
                table: "VisibleElements",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShowInCollective",
                table: "VisibleElements");

            migrationBuilder.AddColumn<bool>(
                name: "IsShowInCollective",
                table: "Goals",
                nullable: false,
                defaultValue: false);
        }
    }
}
