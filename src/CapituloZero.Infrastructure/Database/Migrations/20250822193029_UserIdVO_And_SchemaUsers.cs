using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class UserIdVO_And_SchemaUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_todo_items_asp_net_users_user_id",
                schema: "public",
                table: "todo_items");

            migrationBuilder.EnsureSchema(
                name: "users");

            migrationBuilder.RenameTable(
                name: "users",
                schema: "usuarios",
                newName: "users",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "user_tokens",
                schema: "usuarios",
                newName: "user_tokens",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "user_roles",
                schema: "usuarios",
                newName: "user_roles",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "user_logins",
                schema: "usuarios",
                newName: "user_logins",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "user_claims",
                schema: "usuarios",
                newName: "user_claims",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "roles",
                schema: "usuarios",
                newName: "roles",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "role_claims",
                schema: "usuarios",
                newName: "role_claims",
                newSchema: "users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "usuarios");

            migrationBuilder.RenameTable(
                name: "users",
                schema: "users",
                newName: "users",
                newSchema: "usuarios");

            migrationBuilder.RenameTable(
                name: "user_tokens",
                schema: "users",
                newName: "user_tokens",
                newSchema: "usuarios");

            migrationBuilder.RenameTable(
                name: "user_roles",
                schema: "users",
                newName: "user_roles",
                newSchema: "usuarios");

            migrationBuilder.RenameTable(
                name: "user_logins",
                schema: "users",
                newName: "user_logins",
                newSchema: "usuarios");

            migrationBuilder.RenameTable(
                name: "user_claims",
                schema: "users",
                newName: "user_claims",
                newSchema: "usuarios");

            migrationBuilder.RenameTable(
                name: "roles",
                schema: "users",
                newName: "roles",
                newSchema: "usuarios");

            migrationBuilder.RenameTable(
                name: "role_claims",
                schema: "users",
                newName: "role_claims",
                newSchema: "usuarios");

            migrationBuilder.AddForeignKey(
                name: "fk_todo_items_asp_net_users_user_id",
                schema: "public",
                table: "todo_items",
                column: "user_id",
                principalSchema: "usuarios",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
