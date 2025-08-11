using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTypesAndActiveType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "active_type",
                schema: "public",
                table: "users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "types",
                schema: "public",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "autores",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_autores", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "fluxos_producao",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "text", nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fluxos_producao", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "funcoes",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_funcoes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "livros",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    titulo = table.Column<string>(type: "text", nullable: false),
                    subtitulo = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    autor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fluxo_producao_id = table.Column<Guid>(type: "uuid", nullable: true),
                    current_etapa_index = table.Column<int>(type: "integer", nullable: false),
                    data_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_conclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_livros", x => x.id);
                    table.ForeignKey(
                        name: "fk_livros_autores_autor_id",
                        column: x => x.autor_id,
                        principalSchema: "public",
                        principalTable: "autores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "etapas_template",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "text", nullable: false),
                    funcao_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ordem = table.Column<int>(type: "integer", nullable: false),
                    prazo_dias = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    observacao_padrao = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    fluxo_producao_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_etapas_template", x => x.id);
                    table.ForeignKey(
                        name: "fk_etapas_template_fluxos_producao_fluxo_producao_id",
                        column: x => x.fluxo_producao_id,
                        principalSchema: "public",
                        principalTable: "fluxos_producao",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_etapas_template_funcoes_funcao_id",
                        column: x => x.funcao_id,
                        principalSchema: "public",
                        principalTable: "funcoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "terceiros",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "text", nullable: false),
                    documento = table.Column<string>(type: "text", nullable: false),
                    funcao_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_terceiros", x => x.id);
                    table.ForeignKey(
                        name: "fk_terceiros_funcoes_funcao_id",
                        column: x => x.funcao_id,
                        principalSchema: "public",
                        principalTable: "funcoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "etapas",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    livro_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "text", nullable: false),
                    observacao = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    data_limite = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    responsavel_id = table.Column<Guid>(type: "uuid", nullable: true),
                    funcao_id = table.Column<Guid>(type: "uuid", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_etapas", x => x.id);
                    table.ForeignKey(
                        name: "fk_etapas_funcoes_funcao_id",
                        column: x => x.funcao_id,
                        principalSchema: "public",
                        principalTable: "funcoes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_etapas_livros_livro_id",
                        column: x => x.livro_id,
                        principalSchema: "public",
                        principalTable: "livros",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "artefatos",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    etapa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_uri = table.Column<string>(type: "text", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    content_type = table.Column<string>(type: "text", nullable: false),
                    size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    uploaded_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artefatos", x => x.id);
                    table.ForeignKey(
                        name: "fk_artefatos_etapas_etapa_id",
                        column: x => x.etapa_id,
                        principalSchema: "public",
                        principalTable: "etapas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_artefatos_etapa_id",
                schema: "public",
                table: "artefatos",
                column: "etapa_id");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_funcao_id",
                schema: "public",
                table: "etapas",
                column: "funcao_id");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_livro_id",
                schema: "public",
                table: "etapas",
                column: "livro_id");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_template_fluxo_producao_id",
                schema: "public",
                table: "etapas_template",
                column: "fluxo_producao_id");

            migrationBuilder.CreateIndex(
                name: "ix_etapas_template_funcao_id",
                schema: "public",
                table: "etapas_template",
                column: "funcao_id");

            migrationBuilder.CreateIndex(
                name: "ix_livros_autor_id",
                schema: "public",
                table: "livros",
                column: "autor_id");

            migrationBuilder.CreateIndex(
                name: "ix_terceiros_funcao_id",
                schema: "public",
                table: "terceiros",
                column: "funcao_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "artefatos",
                schema: "public");

            migrationBuilder.DropTable(
                name: "etapas_template",
                schema: "public");

            migrationBuilder.DropTable(
                name: "terceiros",
                schema: "public");

            migrationBuilder.DropTable(
                name: "etapas",
                schema: "public");

            migrationBuilder.DropTable(
                name: "fluxos_producao",
                schema: "public");

            migrationBuilder.DropTable(
                name: "funcoes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "livros",
                schema: "public");

            migrationBuilder.DropTable(
                name: "autores",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "active_type",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "types",
                schema: "public",
                table: "users");
        }
    }
}
