using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _161 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubScriptions_Users_UserID",
                table: "UserSubScriptions");

            migrationBuilder.RenameColumn(
                name: "endDate",
                table: "UserSubScriptions",
                newName: "EndDate");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserID",
                table: "UserSubScriptions",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubScriptions_Users_UserID",
                table: "UserSubScriptions",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubScriptions_Users_UserID",
                table: "UserSubScriptions");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "UserSubScriptions",
                newName: "endDate");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserID",
                table: "UserSubScriptions",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubScriptions_Users_UserID",
                table: "UserSubScriptions",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
