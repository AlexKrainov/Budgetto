using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyPofile_13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_ChartTypes_ChartTypeID",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_UserID",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_Users_UserID",
                table: "ChatUsers");

            migrationBuilder.DropIndex(
                name: "IX_Chats_ChartTypeID",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_UserID",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "ChartTypeID",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "VisibleElementID",
                table: "Chats");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserID",
                table: "ChatUsers",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Left",
                table: "ChatUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_Users_UserID",
                table: "ChatUsers",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_Users_UserID",
                table: "ChatUsers");

            migrationBuilder.DropColumn(
                name: "Left",
                table: "ChatUsers");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserID",
                table: "ChatUsers",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<int>(
                name: "ChartTypeID",
                table: "Chats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "Chats",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "VisibleElementID",
                table: "Chats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ChartTypeID",
                table: "Chats",
                column: "ChartTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UserID",
                table: "Chats",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_ChartTypes_ChartTypeID",
                table: "Chats",
                column: "ChartTypeID",
                principalTable: "ChartTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_UserID",
                table: "Chats",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_Users_UserID",
                table: "ChatUsers",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
