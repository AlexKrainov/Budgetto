using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetRecords_BundgetSections_BudgetSectionID",
                table: "BudgetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_TemplateBudgetSections_BundgetSections_BudgetSectionID",
                table: "TemplateBudgetSections");

            migrationBuilder.DropTable(
                name: "BundgetSections");

            migrationBuilder.CreateTable(
                name: "BudgetSections",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    CodeName = table.Column<string>(nullable: true),
                    Type_RecordType = table.Column<string>(nullable: true),
                    CssIcon = table.Column<string>(nullable: true),
                    PersonID = table.Column<Guid>(nullable: true),
                    BudgetAreaID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetSections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BudgetSections_BudgetAreas_BudgetAreaID",
                        column: x => x.BudgetAreaID,
                        principalTable: "BudgetAreas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetSections_People_PersonID",
                        column: x => x.PersonID,
                        principalTable: "People",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetSections_BudgetAreaID",
                table: "BudgetSections",
                column: "BudgetAreaID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetSections_PersonID",
                table: "BudgetSections",
                column: "PersonID");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetRecords_BudgetSections_BudgetSectionID",
                table: "BudgetRecords",
                column: "BudgetSectionID",
                principalTable: "BudgetSections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateBudgetSections_BudgetSections_BudgetSectionID",
                table: "TemplateBudgetSections",
                column: "BudgetSectionID",
                principalTable: "BudgetSections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetRecords_BudgetSections_BudgetSectionID",
                table: "BudgetRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_TemplateBudgetSections_BudgetSections_BudgetSectionID",
                table: "TemplateBudgetSections");

            migrationBuilder.DropTable(
                name: "BudgetSections");

            migrationBuilder.CreateTable(
                name: "BundgetSections",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BudgetAreaID = table.Column<int>(nullable: false),
                    CodeName = table.Column<string>(nullable: true),
                    CssIcon = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    PersonID = table.Column<Guid>(nullable: true),
                    Type_RecordType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BundgetSections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BundgetSections_BudgetAreas_BudgetAreaID",
                        column: x => x.BudgetAreaID,
                        principalTable: "BudgetAreas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BundgetSections_People_PersonID",
                        column: x => x.PersonID,
                        principalTable: "People",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BundgetSections_BudgetAreaID",
                table: "BundgetSections",
                column: "BudgetAreaID");

            migrationBuilder.CreateIndex(
                name: "IX_BundgetSections_PersonID",
                table: "BundgetSections",
                column: "PersonID");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetRecords_BundgetSections_BudgetSectionID",
                table: "BudgetRecords",
                column: "BudgetSectionID",
                principalTable: "BundgetSections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateBudgetSections_BundgetSections_BudgetSectionID",
                table: "TemplateBudgetSections",
                column: "BudgetSectionID",
                principalTable: "BundgetSections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
