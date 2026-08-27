using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssistenciaTecnica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTecnicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TecnicoId",
                table: "OrdensServico",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tecnicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", nullable: true),
                    WhatsApp = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Especialidade = table.Column<string>(type: "TEXT", nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnicos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_TecnicoId",
                table: "OrdensServico",
                column: "TecnicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdensServico_Tecnicos_TecnicoId",
                table: "OrdensServico",
                column: "TecnicoId",
                principalTable: "Tecnicos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdensServico_Tecnicos_TecnicoId",
                table: "OrdensServico");

            migrationBuilder.DropTable(
                name: "Tecnicos");

            migrationBuilder.DropIndex(
                name: "IX_OrdensServico_TecnicoId",
                table: "OrdensServico");

            migrationBuilder.DropColumn(
                name: "TecnicoId",
                table: "OrdensServico");
        }
    }
}
