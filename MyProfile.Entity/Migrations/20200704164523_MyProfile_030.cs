using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_030 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currencies");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    CanBeUser = table.Column<bool>(nullable: false),
                    CodeName = table.Column<string>(maxLength: 3, nullable: false),
                    Icon = table.Column<string>(maxLength: 1, nullable: false),
                    IntlSpecificCulture = table.Column<string>(maxLength: 8, nullable: false),
                    Name = table.Column<string>(maxLength: 16, nullable: false),
                    SpecificCulture = table.Column<string>(maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.ID);
                });
        }
    }
}
