using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _117 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeZone",
                table: "Reminders",
                newName: "TimeZoneClient");

            migrationBuilder.RenameColumn(
                name: "OffSet",
                table: "Reminders",
                newName: "OffSetClient");

            migrationBuilder.AddColumn<int>(
                name: "TimeZoneID",
                table: "Reminders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TimeZones",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WindowsTimezoneID = table.Column<string>(maxLength: 64, nullable: true),
                    WindowsDisplayName = table.Column<string>(maxLength: 256, nullable: true),
                    UTCOffset = table.Column<int>(nullable: false),
                    IsDST = table.Column<bool>(nullable: false),
                    Abreviatura = table.Column<string>(maxLength: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeZones", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "OlsonTZIDs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    TimeZoneID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OlsonTZIDs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OlsonTZIDs_TimeZones_TimeZoneID",
                        column: x => x.TimeZoneID,
                        principalTable: "TimeZones",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_TimeZoneID",
                table: "Reminders",
                column: "TimeZoneID");

            migrationBuilder.CreateIndex(
                name: "IX_OlsonTZIDs_TimeZoneID",
                table: "OlsonTZIDs",
                column: "TimeZoneID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_TimeZones_TimeZoneID",
                table: "Reminders",
                column: "TimeZoneID",
                principalTable: "TimeZones",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_TimeZones_TimeZoneID",
                table: "Reminders");

            migrationBuilder.DropTable(
                name: "OlsonTZIDs");

            migrationBuilder.DropTable(
                name: "TimeZones");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_TimeZoneID",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "TimeZoneID",
                table: "Reminders");

            migrationBuilder.RenameColumn(
                name: "TimeZoneClient",
                table: "Reminders",
                newName: "TimeZone");

            migrationBuilder.RenameColumn(
                name: "OffSetClient",
                table: "Reminders",
                newName: "OffSet");
        }
    }
}
