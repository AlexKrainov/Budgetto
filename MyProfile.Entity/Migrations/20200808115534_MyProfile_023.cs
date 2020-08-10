using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_023 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoLists_Users_UserID",
                table: "ToDoLists");

            migrationBuilder.DropIndex(
                name: "IX_ToDoLists_UserID",
                table: "ToDoLists");

            migrationBuilder.DropColumn(
                name: "CssIcon",
                table: "ToDoLists");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "ToDoLists");

            migrationBuilder.AddColumn<int>(
                name: "ToDoListFolderID",
                table: "ToDoLists",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ToDoListFolders",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    CssIcon = table.Column<string>(maxLength: 32, nullable: true),
                    UserID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoListFolders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ToDoListFolders_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToDoLists_ToDoListFolderID",
                table: "ToDoLists",
                column: "ToDoListFolderID");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoListFolders_UserID",
                table: "ToDoListFolders",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoLists_ToDoListFolders_ToDoListFolderID",
                table: "ToDoLists",
                column: "ToDoListFolderID",
                principalTable: "ToDoListFolders",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoLists_ToDoListFolders_ToDoListFolderID",
                table: "ToDoLists");

            migrationBuilder.DropTable(
                name: "ToDoListFolders");

            migrationBuilder.DropIndex(
                name: "IX_ToDoLists_ToDoListFolderID",
                table: "ToDoLists");

            migrationBuilder.DropColumn(
                name: "ToDoListFolderID",
                table: "ToDoLists");

            migrationBuilder.AddColumn<string>(
                name: "CssIcon",
                table: "ToDoLists",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "ToDoLists",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ToDoLists_UserID",
                table: "ToDoLists",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoLists_Users_UserID",
                table: "ToDoLists",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
