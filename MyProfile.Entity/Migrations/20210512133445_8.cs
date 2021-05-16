using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BaseAreaID",
                table: "Cards",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BaseSectionID",
                table: "BudgetSections",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BaseAreas",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    CodeName = table.Column<string>(maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseAreas", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BaseSections",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    CodeName = table.Column<string>(maxLength: 32, nullable: false),
                    KeyWords = table.Column<string>(nullable: false),
                    Color = table.Column<string>(maxLength: 32, nullable: true),
                    Icon = table.Column<string>(maxLength: 64, nullable: true),
                    BaseAreaID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseSections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BaseSections_BaseAreas_BaseAreaID",
                        column: x => x.BaseAreaID,
                        principalTable: "BaseAreas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_BaseAreaID",
                table: "Cards",
                column: "BaseAreaID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetSections_BaseSectionID",
                table: "BudgetSections",
                column: "BaseSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseSections_BaseAreaID",
                table: "BaseSections",
                column: "BaseAreaID");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetSections_BaseSections_BaseSectionID",
                table: "BudgetSections",
                column: "BaseSectionID",
                principalTable: "BaseSections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_BaseAreas_BaseAreaID",
                table: "Cards",
                column: "BaseAreaID",
                principalTable: "BaseAreas",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetSections_BaseSections_BaseSectionID",
                table: "BudgetSections");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_BaseAreas_BaseAreaID",
                table: "Cards");

            migrationBuilder.DropTable(
                name: "BaseSections");

            migrationBuilder.DropTable(
                name: "BaseAreas");

            migrationBuilder.DropIndex(
                name: "IX_Cards_BaseAreaID",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_BudgetSections_BaseSectionID",
                table: "BudgetSections");

            migrationBuilder.DropColumn(
                name: "BaseAreaID",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "BaseSectionID",
                table: "BudgetSections");
        }
    }
}
