using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _154 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoLists_PeriodTypes_PeriodTypeID",
                table: "ToDoLists");

            migrationBuilder.DropIndex(
                name: "IX_ToDoLists_PeriodTypeID",
                table: "ToDoLists");

            migrationBuilder.DropColumn(
                name: "PeriodTypeID",
                table: "ToDoLists");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PeriodTypeID",
                table: "ToDoLists",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ToDoLists_PeriodTypeID",
                table: "ToDoLists",
                column: "PeriodTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoLists_PeriodTypes_PeriodTypeID",
                table: "ToDoLists",
                column: "PeriodTypeID",
                principalTable: "PeriodTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
