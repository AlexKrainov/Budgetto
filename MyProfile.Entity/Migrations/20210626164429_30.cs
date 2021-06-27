using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _30 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Year_Accounts",
                table: "UserSettings",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Month_Accounts",
                table: "UserSettings",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Year_Accounts",
                table: "UserSettings",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Month_Accounts",
                table: "UserSettings",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldDefaultValue: false);
        }
    }
}
