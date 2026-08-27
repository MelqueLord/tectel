using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssistenciaTecnica.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CpfCnpj = table.Column<string>(type: "TEXT", maxLength: 14, nullable: true),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    WhatsApp = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Endereco = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Bairro = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Cidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    Cep = table.Column<string>(type: "TEXT", maxLength: 8, nullable: true),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracaoEmpresa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CpfCnpj = table.Column<string>(type: "TEXT", maxLength: 14, nullable: true),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    WhatsApp = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Endereco = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    LogoCaminho = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TextoPadraoOrdem = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: true),
                    PrazoPadraoGarantiaDias = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 90)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracaoEmpresa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Categoria = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Marca = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModeloCompativel = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    PrecoCusto = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    PrecoVenda = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    QuantidadeEstoque = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    EstoqueMinimo = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Aparelhos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Marca = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Modelo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Cor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Imei = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    NumeroSerie = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    SenhaDesbloqueio = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EstadoAparelho = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ItensEntregues = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aparelhos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aparelhos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdensServico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    AparelhoId = table.Column<int>(type: "INTEGER", nullable: false),
                    DefeitoRelatado = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Diagnostico = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ServicoRealizado = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    DataEntrada = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PrevisaoEntrega = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DataConclusao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DataEntrega = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    ValorServico = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    ValorPecas = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    Desconto = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    ValorTotal = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    ValorPago = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    PrazoGarantiaDias = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 90)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdensServico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdensServico_Aparelhos_AparelhoId",
                        column: x => x.AparelhoId,
                        principalTable: "Aparelhos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdensServico_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimentacoesEstoque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProdutoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    DataMovimentacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Motivo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    OrdemServicoId = table.Column<int>(type: "INTEGER", nullable: true),
                    UsuarioResponsavel = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimentacoesEstoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimentacoesEstoque_OrdensServico_OrdemServicoId",
                        column: x => x.OrdemServicoId,
                        principalTable: "OrdensServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MovimentacoesEstoque_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pagamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrdemServicoId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Valor = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    FormaPagamento = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagamentos_OrdensServico_OrdemServicoId",
                        column: x => x.OrdemServicoId,
                        principalTable: "OrdensServico",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aparelhos_ClienteId",
                table: "Aparelhos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Aparelhos_Imei",
                table: "Aparelhos",
                column: "Imei");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_CpfCnpj",
                table: "Clientes",
                column: "CpfCnpj",
                unique: true,
                filter: "[CpfCnpj] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Nome",
                table: "Clientes",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesEstoque_DataMovimentacao",
                table: "MovimentacoesEstoque",
                column: "DataMovimentacao");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesEstoque_OrdemServicoId",
                table: "MovimentacoesEstoque",
                column: "OrdemServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesEstoque_ProdutoId",
                table: "MovimentacoesEstoque",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_AparelhoId",
                table: "OrdensServico",
                column: "AparelhoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_ClienteId",
                table: "OrdensServico",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_DataEntrada",
                table: "OrdensServico",
                column: "DataEntrada");

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_Numero",
                table: "OrdensServico",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdensServico_Status",
                table: "OrdensServico",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_DataPagamento",
                table: "Pagamentos",
                column: "DataPagamento");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_OrdemServicoId",
                table: "Pagamentos",
                column: "OrdemServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_Categoria",
                table: "Produtos",
                column: "Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_Codigo",
                table: "Produtos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_Nome",
                table: "Produtos",
                column: "Nome");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracaoEmpresa");

            migrationBuilder.DropTable(
                name: "MovimentacoesEstoque");

            migrationBuilder.DropTable(
                name: "Pagamentos");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "OrdensServico");

            migrationBuilder.DropTable(
                name: "Aparelhos");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
