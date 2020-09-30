using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_043 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WebSiteTheme_CodeName",
                table: "UserSettings",
                newName: "WebSiteTheme");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WebSiteTheme",
                table: "UserSettings",
                newName: "WebSiteTheme_CodeName");
        }
    }
}
