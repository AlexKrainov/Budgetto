using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_064 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 132, nullable: false),
                    Image = table.Column<string>(maxLength: 256, nullable: true),
                    IconCss = table.Column<string>(maxLength: 32, nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserTags",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 132, nullable: false),
                    Image = table.Column<string>(maxLength: 256, nullable: true),
                    IconCss = table.Column<string>(maxLength: 32, nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    TagID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTags", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserTags_Tags_TagID",
                        column: x => x.TagID,
                        principalTable: "Tags",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecordTags",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RecordID = table.Column<int>(nullable: false),
                    UserTagID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordTags", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RecordTags_BudgetRecords_RecordID",
                        column: x => x.RecordID,
                        principalTable: "BudgetRecords",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecordTags_UserTags_UserTagID",
                        column: x => x.UserTagID,
                        principalTable: "UserTags",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordTags_RecordID",
                table: "RecordTags",
                column: "RecordID");

            migrationBuilder.CreateIndex(
                name: "IX_RecordTags_UserTagID",
                table: "RecordTags",
                column: "UserTagID");

            migrationBuilder.CreateIndex(
                name: "IX_UserTags_TagID",
                table: "UserTags",
                column: "TagID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecordTags");

            migrationBuilder.DropTable(
                name: "UserTags");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
