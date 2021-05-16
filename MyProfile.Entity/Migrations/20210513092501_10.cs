using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SectionTypeID",
                table: "BaseSections",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BaseSections_SectionTypeID",
                table: "BaseSections",
                column: "SectionTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseSections_SectionTypes_SectionTypeID",
                table: "BaseSections",
                column: "SectionTypeID",
                principalTable: "SectionTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseSections_SectionTypes_SectionTypeID",
                table: "BaseSections");

            migrationBuilder.DropIndex(
                name: "IX_BaseSections_SectionTypeID",
                table: "BaseSections");

            migrationBuilder.DropColumn(
                name: "SectionTypeID",
                table: "BaseSections");
        }
    }
}
