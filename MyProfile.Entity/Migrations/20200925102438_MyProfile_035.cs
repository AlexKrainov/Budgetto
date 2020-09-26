using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_035 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_UserID",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Payments_PaymentID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PaymentID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Payments_UserID",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Payments");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentID",
                table: "Users",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PaymentID",
                table: "Users",
                column: "PaymentID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Payments_PaymentID",
                table: "Users",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Payments_PaymentID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PaymentID",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentID",
                table: "Users",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "Payments",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Users_PaymentID",
                table: "Users",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserID",
                table: "Payments",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_UserID",
                table: "Payments",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Payments_PaymentID",
                table: "Users",
                column: "PaymentID",
                principalTable: "Payments",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
