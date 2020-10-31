using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_046 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Where",
                table: "ErrorLogs",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "ErrorLogs",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "UserErrorLogs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ErrorLogID = table.Column<int>(nullable: true),
                    UserLogID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserErrorLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserErrorLogs_ErrorLogs_ErrorLogID",
                        column: x => x.ErrorLogID,
                        principalTable: "ErrorLogs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserErrorLogs_UserLogs_UserLogID",
                        column: x => x.UserLogID,
                        principalTable: "UserLogs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserErrorLogs_ErrorLogID",
                table: "UserErrorLogs",
                column: "ErrorLogID");

            migrationBuilder.CreateIndex(
                name: "IX_UserErrorLogs_UserLogID",
                table: "UserErrorLogs",
                column: "UserLogID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserErrorLogs");

            migrationBuilder.AlterColumn<string>(
                name: "Where",
                table: "ErrorLogs",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "ErrorLogs",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);
        }
    }
}
