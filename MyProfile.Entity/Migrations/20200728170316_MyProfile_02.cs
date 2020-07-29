using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResourceID",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    Extension = table.Column<string>(maxLength: 8, nullable: false),
                    BodyBase64 = table.Column<string>(maxLength: 8, nullable: true),
                    FolderName = table.Column<string>(maxLength: 32, nullable: false),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateEdit = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_ResourceID",
                table: "Users",
                column: "ResourceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Resources_ResourceID",
                table: "Users",
                column: "ResourceID",
                principalTable: "Resources",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Resources_ResourceID",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropIndex(
                name: "IX_Users_ResourceID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResourceID",
                table: "Users");
        }
    }
}
