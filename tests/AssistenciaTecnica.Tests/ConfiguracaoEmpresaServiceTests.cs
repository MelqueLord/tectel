using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Application.Services;
using AssistenciaTecnica.Domain.Entities;
using Moq;

namespace AssistenciaTecnica.Tests;

public class ConfiguracaoEmpresaServiceTests
{
    private readonly Mock<IConfiguracaoEmpresaRepository> _repoMock;
    private readonly ConfiguracaoEmpresaService _service;

    public ConfiguracaoEmpresaServiceTests()
    {
        _repoMock = new Mock<IConfiguracaoEmpresaRepository>();
        _service = new ConfiguracaoEmpresaService(_repoMock.Object);
    }

    [Fact]
    public async Task ObterAsync_SemConfiguracao_DeveRetornarNull()
    {
        _repoMock.Setup(r => r.ObterAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConfiguracaoEmpresa?)null);

        var resultado = await _service.ObterAsync();

        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObterAsync_ComConfiguracao_DeveRetornarDto()
    {
        var config = new ConfiguracaoEmpresa
        {
            Id = 1,
            Nome = "TechCel",
            CpfCnpj = "12345678000190",
            Telefone = "11999998888",
            PrazoPadraoGarantiaDias = 90
        };

        _repoMock.Setup(r => r.ObterAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        var resultado = await _service.ObterAsync();

        Assert.NotNull(resultado);
        Assert.Equal("TechCel", resultado.Nome);
        Assert.Equal(90, resultado.PrazoPadraoGarantiaDias);
    }

    [Fact]
    public async Task SalvarAsync_SemNome_DeveLancarExcecao()
    {
        var dto = new ConfiguracaoEmpresaDto { Nome = "" };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.SalvarAsync(dto));
        Assert.Equal("O nome da assistência é obrigatório.", ex.Message);
    }

    [Fact]
    public async Task SalvarAsync_NovaConfiguracao_DeveAdicionar()
    {
        var dto = new ConfiguracaoEmpresaDto
        {
            Nome = "TechCel",
            Telefone = "11999998888",
            PrazoPadraoGarantiaDias = 60
        };

        _repoMock.Setup(r => r.ObterAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConfiguracaoEmpresa?)null);

        await _service.SalvarAsync(dto);

        _repoMock.Verify(r => r.SalvarAsync(
            It.Is<ConfiguracaoEmpresa>(c =>
                c.Nome == "TechCel" &&
                c.PrazoPadraoGarantiaDias == 60),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SalvarAsync_ConfiguracaoExistente_DeveAtualizar()
    {
        var existente = new ConfiguracaoEmpresa
        {
            Id = 1,
            Nome = "Antigo",
            PrazoPadraoGarantiaDias = 30
        };

        var dto = new ConfiguracaoEmpresaDto
        {
            Id = 1,
            Nome = "Atualizado",
            PrazoPadraoGarantiaDias = 90
        };

        _repoMock.Setup(r => r.ObterAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(existente);

        await _service.SalvarAsync(dto);

        _repoMock.Verify(r => r.SalvarAsync(
            It.Is<ConfiguracaoEmpresa>(c =>
                c.Id == 1 &&
                c.Nome == "Atualizado" &&
                c.PrazoPadraoGarantiaDias == 90),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SalvarAsync_DeveNormalizarTexto()
    {
        var dto = new ConfiguracaoEmpresaDto
        {
            Nome = "  TechCel  ",
            Telefone = "  (11) 99999-8888  ",
            Endereco = "  Rua A, 123  "
        };

        _repoMock.Setup(r => r.ObterAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConfiguracaoEmpresa?)null);

        await _service.SalvarAsync(dto);

        _repoMock.Verify(r => r.SalvarAsync(
            It.Is<ConfiguracaoEmpresa>(c =>
                c.Nome == "TechCel" &&
                c.Telefone == "(11) 99999-8888" &&
                c.Endereco == "Rua A, 123"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
