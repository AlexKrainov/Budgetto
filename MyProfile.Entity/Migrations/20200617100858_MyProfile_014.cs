using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_014 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectionGroupCharts_PartCharts_PartChartID",
                table: "SectionGroupCharts");

            migrationBuilder.DropTable(
                name: "PartCharts");

            migrationBuilder.CreateTable(
                name: "ChartFields",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    CssColor = table.Column<string>(maxLength: 24, nullable: true),
                    ChartID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartFields", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChartFields_Charts_ChartID",
                        column: x => x.ChartID,
                        principalTable: "Charts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChartFields_ChartID",
                table: "ChartFields",
                column: "ChartID");

            migrationBuilder.AddForeignKey(
                name: "FK_SectionGroupCharts_ChartFields_PartChartID",
                table: "SectionGroupCharts",
                column: "PartChartID",
                principalTable: "ChartFields",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectionGroupCharts_ChartFields_PartChartID",
                table: "SectionGroupCharts");

            migrationBuilder.DropTable(
                name: "ChartFields");

            migrationBuilder.CreateTable(
                name: "PartCharts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChartID = table.Column<int>(nullable: false),
                    CssColor = table.Column<string>(maxLength: 24, nullable: true),
                    Name = table.Column<string>(maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartCharts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PartCharts_Charts_ChartID",
                        column: x => x.ChartID,
                        principalTable: "Charts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartCharts_ChartID",
                table: "PartCharts",
                column: "ChartID");

            migrationBuilder.AddForeignKey(
                name: "FK_SectionGroupCharts_PartCharts_PartChartID",
                table: "SectionGroupCharts",
                column: "PartChartID",
                principalTable: "PartCharts",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
