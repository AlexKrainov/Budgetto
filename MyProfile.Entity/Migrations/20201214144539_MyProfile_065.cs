using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_065 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreate",
                table: "UserTags",
                nullable: false,
                defaultValue: new DateTime(2020, 12, 14, 14, 45, 38, 710, DateTimeKind.Utc).AddTicks(6609),
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreate",
                table: "Tags",
                nullable: false,
                defaultValue: new DateTime(2020, 12, 14, 14, 45, 38, 722, DateTimeKind.Utc).AddTicks(3269),
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateSet",
                table: "RecordTags",
                nullable: false,
                defaultValue: new DateTime(2020, 12, 14, 14, 45, 38, 722, DateTimeKind.Utc).AddTicks(5425));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateSet",
                table: "RecordTags");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreate",
                table: "UserTags",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 12, 14, 14, 45, 38, 710, DateTimeKind.Utc).AddTicks(6609));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreate",
                table: "Tags",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 12, 14, 14, 45, 38, 722, DateTimeKind.Utc).AddTicks(3269));
        }
    }
}
