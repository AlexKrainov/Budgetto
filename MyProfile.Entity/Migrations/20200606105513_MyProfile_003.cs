using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_003 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLogs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IP = table.Column<string>(maxLength: 64, nullable: true),
                    City = table.Column<string>(maxLength: 32, nullable: true),
                    Country = table.Column<string>(maxLength: 32, nullable: true),
                    Location = table.Column<string>(maxLength: 64, nullable: true),
                    PostCode = table.Column<string>(maxLength: 16, nullable: true),
                    BrowerName = table.Column<string>(maxLength: 32, nullable: true),
                    BrowserVersion = table.Column<string>(maxLength: 16, nullable: true),
                    OS_Name = table.Column<string>(maxLength: 32, nullable: true),
                    Os_Version = table.Column<string>(maxLength: 16, nullable: true),
                    ScreenSize = table.Column<string>(maxLength: 16, nullable: true),
                    CurrentDateTime = table.Column<DateTime>(nullable: false),
                    ActionCodeName = table.Column<string>(maxLength: 16, nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    SessionID = table.Column<string>(maxLength: 32, nullable: true),
                    ParentUserLogID = table.Column<int>(nullable: true),
                    UserID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserLogs_UserLogs_ParentUserLogID",
                        column: x => x.ParentUserLogID,
                        principalTable: "UserLogs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserLogs_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLogs_ParentUserLogID",
                table: "UserLogs",
                column: "ParentUserLogID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogs_UserID",
                table: "UserLogs",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLogs");
        }
    }
}
