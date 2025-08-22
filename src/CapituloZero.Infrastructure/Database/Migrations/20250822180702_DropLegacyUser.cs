using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    // Renamed to avoid class name collision with 20250822181024_DropLegacyUser
    public partial class DropLegacyUser_20250822180702 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            // Adjust FK: todo_items.user_id now references usuarios.users
            migrationBuilder.DropForeignKey(
                name: "fk_todo_items_user_user_id",
                schema: "public",
                table: "todo_items");

            // Drop legacy table public.user if exists
            migrationBuilder.DropTable(
                name: "user",
                schema: "public");

            // Recreate FK to usuarios.users
            migrationBuilder.AddForeignKey(
                name: "fk_todo_items_users_user_id",
                schema: "public",
                table: "todo_items",
                column: "user_id",
                principalSchema: "usuarios",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            // Recreate legacy table (minimal columns) to restore previous state
            migrationBuilder.CreateTable(
                name: "user",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_email",
                schema: "public",
                table: "user",
                column: "email",
                unique: true);

            // Restore original FK to public.user
            migrationBuilder.DropForeignKey(
                name: "fk_todo_items_users_user_id",
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
