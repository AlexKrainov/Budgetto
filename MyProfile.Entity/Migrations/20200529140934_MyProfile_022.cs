using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SectionTypeID",
                table: "BudgetSections",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SectionGroupLimits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LimitID = table.Column<int>(nullable: false),
                    BudgetSectionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionGroupLimits", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SectionGroupLimits_BudgetSections_BudgetSectionID",
                        column: x => x.BudgetSectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SectionGroupLimits_Limits_LimitID",
                        column: x => x.LimitID,
                        principalTable: "Limits",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 16, nullable: false),
                    CodeName = table.Column<string>(maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SectionTypeViews",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsShow = table.Column<bool>(nullable: false),
                    PersonID = table.Column<Guid>(nullable: false),
                    PeriodTypeID = table.Column<int>(nullable: false),
                    SectionTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionTypeViews", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SectionTypeViews_PeriodTypes_PeriodTypeID",
                        column: x => x.PeriodTypeID,
                        principalTable: "PeriodTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionTypeViews_People_PersonID",
                        column: x => x.PersonID,
                        principalTable: "People",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionTypeViews_SectionTypes_SectionTypeID",
                        column: x => x.SectionTypeID,
                        principalTable: "SectionTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetSections_SectionTypeID",
                table: "BudgetSections",
                column: "SectionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionGroupLimits_BudgetSectionID",
                table: "SectionGroupLimits",
                column: "BudgetSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionGroupLimits_LimitID",
                table: "SectionGroupLimits",
                column: "LimitID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionTypeViews_PeriodTypeID",
                table: "SectionTypeViews",
                column: "PeriodTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionTypeViews_PersonID",
                table: "SectionTypeViews",
                column: "PersonID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionTypeViews_SectionTypeID",
                table: "SectionTypeViews",
                column: "SectionTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetSections_SectionTypes_SectionTypeID",
                table: "BudgetSections",
                column: "SectionTypeID",
                principalTable: "SectionTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetSections_SectionTypes_SectionTypeID",
                table: "BudgetSections");

            migrationBuilder.DropTable(
                name: "SectionGroupLimits");

            migrationBuilder.DropTable(
                name: "SectionTypeViews");

            migrationBuilder.DropTable(
                name: "SectionTypes");

            migrationBuilder.DropIndex(
                name: "IX_BudgetSections_SectionTypeID",
                table: "BudgetSections");

            migrationBuilder.DropColumn(
                name: "SectionTypeID",
                table: "BudgetSections");
        }
    }
}
