using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class PruneLegacyUserFromModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
            // Only drop if the legacy table still exists (guard for idempotency across envs)
            migrationBuilder.Sql("DO $$ BEGIN IF to_regclass('public.user') IS NOT NULL THEN DROP TABLE public.\"user\"; END IF; END $$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
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
        }
    }
}
