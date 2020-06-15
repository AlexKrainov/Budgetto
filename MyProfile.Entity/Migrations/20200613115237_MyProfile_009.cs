using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_009 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GoalID",
                table: "SectionGroupLimits",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Limits",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    TotalMoney = table.Column<decimal>(type: "Money", nullable: true),
                    DateStart = table.Column<DateTime>(nullable: true),
                    DateEnd = table.Column<DateTime>(nullable: true),
                    IsShowOnDashBoard = table.Column<bool>(nullable: false, defaultValue: true),
                    IsShowInCollective = table.Column<bool>(nullable: false),
                    IsFinished = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Goals_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoalRecords",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Total = table.Column<decimal>(type: "Money", nullable: false),
                    DateTimeOfPayment = table.Column<DateTime>(nullable: true),
                    CreateDateTime = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<Guid>(nullable: true),
                    GoalID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalRecords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GoalRecords_Goals_GoalID",
                        column: x => x.GoalID,
                        principalTable: "Goals",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoalRecords_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SectionGroupLimits_GoalID",
                table: "SectionGroupLimits",
                column: "GoalID");

            migrationBuilder.CreateIndex(
                name: "IX_GoalRecords_GoalID",
                table: "GoalRecords",
                column: "GoalID");

            migrationBuilder.CreateIndex(
                name: "IX_GoalRecords_UserID",
                table: "GoalRecords",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_UserID",
                table: "Goals",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_SectionGroupLimits_Goals_GoalID",
                table: "SectionGroupLimits",
                column: "GoalID",
                principalTable: "Goals",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SectionGroupLimits_Goals_GoalID",
                table: "SectionGroupLimits");

            migrationBuilder.DropTable(
                name: "GoalRecords");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropIndex(
                name: "IX_SectionGroupLimits_GoalID",
                table: "SectionGroupLimits");

            migrationBuilder.DropColumn(
                name: "GoalID",
                table: "SectionGroupLimits");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Limits");
        }
    }
}
