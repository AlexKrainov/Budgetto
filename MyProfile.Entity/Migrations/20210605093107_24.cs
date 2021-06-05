using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _24 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AsPrepared",
                table: "Templates",
                newName: "IsCreatedByPrepared");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsCreatedByPrepared",
                table: "Templates",
                newName: "AsPrepared");
        }
    }
}
