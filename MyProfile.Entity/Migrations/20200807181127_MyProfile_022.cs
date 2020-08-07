using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ToDoLists",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateEdit = table.Column<DateTime>(nullable: false),
                    CssIcon = table.Column<string>(maxLength: 32, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    PeriodTypeID = table.Column<int>(nullable: false),
                    VisibleElementID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoLists", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ToDoLists_PeriodTypes_PeriodTypeID",
                        column: x => x.PeriodTypeID,
                        principalTable: "PeriodTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToDoLists_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToDoLists_VisibleElements_VisibleElementID",
                        column: x => x.VisibleElementID,
                        principalTable: "VisibleElements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToDoListItems",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateEdit = table.Column<DateTime>(nullable: false),
                    IsDone = table.Column<bool>(nullable: false),
                    ToDoListID = table.Column<int>(nullable: false),
                    OwnerUserID = table.Column<Guid>(nullable: true),
                    DoneUserID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoListItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ToDoListItems_Users_DoneUserID",
                        column: x => x.DoneUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToDoListItems_Users_OwnerUserID",
                        column: x => x.OwnerUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToDoListItems_ToDoLists_ToDoListID",
                        column: x => x.ToDoListID,
                        principalTable: "ToDoLists",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToDoListItems_DoneUserID",
                table: "ToDoListItems",
                column: "DoneUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoListItems_OwnerUserID",
                table: "ToDoListItems",
                column: "OwnerUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoListItems_ToDoListID",
                table: "ToDoListItems",
                column: "ToDoListID");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoLists_PeriodTypeID",
                table: "ToDoLists",
                column: "PeriodTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoLists_UserID",
                table: "ToDoLists",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoLists_VisibleElementID",
                table: "ToDoLists",
                column: "VisibleElementID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToDoListItems");

            migrationBuilder.DropTable(
                name: "ToDoLists");
        }
    }
}
