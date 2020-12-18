using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_067 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreate",
                table: "UserTags",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 12, 14, 20, 18, 54, 372, DateTimeKind.Utc).AddTicks(9380));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreate",
                table: "Tags",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 12, 14, 20, 18, 54, 374, DateTimeKind.Utc).AddTicks(7029));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateSet",
                table: "RecordTags",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 12, 14, 20, 18, 54, 374, DateTimeKind.Utc).AddTicks(8673));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreate",
                table: "UserTags",
                nullable: false,
                defaultValue: new DateTime(2020, 12, 14, 20, 18, 54, 372, DateTimeKind.Utc).AddTicks(9380),
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreate",
                table: "Tags",
                nullable: false,
                defaultValue: new DateTime(2020, 12, 14, 20, 18, 54, 374, DateTimeKind.Utc).AddTicks(7029),
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateSet",
                table: "RecordTags",
                nullable: false,
                defaultValue: new DateTime(2020, 12, 14, 20, 18, 54, 374, DateTimeKind.Utc).AddTicks(8673),
                oldClrType: typeof(DateTime));
        }
    }
}
