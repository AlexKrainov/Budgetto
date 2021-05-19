using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTags_Tags_TagID",
                table: "UserTags");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_UserTags_TagID",
                table: "UserTags");

            migrationBuilder.DropColumn(
                name: "TagID",
                table: "UserTags");

            migrationBuilder.DropColumn(
                name: "CodeName",
                table: "BudgetAreas");

            migrationBuilder.AddColumn<int>(
                name: "CompanyID",
                table: "UserTags",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    TagKeyWords = table.Column<string>(maxLength: 512, nullable: true),
                    BankKeyWords = table.Column<string>(maxLength: 512, nullable: true),
                    Site = table.Column<string>(maxLength: 128, nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    IsChecked = table.Column<bool>(nullable: false),
                    Country = table.Column<string>(maxLength: 64, nullable: true),
                    City = table.Column<string>(maxLength: 64, nullable: true),
                    BrandColor = table.Column<string>(maxLength: 8, nullable: true),
                    LogoCircle = table.Column<string>(maxLength: 128, nullable: true),
                    LogoSquare = table.Column<string>(maxLength: 128, nullable: true),
                    ParentCompanyID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Companies_Companies_ParentCompanyID",
                        column: x => x.ParentCompanyID,
                        principalTable: "Companies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MccCategories",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: true),
                    IsSystem = table.Column<bool>(nullable: false),
                    bankCategoryID = table.Column<int>(nullable: true),
                    bankParentCategoryID = table.Column<int>(nullable: true),
                    BankID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MccCategories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MccCategories_Banks_BankID",
                        column: x => x.BankID,
                        principalTable: "Banks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MccCodes",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Mcc = table.Column<int>(nullable: false),
                    CompanyID = table.Column<int>(nullable: false),
                    MccCategoryID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MccCodes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MccCodes_Companies_CompanyID",
                        column: x => x.CompanyID,
                        principalTable: "Companies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MccCodes_MccCategories_MccCategoryID",
                        column: x => x.MccCategoryID,
                        principalTable: "MccCategories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTags_CompanyID",
                table: "UserTags",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_ParentCompanyID",
                table: "Companies",
                column: "ParentCompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_MccCategories_BankID",
                table: "MccCategories",
                column: "BankID");

            migrationBuilder.CreateIndex(
                name: "IX_MccCodes_CompanyID",
                table: "MccCodes",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_MccCodes_MccCategoryID",
                table: "MccCodes",
                column: "MccCategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTags_Companies_CompanyID",
                table: "UserTags",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTags_Companies_CompanyID",
                table: "UserTags");

            migrationBuilder.DropTable(
                name: "MccCodes");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "MccCategories");

            migrationBuilder.DropIndex(
                name: "IX_UserTags_CompanyID",
                table: "UserTags");

            migrationBuilder.DropColumn(
                name: "CompanyID",
                table: "UserTags");

            migrationBuilder.AddColumn<long>(
                name: "TagID",
                table: "UserTags",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodeName",
                table: "BudgetAreas",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    IconCss = table.Column<string>(maxLength: 32, nullable: true),
                    Image = table.Column<string>(maxLength: 256, nullable: true),
                    Title = table.Column<string>(maxLength: 132, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTags_TagID",
                table: "UserTags",
                column: "TagID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTags_Tags_TagID",
                table: "UserTags",
                column: "TagID",
                principalTable: "Tags",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
