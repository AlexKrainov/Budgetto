using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_057 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaName",
                table: "HelpArticles");

            migrationBuilder.AddColumn<int>(
                name: "HelpMenuID",
                table: "HelpArticles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "HelpMenus",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 16, nullable: false),
                    Icon = table.Column<string>(maxLength: 16, nullable: false),
                    IsVisible = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpMenus", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HelpArticles_HelpMenuID",
                table: "HelpArticles",
                column: "HelpMenuID");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpArticles_HelpMenus_HelpMenuID",
                table: "HelpArticles",
                column: "HelpMenuID",
                principalTable: "HelpMenus",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpArticles_HelpMenus_HelpMenuID",
                table: "HelpArticles");

            migrationBuilder.DropTable(
                name: "HelpMenus");

            migrationBuilder.DropIndex(
                name: "IX_HelpArticles_HelpMenuID",
                table: "HelpArticles");

            migrationBuilder.DropColumn(
                name: "HelpMenuID",
                table: "HelpArticles");

            migrationBuilder.AddColumn<string>(
                name: "AreaName",
                table: "HelpArticles",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }
    }
}
