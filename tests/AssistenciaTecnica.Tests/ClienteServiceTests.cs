using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Application.Services;
using AssistenciaTecnica.Domain.Entities;
using Moq;

namespace AssistenciaTecnica.Tests;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _repositoryMock;
    private readonly ClienteService _service;

    public ClienteServiceTests()
    {
        _repositoryMock = new Mock<IClienteRepository>();
        _service = new ClienteService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CriarAsync_ClienteValido_DeveCriar()
    {
        var dto = new CriarClienteDto
        {
            Nome = "João Silva",
            Telefone = "11999998888",
            Email = "joao@email.com"
        };

        _repositoryMock.Setup(r => r.ExisteCpfCnpjAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var resultado = await _service.CriarAsync(dto);

        Assert.NotNull(resultado);
        Assert.Equal("João Silva", resultado.Nome);
        Assert.True(resultado.Ativo);
        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SalvarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_SemNome_DeveLancarExcecao()
    {
        var dto = new CriarClienteDto
        {
            Nome = "",
            Telefone = "11999998888"
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Equal("O nome é obrigatório.", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_NomeMenorQue3_DeveLancarExcecao()
    {
        var dto = new CriarClienteDto
        {
            Nome = "Ab",
            Telefone = "11999998888"
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Equal("O nome deve possuir pelo menos 3 caracteres.", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_SemTelefoneESemWhatsApp_DeveLancarExcecao()
    {
        var dto = new CriarClienteDto
        {
            Nome = "João Silva"
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Equal("Informe um telefone ou WhatsApp.", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_EmailInvalido_DeveLancarExcecao()
    {
        var dto = new CriarClienteDto
        {
            Nome = "João Silva",
            Telefone = "11999998888",
            Email = "email-invalido"
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Equal("O e-mail informado é inválido.", ex.Message);
    }

    [Fact]
    public async Task CriarAsync_CpfCnpjDuplicado_DeveLancarExcecao()
    {
        var dto = new CriarClienteDto
        {
            Nome = "João Silva",
            Telefone = "11999998888",
            CpfCnpj = "12345678901"
        };

        _repositoryMock.Setup(r => r.ExisteCpfCnpjAsync("12345678901", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
        Assert.Equal("Já existe um cliente com este CPF ou CNPJ.", ex.Message);
    }

    [Fact]
    public async Task AtualizarAsync_ClienteExistente_DeveAtualizar()
    {
        var cliente = new Cliente
        {
            Id = 1,
            Nome = "João Silva",
            Telefone = "11999998888",
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);
        _repositoryMock.Setup(r => r.ExisteCpfCnpjAsync(It.IsAny<string>(), 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var dto = new EditarClienteDto
        {
            Id = 1,
            Nome = "João Silva Atualizado",
            Telefone = "11988887777",
            Ativo = true
        };

        var resultado = await _service.AtualizarAsync(dto);

        Assert.Equal("João Silva Atualizado", resultado.Nome);
        Assert.NotNull(resultado.DataAtualizacao);
    }

    [Fact]
    public async Task InativarAsync_ClienteExistente_DeveInativar()
    {
        var cliente = new Cliente
        {
            Id = 1,
            Nome = "João Silva",
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        await _service.InativarAsync(1);

        Assert.False(cliente.Ativo);
        Assert.NotNull(cliente.DataAtualizacao);
    }

    [Fact]
    public async Task ReativarAsync_ClienteExistente_DeveReativar()
    {
        var cliente = new Cliente
        {
            Id = 1,
            Nome = "João Silva",
            Ativo = false,
            DataCadastro = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.ObterPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        await _service.ReativarAsync(1);

        Assert.True(cliente.Ativo);
        Assert.NotNull(cliente.DataAtualizacao);
    }

    [Fact]
    public async Task CriarAsync_DeveNormalizarDados()
    {
        var dto = new CriarClienteDto
        {
            Nome = "  João Silva  ",
            CpfCnpj = "123.456.789-01",
            Telefone = "(11) 99999-8888",
            WhatsApp = "(11) 98888-7777",
            Email = "  JOAO@EMAIL.COM  ",
            Cep = "12345-678"
        };

        _repositoryMock.Setup(r => r.ExisteCpfCnpjAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var resultado = await _service.CriarAsync(dto);

        Assert.Equal("João Silva", resultado.Nome);
        Assert.Equal("12345678901", resultado.CpfCnpj);
        Assert.Equal("11999998888", resultado.Telefone);
        Assert.Equal("11988887777", resultado.WhatsApp);
        Assert.Equal("joao@email.com", resultado.Email);
        Assert.Equal("12345678", resultado.Cep);
    }
}
