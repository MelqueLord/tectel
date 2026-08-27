using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;

    public ClienteService(IClienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<ClienteDto> CriarAsync(CriarClienteDto dto, CancellationToken cancellationToken = default)
    {
        ValidarNome(dto.Nome);
        ValidarContato(dto.Telefone, dto.WhatsApp);
        ValidarEmail(dto.Email);

        var cpfCnpjNormalizado = NormalizarCpfCnpj(dto.CpfCnpj);
        if (!string.IsNullOrWhiteSpace(cpfCnpjNormalizado))
        {
            if (await _repository.ExisteCpfCnpjAsync(cpfCnpjNormalizado, null, cancellationToken))
                throw new InvalidOperationException("Já existe um cliente com este CPF ou CNPJ.");
        }

        var cliente = new Cliente
        {
            Nome = dto.Nome.Trim(),
            CpfCnpj = cpfCnpjNormalizado,
            Telefone = NormalizarNumero(dto.Telefone),
            WhatsApp = NormalizarNumero(dto.WhatsApp),
            Email = NormalizarEmail(dto.Email),
            Endereco = NormalizarTexto(dto.Endereco),
            Bairro = NormalizarTexto(dto.Bairro),
            Cidade = NormalizarTexto(dto.Cidade),
            Estado = NormalizarTexto(dto.Estado)?.ToUpperInvariant(),
            Cep = NormalizarNumero(dto.Cep),
            Observacoes = NormalizarTexto(dto.Observacoes),
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        await _repository.AdicionarAsync(cliente, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);

        return MapearParaDto(cliente);
    }

    public async Task<ClienteDto> AtualizarAsync(EditarClienteDto dto, CancellationToken cancellationToken = default)
    {
        var cliente = await _repository.ObterPorIdAsync(dto.Id, cancellationToken)
            ?? throw new InvalidOperationException("Cliente não encontrado.");

        ValidarNome(dto.Nome);
        ValidarContato(dto.Telefone, dto.WhatsApp);
        ValidarEmail(dto.Email);

        var cpfCnpjNormalizado = NormalizarCpfCnpj(dto.CpfCnpj);
        if (!string.IsNullOrWhiteSpace(cpfCnpjNormalizado))
        {
            if (await _repository.ExisteCpfCnpjAsync(cpfCnpjNormalizado, dto.Id, cancellationToken))
                throw new InvalidOperationException("Já existe um cliente com este CPF ou CNPJ.");
        }

        cliente.Nome = dto.Nome.Trim();
        cliente.CpfCnpj = cpfCnpjNormalizado;
        cliente.Telefone = NormalizarNumero(dto.Telefone);
        cliente.WhatsApp = NormalizarNumero(dto.WhatsApp);
        cliente.Email = NormalizarEmail(dto.Email);
        cliente.Endereco = NormalizarTexto(dto.Endereco);
        cliente.Bairro = NormalizarTexto(dto.Bairro);
        cliente.Cidade = NormalizarTexto(dto.Cidade);
        cliente.Estado = NormalizarTexto(dto.Estado)?.ToUpperInvariant();
        cliente.Cep = NormalizarNumero(dto.Cep);
        cliente.Observacoes = NormalizarTexto(dto.Observacoes);
        cliente.Ativo = dto.Ativo;
        cliente.DataAtualizacao = DateTime.UtcNow;

        await _repository.AtualizarAsync(cliente, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);

        return MapearParaDto(cliente);
    }

    public async Task<ClienteDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cliente = await _repository.ObterPorIdAsync(id, cancellationToken);
        return cliente is null ? null : MapearParaDto(cliente);
    }

    public async Task<IEnumerable<ClienteListagemDto>> ListarAtivosAsync(CancellationToken cancellationToken = default)
    {
        var clientes = await _repository.ListarAtivosAsync(100, cancellationToken);
        return clientes.Select(MapearParaListagemDto);
    }

    public async Task<IEnumerable<ClienteListagemDto>> ListarInativosAsync(CancellationToken cancellationToken = default)
    {
        var clientes = await _repository.ListarInativosAsync(100, cancellationToken);
        return clientes.Select(MapearParaListagemDto);
    }

    public async Task<IEnumerable<ClienteListagemDto>> ListarTodosAsync(CancellationToken cancellationToken = default)
    {
        var clientes = await _repository.ListarTodosAsync(100, cancellationToken);
        return clientes.Select(MapearParaListagemDto);
    }

    public async Task<IEnumerable<ClienteListagemDto>> PesquisarAsync(string termo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return await ListarAtivosAsync(cancellationToken);

        var clientes = await _repository.PesquisarAsync(termo.Trim(), 100, cancellationToken);
        return clientes.Select(MapearParaListagemDto);
    }

    public async Task InativarAsync(int id, CancellationToken cancellationToken = default)
    {
        var cliente = await _repository.ObterPorIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Cliente não encontrado.");

        cliente.Ativo = false;
        cliente.DataAtualizacao = DateTime.UtcNow;

        await _repository.AtualizarAsync(cliente, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);
    }

    public async Task ReativarAsync(int id, CancellationToken cancellationToken = default)
    {
        var cliente = await _repository.ObterPorIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Cliente não encontrado.");

        cliente.Ativo = true;
        cliente.DataAtualizacao = DateTime.UtcNow;

        await _repository.AtualizarAsync(cliente, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new InvalidOperationException("O nome é obrigatório.");

        if (nome.Trim().Length < 3)
            throw new InvalidOperationException("O nome deve possuir pelo menos 3 caracteres.");
    }

    private static void ValidarContato(string? telefone, string? whatsApp)
    {
        if (string.IsNullOrWhiteSpace(telefone) && string.IsNullOrWhiteSpace(whatsApp))
            throw new InvalidOperationException("Informe um telefone ou WhatsApp.");
    }

    private static void ValidarEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            if (addr.Address != email.Trim())
                throw new InvalidOperationException("O e-mail informado é inválido.");
        }
        catch
        {
            throw new InvalidOperationException("O e-mail informado é inválido.");
        }
    }

    private static string? NormalizarCpfCnpj(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return null;

        var numeros = new string(valor.Where(char.IsDigit).ToArray());
        return string.IsNullOrWhiteSpace(numeros) ? null : numeros;
    }

    private static string? NormalizarNumero(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return null;

        var numeros = new string(valor.Where(char.IsDigit).ToArray());
        return string.IsNullOrWhiteSpace(numeros) ? null : numeros;
    }

    private static string? NormalizarEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return email.Trim().ToLowerInvariant();
    }

    private static string? NormalizarTexto(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return null;

        var trimmed = valor.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

    private static ClienteDto MapearParaDto(Cliente cliente)
    {
        return new ClienteDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            CpfCnpj = cliente.CpfCnpj,
            Telefone = cliente.Telefone,
            WhatsApp = cliente.WhatsApp,
            Email = cliente.Email,
            Endereco = cliente.Endereco,
            Bairro = cliente.Bairro,
            Cidade = cliente.Cidade,
            Estado = cliente.Estado,
            Cep = cliente.Cep,
            Observacoes = cliente.Observacoes,
            Ativo = cliente.Ativo,
            DataCadastro = cliente.DataCadastro,
            DataAtualizacao = cliente.DataAtualizacao
        };
    }

    private static ClienteListagemDto MapearParaListagemDto(Cliente cliente)
    {
        return new ClienteListagemDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            CpfCnpj = cliente.CpfCnpj,
            Telefone = cliente.Telefone,
            WhatsApp = cliente.WhatsApp,
            Cidade = cliente.Cidade,
            Ativo = cliente.Ativo,
            DataCadastro = cliente.DataCadastro
        };
    }
}
