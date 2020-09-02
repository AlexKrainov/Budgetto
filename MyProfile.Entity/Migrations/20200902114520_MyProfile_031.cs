using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_031 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpArticleUserViews_HelpArticles_HelpArticleID",
                table: "HelpArticleUserViews");

            migrationBuilder.AlterColumn<int>(
                name: "HelpArticleID",
                table: "HelpArticleUserViews",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HelpArticleUserViews_HelpArticles_HelpArticleID",
                table: "HelpArticleUserViews",
                column: "HelpArticleID",
                principalTable: "HelpArticles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpArticleUserViews_HelpArticles_HelpArticleID",
                table: "HelpArticleUserViews");

            migrationBuilder.AlterColumn<int>(
                name: "HelpArticleID",
                table: "HelpArticleUserViews",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_HelpArticleUserViews_HelpArticles_HelpArticleID",
                table: "HelpArticleUserViews",
                column: "HelpArticleID",
                principalTable: "HelpArticles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
