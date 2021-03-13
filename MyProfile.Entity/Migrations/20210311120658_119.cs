using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _119 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UTCOffset",
                table: "TimeZones",
                newName: "UTCOffsetHours");

            migrationBuilder.AddColumn<int>(
                name: "UTCOffsetMinutes",
                table: "TimeZones",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UTCOffsetMinutes",
                table: "TimeZones");

            migrationBuilder.RenameColumn(
                name: "UTCOffsetHours",
                table: "TimeZones",
                newName: "UTCOffset");
        }
    }
}
