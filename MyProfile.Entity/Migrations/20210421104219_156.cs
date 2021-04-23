using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _156 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubScriptionCategories",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 128, nullable: false),
                    CodeName = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubScriptionCategories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SubScriptions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Site = table.Column<string>(maxLength: 512, nullable: true),
                    LogoBig = table.Column<string>(maxLength: 512, nullable: true),
                    LogoSmall = table.Column<string>(maxLength: 512, nullable: true),
                    SubScriptionCategoryID = table.Column<int>(nullable: false),
                    ParentID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubScriptions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SubScriptions_SubScriptions_ParentID",
                        column: x => x.ParentID,
                        principalTable: "SubScriptions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubScriptions_SubScriptionCategories_SubScriptionCategoryID",
                        column: x => x.SubScriptionCategoryID,
                        principalTable: "SubScriptionCategories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubScriptionOptions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    EditDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                    IsPersonally = table.Column<bool>(nullable: false),
                    IsFamaly = table.Column<bool>(nullable: false),
                    IsStudent = table.Column<bool>(nullable: false),
                    _raiting = table.Column<decimal>(nullable: true),
                    SubScriptionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubScriptionOptions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SubScriptionOptions_SubScriptions_SubScriptionID",
                        column: x => x.SubScriptionID,
                        principalTable: "SubScriptions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubScriptionPricings",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PricingPeriodTypeID = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(type: "Money", nullable: false),
                    PricePerMonth = table.Column<decimal>(type: "Money", nullable: false),
                    SubScriptionOptionID = table.Column<int>(nullable: false),
                    UserID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubScriptionPricings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SubScriptionPricings_SubScriptionOptions_SubScriptionOptionID",
                        column: x => x.SubScriptionOptionID,
                        principalTable: "SubScriptionOptions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubScriptionPricings_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSubScriptions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    endDate = table.Column<DateTime>(nullable: true),
                    Price = table.Column<decimal>(type: "Money", nullable: false),
                    PricePerMonth = table.Column<decimal>(type: "Money", nullable: false),
                    IsPause = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SubScriptionPricingID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubScriptions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserSubScriptions_SubScriptionPricings_SubScriptionPricingID",
                        column: x => x.SubScriptionPricingID,
                        principalTable: "SubScriptionPricings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubScriptionOptions_SubScriptionID",
                table: "SubScriptionOptions",
                column: "SubScriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_SubScriptionPricings_SubScriptionOptionID",
                table: "SubScriptionPricings",
                column: "SubScriptionOptionID");

            migrationBuilder.CreateIndex(
                name: "IX_SubScriptionPricings_UserID",
                table: "SubScriptionPricings",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_SubScriptions_ParentID",
                table: "SubScriptions",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_SubScriptions_SubScriptionCategoryID",
                table: "SubScriptions",
                column: "SubScriptionCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubScriptions_SubScriptionPricingID",
                table: "UserSubScriptions",
                column: "SubScriptionPricingID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSubScriptions");

            migrationBuilder.DropTable(
                name: "SubScriptionPricings");

            migrationBuilder.DropTable(
                name: "SubScriptionOptions");

            migrationBuilder.DropTable(
                name: "SubScriptions");

            migrationBuilder.DropTable(
                name: "SubScriptionCategories");
        }
    }
}
