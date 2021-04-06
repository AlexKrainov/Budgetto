using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _151 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CapitalizationOfDeposit",
                table: "AccountInfos",
                newName: "CapitalizationTimeListID");

            migrationBuilder.AddColumn<bool>(
                name: "IsCapitalization",
                table: "AccountInfos",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCapitalization",
                table: "AccountInfos");

            migrationBuilder.RenameColumn(
                name: "CapitalizationTimeListID",
                table: "AccountInfos",
                newName: "CapitalizationOfDeposit");
        }
    }
}
