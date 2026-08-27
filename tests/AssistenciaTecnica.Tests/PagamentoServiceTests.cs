using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Application.Services;
using AssistenciaTecnica.Domain.Entities;
using Moq;

namespace AssistenciaTecnica.Tests;

public class PagamentoServiceTests
{
    private readonly Mock<IPagamentoRepository> _pagamentoRepoMock;
    private readonly Mock<IOrdemServicoRepository> _ordemRepoMock;
    private readonly PagamentoService _service;

    public PagamentoServiceTests()
    {
        _pagamentoRepoMock = new Mock<IPagamentoRepository>();
        _ordemRepoMock = new Mock<IOrdemServicoRepository>();
        _service = new PagamentoService(_pagamentoRepoMock.Object, _ordemRepoMock.Object);
    }

    [Fact]
    public async Task RegistrarAsync_ValorZero_DeveLancarExcecao()
    {
        var dto = new RegistrarPagamentoDto
        {
            OrdemServicoId = 1,
            Valor = 0,
            FormaPagamento = FormaPagamento.Dinheiro
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegistrarAsync(dto));
        Assert.Equal("O valor do pagamento deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public async Task RegistrarAsync_ValorNegativo_DeveLancarExcecao()
    {
        var dto = new RegistrarPagamentoDto
        {
            OrdemServicoId = 1,
            Valor = -10,
            FormaPagamento = FormaPagamento.Dinheiro
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegistrarAsync(dto));
        Assert.Equal("O valor do pagamento deve ser maior que zero.", ex.Message);
    }

    [Fact]
    public async Task RegistrarAsync_OrdemInexistente_DeveLancarExcecao()
    {
        var dto = new RegistrarPagamentoDto
        {
            OrdemServicoId = 999,
            Valor = 100,
            FormaPagamento = FormaPagamento.Pix
        };

        _ordemRepoMock.Setup(r => r.ObterPorIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrdemServico?)null);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegistrarAsync(dto));
        Assert.Equal("Ordem de serviço não encontrada.", ex.Message);
    }

    [Fact]
    public async Task RegistrarAsync_SemSaldoPendente_DeveLancarExcecao()
    {
        var ordem = new OrdemServico
        {
            Id = 1,
            ValorTotal = 200,
            ValorPago = 200
        };

        var dto = new RegistrarPagamentoDto
        {
            OrdemServicoId = 1,
            Valor = 50,
            FormaPagamento = FormaPagamento.Dinheiro
        };

        _ordemRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordem);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegistrarAsync(dto));
        Assert.Equal("Esta ordem não possui saldo pendente.", ex.Message);
    }

    [Fact]
    public async Task RegistrarAsync_ValorExcedeSaldo_DeveLancarExcecao()
    {
        var ordem = new OrdemServico
        {
            Id = 1,
            ValorTotal = 200,
            ValorPago = 150
        };

        var dto = new RegistrarPagamentoDto
        {
            OrdemServicoId = 1,
            Valor = 100,
            FormaPagamento = FormaPagamento.Dinheiro
        };

        _ordemRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordem);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegistrarAsync(dto));
        Assert.Contains("excede o saldo pendente", ex.Message);
    }

    [Fact]
    public async Task RegistrarAsync_DadosValidos_DeveRegistrar()
    {
        var ordem = new OrdemServico
        {
            Id = 1,
            ValorTotal = 200,
            ValorPago = 0
        };

        var dto = new RegistrarPagamentoDto
        {
            OrdemServicoId = 1,
            Valor = 100,
            FormaPagamento = FormaPagamento.Pix,
            Observacao = "Pagamento parcial"
        };

        _ordemRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordem);

        var resultado = await _service.RegistrarAsync(dto);

        Assert.NotNull(resultado);
        Assert.Equal(100, resultado.Valor);
        Assert.Equal(FormaPagamento.Pix, resultado.FormaPagamento);
        Assert.Equal(100, ordem.ValorPago);
        _pagamentoRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Pagamento>(), It.IsAny<CancellationToken>()), Times.Once);
        _pagamentoRepoMock.Verify(r => r.SalvarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarAsync_MultiplosPagamentos_DeveAcumularValorPago()
    {
        var ordem = new OrdemServico
        {
            Id = 1,
            ValorTotal = 300,
            ValorPago = 100
        };

        var dto = new RegistrarPagamentoDto
        {
            OrdemServicoId = 1,
            Valor = 150,
            FormaPagamento = FormaPagamento.Dinheiro
        };

        _ordemRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordem);

        await _service.RegistrarAsync(dto);

        Assert.Equal(250, ordem.ValorPago);
    }

    [Fact]
    public async Task ListarPorOrdemAsync_DeveRetornarPagamentos()
    {
        var pagamentos = new List<Pagamento>
        {
            new() { Id = 1, OrdemServicoId = 1, Valor = 100, FormaPagamento = FormaPagamento.Pix, DataPagamento = DateTime.UtcNow },
            new() { Id = 2, OrdemServicoId = 1, Valor = 50, FormaPagamento = FormaPagamento.Dinheiro, DataPagamento = DateTime.UtcNow }
        };

        _pagamentoRepoMock.Setup(r => r.ListarPorOrdemAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagamentos);

        var resultado = await _service.ListarPorOrdemAsync(1);

        Assert.Equal(2, resultado.Count());
    }
}
