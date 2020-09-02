using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_028 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HelpArticles",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 512, nullable: false),
                    IsVisible = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateEdit = table.Column<DateTime>(nullable: false),
                    Link = table.Column<string>(maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpArticles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "HelpArticleUserViews",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateView = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<Guid>(nullable: true),
                    HelpArticleID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpArticleUserViews", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HelpArticleUserViews_HelpArticles_HelpArticleID",
                        column: x => x.HelpArticleID,
                        principalTable: "HelpArticles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HelpArticleUserViews_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HelpArticleUserViews_HelpArticleID",
                table: "HelpArticleUserViews",
                column: "HelpArticleID");

            migrationBuilder.CreateIndex(
                name: "IX_HelpArticleUserViews_UserID",
                table: "HelpArticleUserViews",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HelpArticleUserViews");

            migrationBuilder.DropTable(
                name: "HelpArticles");
        }
    }
}
