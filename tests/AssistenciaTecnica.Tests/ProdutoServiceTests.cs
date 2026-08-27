using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Application.Services;
using AssistenciaTecnica.Domain.Entities;
using Moq;

namespace AssistenciaTecnica.Tests;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _produtoRepoMock;
    private readonly Mock<IMovimentacaoEstoqueRepository> _movimentacaoRepoMock;
    private readonly ProdutoService _service;

    public ProdutoServiceTests()
    {
        _produtoRepoMock = new Mock<IProdutoRepository>();
        _movimentacaoRepoMock = new Mock<IMovimentacaoEstoqueRepository>();
        _service = new ProdutoService(_produtoRepoMock.Object, _movimentacaoRepoMock.Object);
    }

    [Fact]
    public async Task CriarAsync_SemCodigo_DeveLancarExcecao()
    {
        var dto = new CriarProdutoDto { Codigo = "", Nome = "Tela Samsung" };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Equal("O código é obrigatório.", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_SemNome_DeveLancarExcecao()
    {
        var dto = new CriarProdutoDto { Codigo = "PEC001", Nome = "" };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Equal("O nome é obrigatório.", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_CodigoDuplicado_DeveLancarExcecao()
    {
        var dto = new CriarProdutoDto { Codigo = "PEC001", Nome = "Tela Samsung" };

        _produtoRepoMock.Setup(r => r.ObterPorCodigoAsync("PEC001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Produto { Id = 1, Codigo = "PEC001" });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Equal("Já existe um produto com este código.", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_DadosValidos_DeveCriar()
    {
        var dto = new CriarProdutoDto
        {
            Codigo = "PEC001",
            Nome = "Tela Samsung Galaxy S23",
            Categoria = "Telas",
            PrecoCusto = 100,
            PrecoVenda = 200,
            QuantidadeEstoque = 10,
            EstoqueMinimo = 3
        };

        _produtoRepoMock.Setup(r => r.ObterPorCodigoAsync("PEC001", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Produto?)null);

        var resultado = await _service.CriarAsync(dto);

        Assert.NotNull(resultado);
        Assert.Equal("PEC001", resultado.Codigo);
        Assert.Equal("Tela Samsung Galaxy S23", resultado.Nome);
        Assert.True(resultado.Ativo);
        _produtoRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Produto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarEntradaAsync_DeveAumentarEstoque()
    {
        var produto = new Produto { Id = 1, Nome = "Tela", QuantidadeEstoque = 5 };

        _produtoRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);

        await _service.RegistrarEntradaAsync(1, 10, "Compra");

        Assert.Equal(15, produto.QuantidadeEstoque);
        _movimentacaoRepoMock.Verify(r => r.AdicionarAsync(
            It.Is<MovimentacaoEstoque>(m => m.Tipo == TipoMovimentacao.Entrada && m.Quantidade == 10),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarSaidaAsync_DeveDiminuirEstoque()
    {
        var produto = new Produto { Id = 1, Nome = "Tela", QuantidadeEstoque = 10 };

        _produtoRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);

        await _service.RegistrarSaidaAsync(1, 3, "Uso em OS", null);

        Assert.Equal(7, produto.QuantidadeEstoque);
        _movimentacaoRepoMock.Verify(r => r.AdicionarAsync(
            It.Is<MovimentacaoEstoque>(m => m.Tipo == TipoMovimentacao.Saida && m.Quantidade == 3),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarSaidaAsync_EstoqueInsuficiente_DeveLancarExcecao()
    {
        var produto = new Produto { Id = 1, Nome = "Tela", QuantidadeEstoque = 2 };

        _produtoRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.RegistrarSaidaAsync(1, 5, "Uso em OS", null));

        Assert.Equal("Estoque insuficiente.", ex.Message);
    }

    [Fact]
    public async Task RegistrarSaidaAsync_QuantidadeZero_DeveLancarExcecao()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.RegistrarSaidaAsync(1, 0, "Teste", null));

        Assert.Equal("A quantidade deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public async Task RegistrarAjusteAsync_DeveAjustarEstoque()
    {
        var produto = new Produto { Id = 1, Nome = "Tela", QuantidadeEstoque = 5 };

        _produtoRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);

        await _service.RegistrarAjusteAsync(1, 20, "Inventário");

        Assert.Equal(20, produto.QuantidadeEstoque);
        _movimentacaoRepoMock.Verify(r => r.AdicionarAsync(
            It.Is<MovimentacaoEstoque>(m => m.Tipo == TipoMovimentacao.Ajuste && m.Quantidade == 15),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarAjusteAsync_QuantidadeNegativa_DeveLancarExcecao()
    {
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.RegistrarAjusteAsync(1, -5, "Teste"));

        Assert.Equal("A quantidade não pode ser negativa.", ex.Message);
    }
}
