using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _087 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Summaries",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    CodeName = table.Column<string>(maxLength: 64, nullable: false),
                    CurrentDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsChart = table.Column<bool>(nullable: false),
                    VisibleElementID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Summaries", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Summaries_VisibleElements_VisibleElementID",
                        column: x => x.VisibleElementID,
                        principalTable: "VisibleElements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSummaries",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Value = table.Column<string>(nullable: true),
                    CurrentDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsChart = table.Column<bool>(nullable: false),
                    VisibleElementID = table.Column<int>(nullable: true),
                    UserID = table.Column<Guid>(nullable: false),
                    SummaryID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSummaries", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserSummaries_Summaries_SummaryID",
                        column: x => x.SummaryID,
                        principalTable: "Summaries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSummaries_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSummaries_VisibleElements_VisibleElementID",
                        column: x => x.VisibleElementID,
                        principalTable: "VisibleElements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSummarySections",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserSummaryID = table.Column<int>(nullable: false),
                    SectionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSummarySections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserSummarySections_BudgetSections_SectionID",
                        column: x => x.SectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSummarySections_UserSummaries_UserSummaryID",
                        column: x => x.UserSummaryID,
                        principalTable: "UserSummaries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSummarySectionTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserSummaryID = table.Column<int>(nullable: false),
                    SectionTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSummarySectionTypes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserSummarySectionTypes_SectionTypes_SectionTypeID",
                        column: x => x.SectionTypeID,
                        principalTable: "SectionTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSummarySectionTypes_UserSummaries_UserSummaryID",
                        column: x => x.UserSummaryID,
                        principalTable: "UserSummaries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Summaries_VisibleElementID",
                table: "Summaries",
                column: "VisibleElementID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSummaries_SummaryID",
                table: "UserSummaries",
                column: "SummaryID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSummaries_UserID",
                table: "UserSummaries",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSummaries_VisibleElementID",
                table: "UserSummaries",
                column: "VisibleElementID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSummarySections_SectionID",
                table: "UserSummarySections",
                column: "SectionID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSummarySections_UserSummaryID",
                table: "UserSummarySections",
                column: "UserSummaryID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSummarySectionTypes_SectionTypeID",
                table: "UserSummarySectionTypes",
                column: "SectionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSummarySectionTypes_UserSummaryID",
                table: "UserSummarySectionTypes",
                column: "UserSummaryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSummarySections");

            migrationBuilder.DropTable(
                name: "UserSummarySectionTypes");

            migrationBuilder.DropTable(
                name: "UserSummaries");

            migrationBuilder.DropTable(
                name: "Summaries");
        }
    }
}
