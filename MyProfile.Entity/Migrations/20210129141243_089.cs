using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _089 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShedulerTasks",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    FirstStart = table.Column<DateTime>(nullable: false),
                    LastStart = table.Column<DateTime>(nullable: false),
                    TaskStatus = table.Column<string>(maxLength: 16, nullable: false),
                    TaskType = table.Column<string>(maxLength: 16, nullable: false),
                    CronExpression = table.Column<string>(maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShedulerTasks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ShedulerTaskLogs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: true),
                    IsError = table.Column<bool>(nullable: false),
                    ChangedItems = table.Column<int>(nullable: false),
                    TaskID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShedulerTaskLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ShedulerTaskLogs_ShedulerTasks_TaskID",
                        column: x => x.TaskID,
                        principalTable: "ShedulerTasks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShedulerTaskLogs_TaskID",
                table: "ShedulerTaskLogs",
                column: "TaskID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShedulerTaskLogs");

            migrationBuilder.DropTable(
                name: "ShedulerTasks");
        }
    }
}
