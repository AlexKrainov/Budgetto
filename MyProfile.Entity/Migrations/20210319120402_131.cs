using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _131 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "AccountHistories",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Account2ID",
                table: "AccountHistories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Actions",
                table: "AccountHistories",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "AccountHistories",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewValue",
                table: "AccountHistories",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldValue",
                table: "AccountHistories",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Account2ID",
                table: "AccountHistories");

            migrationBuilder.DropColumn(
                name: "Actions",
                table: "AccountHistories");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "AccountHistories");

            migrationBuilder.DropColumn(
                name: "NewValue",
                table: "AccountHistories");

            migrationBuilder.DropColumn(
                name: "OldValue",
                table: "AccountHistories");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "AccountHistories",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128,
                oldNullable: true);
        }
    }
}
