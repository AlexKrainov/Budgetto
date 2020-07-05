using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_036 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CBR_NameCurrency",
                table: "Currencies",
                newName: "CodeName_CBR");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CodeName_CBR",
                table: "Currencies",
                newName: "CBR_NameCurrency");
        }
    }
}
