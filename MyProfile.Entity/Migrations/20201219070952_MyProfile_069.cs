using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_069 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserTags",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserTags");
        }
    }
}
