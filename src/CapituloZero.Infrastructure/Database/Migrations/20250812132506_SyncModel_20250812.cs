using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CapituloZero.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel_20250812 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "labels",
                schema: "public",
                table: "todo_items");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "labels",
                schema: "public",
                table: "todo_items",
                type: "text[]",
                nullable: false);
        }
    }
}
