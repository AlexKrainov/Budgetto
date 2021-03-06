using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserConnects",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    TelegramLogin = table.Column<string>(maxLength: 64, nullable: true),
                    TelegramKey = table.Column<string>(maxLength: 256, nullable: true),
                    UserConnectID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConnects", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserConnects_Users_ID",
                        column: x => x.ID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserConnects_UserConnects_UserConnectID",
                        column: x => x.UserConnectID,
                        principalTable: "UserConnects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HubConnects",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateConnect = table.Column<DateTime>(nullable: false),
                    ConnectionID = table.Column<string>(maxLength: 128, nullable: true),
                    UserConnectID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HubConnects", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HubConnects_UserConnects_UserConnectID",
                        column: x => x.UserConnectID,
                        principalTable: "UserConnects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HubConnects_UserConnectID",
                table: "HubConnects",
                column: "UserConnectID");

            migrationBuilder.CreateIndex(
                name: "IX_UserConnects_UserConnectID",
                table: "UserConnects",
                column: "UserConnectID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HubConnects");

            migrationBuilder.DropTable(
                name: "UserConnects");
        }
    }
}
