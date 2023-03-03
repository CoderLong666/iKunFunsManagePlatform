using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class _004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Uid",
                table: "EntityBase",
                newName: "Id");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EntityBase",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "EntityBase",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EntityBase");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "EntityBase");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EntityBase",
                newName: "Uid");
        }
    }
}
