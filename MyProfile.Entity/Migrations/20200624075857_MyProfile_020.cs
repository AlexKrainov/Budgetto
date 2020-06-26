using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmEmail",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "MailTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodeName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MailLogs",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    SentDateTime = table.Column<DateTime>(nullable: false),
                    CameDateTime = table.Column<DateTime>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    UserID = table.Column<Guid>(nullable: true),
                    MailTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MailLogs_MailTypes_MailTypeID",
                        column: x => x.MailTypeID,
                        principalTable: "MailTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MailLogs_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MailLogs_MailTypeID",
                table: "MailLogs",
                column: "MailTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_MailLogs_UserID",
                table: "MailLogs",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MailLogs");

            migrationBuilder.DropTable(
                name: "MailTypes");

            migrationBuilder.DropColumn(
                name: "IsConfirmEmail",
                table: "Users");
        }
    }
}
