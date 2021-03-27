using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _142 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_PaymentSystems_PaymentSystemID",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Cards_PaymentSystemID",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "PaymentSystemID",
                table: "Cards");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentSystemID",
                table: "Cards",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cards_PaymentSystemID",
                table: "Cards",
                column: "PaymentSystemID");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_PaymentSystems_PaymentSystemID",
                table: "Cards",
                column: "PaymentSystemID",
                principalTable: "PaymentSystems",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
