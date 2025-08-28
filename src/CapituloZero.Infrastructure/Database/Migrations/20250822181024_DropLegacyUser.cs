using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class DropLegacyUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
            migrationBuilder.DropForeignKey(
                name: "fk_todo_items_user_user_id",
                schema: "public",
                table: "todo_items");

            migrationBuilder.AddForeignKey(
                name: "fk_todo_items_asp_net_users_user_id",
                schema: "public",
                table: "todo_items",
                column: "user_id",
                principalSchema: "usuarios",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            // Drop the legacy standalone table created during IdentityInit
            migrationBuilder.DropTable(
                name: "user",
                schema: "public");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
            migrationBuilder.DropForeignKey(
                name: "fk_todo_items_asp_net_users_user_id",
                schema: "public",
                table: "todo_items");

            migrationBuilder.AddForeignKey(
                name: "fk_todo_items_user_user_id",
                schema: "public",
                table: "todo_items",
                column: "user_id",
                principalSchema: "public",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
