using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_066 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "UserTags",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Tags",
                newName: "Title");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreate",
                table: "UserTags",
                nullable: false,
                defaultValue: new DateTime(2020, 12, 14, 20, 18, 54, 372, DateTimeKind.Utc).AddTicks(9380),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 12, 14, 14, 45, 38, 710, DateTimeKind.Utc).AddTicks(6609));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreate",
                table: "Tags",
                nullable: false,
                defaultValue: new DateTime(2020, 12, 14, 20, 18, 54, 374, DateTimeKind.Utc).AddTicks(7029),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 12, 14, 14, 45, 38, 722, DateTimeKind.Utc).AddTicks(3269));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateSet",
                table: "RecordTags",
                nullable: false,
                defaultValue: new DateTime(2020, 12, 14, 20, 18, 54, 374, DateTimeKind.Utc).AddTicks(8673),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 12, 14, 14, 45, 38, 722, DateTimeKind.Utc).AddTicks(5425));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "UserTags",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Tags",
                newName: "Name");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreate",
                table: "UserTags",
                nullable: false,
                defaultValue: new DateTime(2020, 12, 14, 14, 45, 38, 710, DateTimeKind.Utc).AddTicks(6609),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 12, 14, 20, 18, 54, 372, DateTimeKind.Utc).AddTicks(9380));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreate",
                table: "Tags",
                nullable: false,
                defaultValue: new DateTime(2020, 12, 14, 14, 45, 38, 722, DateTimeKind.Utc).AddTicks(3269),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 12, 14, 20, 18, 54, 374, DateTimeKind.Utc).AddTicks(7029));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateSet",
                table: "RecordTags",
                nullable: false,
                defaultValue: new DateTime(2020, 12, 14, 14, 45, 38, 722, DateTimeKind.Utc).AddTicks(5425),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 12, 14, 20, 18, 54, 374, DateTimeKind.Utc).AddTicks(8673));
        }
    }
}
