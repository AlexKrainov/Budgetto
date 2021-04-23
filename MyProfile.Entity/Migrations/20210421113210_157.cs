﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _157 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBoth",
                table: "SubScriptionOptions",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBoth",
                table: "SubScriptionOptions");
        }
    }
}