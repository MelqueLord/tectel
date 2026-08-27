using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Application.Services;
using AssistenciaTecnica.Domain.Entities;
using Moq;

namespace AssistenciaTecnica.Tests;

public class OrdemServicoServiceTests
{
    private readonly Mock<IOrdemServicoRepository> _ordemRepoMock;
    private readonly Mock<IClienteRepository> _clienteRepoMock;
    private readonly Mock<IAparelhoRepository> _aparelhoRepoMock;
    private readonly Mock<ITecnicoRepository> _tecnicoRepoMock;
    private readonly OrdemServicoService _service;

    public OrdemServicoServiceTests()
    {
        _ordemRepoMock = new Mock<IOrdemServicoRepository>();
        _clienteRepoMock = new Mock<IClienteRepository>();
        _aparelhoRepoMock = new Mock<IAparelhoRepository>();
        _tecnicoRepoMock = new Mock<ITecnicoRepository>();
        _service = new OrdemServicoService(
            _ordemRepoMock.Object,
            _clienteRepoMock.Object,
            _aparelhoRepoMock.Object,
            _tecnicoRepoMock.Object);
    }

    [Fact]
    public async Task CriarAsync_SemCliente_DeveLancarExcecao()
    {
        var dto = new CriarOrdemServicoDto
        {
            ClienteId = 999,
            AparelhoId = 1,
            DefeitoRelatado = "Tela quebrada"
        };

        _clienteRepoMock.Setup(r => r.ObterPorIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Equal("Cliente não encontrado.", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_SemAparelho_DeveLancarExcecao()
    {
        var dto = new CriarOrdemServicoDto
        {
            ClienteId = 1,
            AparelhoId = 999,
            DefeitoRelatado = "Tela quebrada"
        };

        _clienteRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Cliente { Id = 1, Nome = "João" });

        _aparelhoRepoMock.Setup(r => r.ObterPorIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Aparelho?)null);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Equal("Aparelho não encontrado.", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_SemDefeito_DeveLancarExcecao()
    {
        var dto = new CriarOrdemServicoDto
        {
            ClienteId = 1,
            AparelhoId = 1,
            DefeitoRelatado = ""
        };

        _clienteRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Cliente { Id = 1, Nome = "João" });

        _aparelhoRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Aparelho { Id = 1, Marca = "Samsung", Modelo = "Galaxy" });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Equal("O defeito relatado é obrigatório.", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_DadosValidos_DeveCriarComStatusAberta()
    {
        var dto = new CriarOrdemServicoDto
        {
            ClienteId = 1,
            AparelhoId = 1,
            DefeitoRelatado = "Tela quebrada"
        };

        _clienteRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Cliente { Id = 1, Nome = "João" });

        _aparelhoRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Aparelho { Id = 1, Marca = "Samsung", Modelo = "Galaxy" });

        _ordemRepoMock.Setup(r => r.GerarProximoNumeroAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("OS0001");

        var resultado = await _service.CriarAsync(dto);

        Assert.NotNull(resultado);
        Assert.Equal(StatusOrdemServico.Aberta, resultado.Status);
        Assert.Equal("OS0001", resultado.Numero);
        Assert.Equal("João", resultado.ClienteNome);
        Assert.Equal("Samsung Galaxy", resultado.AparelhoDescricao);
    }

    [Fact]
    public async Task CriarAsync_DeveGerarNumeroAutomatico()
    {
        var dto = new CriarOrdemServicoDto
        {
            ClienteId = 1,
            AparelhoId = 1,
            DefeitoRelatado = "Tela quebrada"
        };

        _clienteRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Cliente { Id = 1, Nome = "João" });

        _aparelhoRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Aparelho { Id = 1, Marca = "Samsung", Modelo = "Galaxy" });

        _ordemRepoMock.Setup(r => r.GerarProximoNumeroAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("OS0042");

        var resultado = await _service.CriarAsync(dto);

        Assert.Equal("OS0042", resultado.Numero);
        _ordemRepoMock.Verify(r => r.GerarProximoNumeroAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void CalcularValorTotal_DeveSomarServicoEPecasESubtrairDesconto()
    {
        var ordem = new OrdemServico
        {
            ValorServico = 100,
            ValorPecas = 50,
            Desconto = 20
        };

        ordem.CalcularValorTotal();

        Assert.Equal(130, ordem.ValorTotal);
    }

    [Fact]
    public void CalcularValorTotal_PodeSerZero()
    {
        var ordem = new OrdemServico
        {
            ValorServico = 0,
            ValorPecas = 0,
            Desconto = 0
        };

        ordem.CalcularValorTotal();

        Assert.Equal(0, ordem.ValorTotal);
    }

    [Fact]
    public void SaldoPendente_DeveSerTotalMenosPago()
    {
        var ordem = new OrdemServico
        {
            ValorTotal = 200,
            ValorPago = 80
        };

        Assert.Equal(120, ordem.SaldoPendente);
    }

    [Fact]
    public async Task AlterarStatusAsync_DeAbertaParaEmAnalise_DevePermitir()
    {
        var ordem = new OrdemServico
        {
            Id = 1,
            Status = StatusOrdemServico.Aberta,
            Cliente = new Cliente { Nome = "João" },
            Aparelho = new Aparelho { Marca = "Samsung", Modelo = "Galaxy" }
        };

        _ordemRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordem);

        await _service.AlterarStatusAsync(1, StatusOrdemServico.EmAnalise);

        Assert.Equal(StatusOrdemServico.EmAnalise, ordem.Status);
    }

    [Fact]
    public async Task AlterarStatusAsync_DeAbertaParaConcluida_DeveLancarExcecao()
    {
        var ordem = new OrdemServico
        {
            Id = 1,
            Status = StatusOrdemServico.Aberta,
            Cliente = new Cliente { Nome = "João" },
            Aparelho = new Aparelho { Marca = "Samsung", Modelo = "Galaxy" }
        };

        _ordemRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordem);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.AlterarStatusAsync(1, StatusOrdemServico.Concluida));

        Assert.Contains("Não é possível alterar o status", ex.Message);
    }
}
