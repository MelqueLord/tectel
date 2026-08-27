using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Application.Services;
using AssistenciaTecnica.Domain.Entities;
using Moq;

namespace AssistenciaTecnica.Tests;

public class AparelhoServiceTests
{
    private readonly Mock<IAparelhoRepository> _aparelhoRepoMock;
    private readonly Mock<IClienteRepository> _clienteRepoMock;
    private readonly AparelhoService _service;

    public AparelhoServiceTests()
    {
        _aparelhoRepoMock = new Mock<IAparelhoRepository>();
        _clienteRepoMock = new Mock<IClienteRepository>();
        _service = new AparelhoService(_aparelhoRepoMock.Object, _clienteRepoMock.Object);
    }

    [Fact]
    public async Task CriarAsync_SemCliente_DeveLancarExcecao()
    {
        var dto = new CriarAparelhoDto
        {
            ClienteId = 999,
            Tipo = "Celular",
            Marca = "Samsung",
            Modelo = "Galaxy S23"
        };

        _clienteRepoMock.Setup(r => r.ObterPorIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Equal("Cliente não encontrado.", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_SemMarca_DeveLancarExcecao()
    {
        var dto = new CriarAparelhoDto
        {
            ClienteId = 1,
            Tipo = "Celular",
            Marca = "",
            Modelo = "Galaxy S23"
        };

        _clienteRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Cliente { Id = 1, Nome = "João" });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Equal("A marca é obrigatória.", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_SemModelo_DeveLancarExcecao()
    {
        var dto = new CriarAparelhoDto
        {
            ClienteId = 1,
            Tipo = "Celular",
            Marca = "Samsung",
            Modelo = ""
        };

        _clienteRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Cliente { Id = 1, Nome = "João" });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Equal("O modelo é obrigatório.", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_DadosValidos_DeveCriar()
    {
        var cliente = new Cliente { Id = 1, Nome = "João Silva" };
        var dto = new CriarAparelhoDto
        {
            ClienteId = 1,
            Tipo = "Celular",
            Marca = "Samsung",
            Modelo = "Galaxy S23",
            Cor = "Preto",
            Imei = "123456789"
        };

        _clienteRepoMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        var resultado = await _service.CriarAsync(dto);

        Assert.NotNull(resultado);
        Assert.Equal("Samsung", resultado.Marca);
        Assert.Equal("Galaxy S23", resultado.Modelo);
        Assert.Equal("João Silva", resultado.ClienteNome);
        _aparelhoRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Aparelho>(), It.IsAny<CancellationToken>()), Times.Once);
        _aparelhoRepoMock.Verify(r => r.SalvarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
