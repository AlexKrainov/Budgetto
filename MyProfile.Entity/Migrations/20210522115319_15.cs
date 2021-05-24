using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgressItemTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodeName = table.Column<string>(maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressItemTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProgressTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodeName = table.Column<string>(maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Progresses",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsComplete = table.Column<bool>(nullable: false),
                    DateComplete = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(maxLength: 32, nullable: true),
                    UserDateEdit = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                    UserID = table.Column<Guid>(nullable: false),
                    ParentProgressID = table.Column<long>(nullable: true),
                    ProgressTypeID = table.Column<int>(nullable: false),
                    ProgressItemTypeID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Progresses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Progresses_Progresses_ParentProgressID",
                        column: x => x.ParentProgressID,
                        principalTable: "Progresses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Progresses_ProgressItemTypes_ProgressItemTypeID",
                        column: x => x.ProgressItemTypeID,
                        principalTable: "ProgressItemTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Progresses_ProgressTypes_ProgressTypeID",
                        column: x => x.ProgressTypeID,
                        principalTable: "ProgressTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Progresses_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgressLogs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsComplete = table.Column<bool>(nullable: false),
                    DateComplete = table.Column<DateTime>(nullable: true),
                    Value = table.Column<string>(maxLength: 32, nullable: true),
                    UserDateEdit = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    ProgressID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProgressLogs_Progresses_ProgressID",
                        column: x => x.ProgressID,
                        principalTable: "Progresses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Progresses_ParentProgressID",
                table: "Progresses",
                column: "ParentProgressID");

            migrationBuilder.CreateIndex(
                name: "IX_Progresses_ProgressItemTypeID",
                table: "Progresses",
                column: "ProgressItemTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Progresses_ProgressTypeID",
                table: "Progresses",
                column: "ProgressTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Progresses_UserID",
                table: "Progresses",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressLogs_ProgressID",
                table: "ProgressLogs",
                column: "ProgressID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgressLogs");

            migrationBuilder.DropTable(
                name: "Progresses");

            migrationBuilder.DropTable(
                name: "ProgressItemTypes");

            migrationBuilder.DropTable(
                name: "ProgressTypes");
        }
    }
}
