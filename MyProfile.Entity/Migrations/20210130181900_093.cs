using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _093 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShedulerTaskLogs");

            migrationBuilder.DropTable(
                name: "ShedulerTasks");

            migrationBuilder.CreateTable(
                name: "SchedulerTasks",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    FirstStart = table.Column<DateTime>(nullable: true),
                    LastStart = table.Column<DateTime>(nullable: true),
                    TaskStatus = table.Column<string>(maxLength: 16, nullable: false),
                    TaskType = table.Column<string>(maxLength: 64, nullable: false),
                    CronExpression = table.Column<string>(maxLength: 16, nullable: true),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulerTasks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SchedulerTaskLogs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(nullable: true),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: true),
                    IsError = table.Column<bool>(nullable: false),
                    ChangedItems = table.Column<int>(nullable: false),
                    TaskID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulerTaskLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SchedulerTaskLogs_SchedulerTasks_TaskID",
                        column: x => x.TaskID,
                        principalTable: "SchedulerTasks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerTaskLogs_TaskID",
                table: "SchedulerTaskLogs",
                column: "TaskID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchedulerTaskLogs");

            migrationBuilder.DropTable(
                name: "SchedulerTasks");

            migrationBuilder.CreateTable(
                name: "ShedulerTasks",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(nullable: true),
                    CronExpression = table.Column<string>(maxLength: 16, nullable: true),
                    FirstStart = table.Column<DateTime>(nullable: true),
                    LastStart = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    TaskStatus = table.Column<string>(maxLength: 16, nullable: false),
                    TaskType = table.Column<string>(maxLength: 64, nullable: false)
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
                    ChangedItems = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    End = table.Column<DateTime>(nullable: true),
                    IsError = table.Column<bool>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
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
    }
}
