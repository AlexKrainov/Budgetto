using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_CollectiveBudgets_CollectiveBudgetID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CollectiveBudgetID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CollectiveBudgetID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DateCreate",
                table: "CollectiveBudgets");

            migrationBuilder.DropColumn(
                name: "DateDelete",
                table: "CollectiveBudgets");

            migrationBuilder.CreateTable(
                name: "CollectiveBudgetRequestOwners",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectiveBudgetRequestOwners", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CollectiveBudgetRequestOwners_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectiveBudgetUsers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<string>(maxLength: 16, nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    DateUpdate = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    CollectiveBudgetID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectiveBudgetUsers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CollectiveBudgetUsers_CollectiveBudgets_CollectiveBudgetID",
                        column: x => x.CollectiveBudgetID,
                        principalTable: "CollectiveBudgets",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectiveBudgetUsers_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectiveBudgetRequests",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<string>(maxLength: 16, nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    DateUpdate = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: true),
                    CollectiveBudgetRequestOwnerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectiveBudgetRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CollectiveBudgetRequests_CollectiveBudgetRequestOwners_CollectiveBudgetRequestOwnerID",
                        column: x => x.CollectiveBudgetRequestOwnerID,
                        principalTable: "CollectiveBudgetRequestOwners",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectiveBudgetRequests_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveBudgetRequestOwners_UserID",
                table: "CollectiveBudgetRequestOwners",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveBudgetRequests_CollectiveBudgetRequestOwnerID",
                table: "CollectiveBudgetRequests",
                column: "CollectiveBudgetRequestOwnerID");

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveBudgetRequests_UserID",
                table: "CollectiveBudgetRequests",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveBudgetUsers_CollectiveBudgetID",
                table: "CollectiveBudgetUsers",
                column: "CollectiveBudgetID");

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveBudgetUsers_UserID",
                table: "CollectiveBudgetUsers",
                column: "UserID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectiveBudgetRequests");

            migrationBuilder.DropTable(
                name: "CollectiveBudgetUsers");

            migrationBuilder.DropTable(
                name: "CollectiveBudgetRequestOwners");

            migrationBuilder.AddColumn<Guid>(
                name: "CollectiveBudgetID",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreate",
                table: "CollectiveBudgets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDelete",
                table: "CollectiveBudgets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CollectiveBudgetID",
                table: "Users",
                column: "CollectiveBudgetID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_CollectiveBudgets_CollectiveBudgetID",
                table: "Users",
                column: "CollectiveBudgetID",
                principalTable: "CollectiveBudgets",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
