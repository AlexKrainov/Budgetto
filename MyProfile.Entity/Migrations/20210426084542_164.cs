using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _164 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tariff",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Tariff",
                table: "PaymentHistories");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PeriodTypes",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "CodeName",
                table: "PeriodTypes",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "PaymentTariffID",
                table: "Payments",
                nullable: true,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentTariffID",
                table: "PaymentHistories",
                nullable: true,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "EntityTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodeName = table.Column<string>(maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PaymentCounters",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CanBeCount = table.Column<int>(nullable: false),
                    LastChanges = table.Column<DateTime>(nullable: false),
                    PaymentTariffID = table.Column<int>(nullable: false),
                    EntityTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentCounters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PaymentCounters_EntityTypes_EntityTypeID",
                        column: x => x.EntityTypeID,
                        principalTable: "EntityTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentCounters_PaymentTariffs_PaymentTariffID",
                        column: x => x.PaymentTariffID,
                        principalTable: "PaymentTariffs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEntityCounters",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CanBeCount = table.Column<int>(nullable: false),
                    LastChanges = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    EntityTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEntityCounters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserEntityCounters_EntityTypes_EntityTypeID",
                        column: x => x.EntityTypeID,
                        principalTable: "EntityTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserEntityCounters_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentCounters_EntityTypeID",
                table: "PaymentCounters",
                column: "EntityTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentCounters_PaymentTariffID",
                table: "PaymentCounters",
                column: "PaymentTariffID");

            migrationBuilder.CreateIndex(
                name: "IX_UserEntityCounters_EntityTypeID",
                table: "UserEntityCounters",
                column: "EntityTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_UserEntityCounters_UserID",
                table: "UserEntityCounters",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentCounters");

            migrationBuilder.DropTable(
                name: "UserEntityCounters");

            migrationBuilder.DropTable(
                name: "EntityTypes");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PeriodTypes",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<string>(
                name: "CodeName",
                table: "PeriodTypes",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentTariffID",
                table: "Payments",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true,
                oldDefaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Tariff",
                table: "Payments",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentTariffID",
                table: "PaymentHistories",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true,
                oldDefaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Tariff",
                table: "PaymentHistories",
                nullable: true);
        }
    }
}
