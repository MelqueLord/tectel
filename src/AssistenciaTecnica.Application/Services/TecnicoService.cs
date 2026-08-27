using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Services;

public class TecnicoService : ITecnicoService
{
    private readonly ITecnicoRepository _repository;

    public TecnicoService(ITecnicoRepository repository)
    {
        _repository = repository;
    }

    public async Task<TecnicoDto> CriarAsync(TecnicoFormDto dto, CancellationToken cancellationToken = default)
    {
        ValidarNome(dto.Nome);

        var tecnico = new Tecnico
        {
            Nome = dto.Nome.Trim(),
            Telefone = NormalizarNumero(dto.Telefone),
            WhatsApp = NormalizarNumero(dto.WhatsApp),
            Email = NormalizarEmail(dto.Email),
            Especialidade = NormalizarTexto(dto.Especialidade),
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        await _repository.AdicionarAsync(tecnico, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);

        return MapearParaDto(tecnico);
    }

    public async Task<TecnicoDto> AtualizarAsync(TecnicoFormDto dto, CancellationToken cancellationToken = default)
    {
        var tecnico = await _repository.ObterPorIdAsync(dto.Id ?? 0, cancellationToken)
            ?? throw new InvalidOperationException("Técnico não encontrado.");

        ValidarNome(dto.Nome);

        tecnico.Nome = dto.Nome.Trim();
        tecnico.Telefone = NormalizarNumero(dto.Telefone);
        tecnico.WhatsApp = NormalizarNumero(dto.WhatsApp);
        tecnico.Email = NormalizarEmail(dto.Email);
        tecnico.Especialidade = NormalizarTexto(dto.Especialidade);
        tecnico.Ativo = dto.Ativo;
        tecnico.DataAtualizacao = DateTime.UtcNow;

        await _repository.AtualizarAsync(tecnico, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);

        return MapearParaDto(tecnico);
    }

    public async Task<TecnicoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tecnico = await _repository.ObterPorIdAsync(id, cancellationToken);
        return tecnico is null ? null : MapearParaDto(tecnico);
    }

    public async Task<IEnumerable<TecnicoListagemDto>> ListarAtivosAsync(CancellationToken cancellationToken = default)
    {
        var tecnicos = await _repository.ListarAtivosAsync(100, cancellationToken);
        return tecnicos.Select(MapearParaListagemDto);
    }

    public async Task<IEnumerable<TecnicoListagemDto>> ListarTodosAsync(CancellationToken cancellationToken = default)
    {
        var tecnicos = await _repository.ListarTodosAsync(100, cancellationToken);
        return tecnicos.Select(MapearParaListagemDto);
    }

    public async Task<IEnumerable<TecnicoListagemDto>> PesquisarAsync(string termo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return await ListarAtivosAsync(cancellationToken);

        var tecnicos = await _repository.PesquisarAsync(termo.Trim(), 100, cancellationToken);
        return tecnicos.Select(MapearParaListagemDto);
    }

    public async Task InativarAsync(int id, CancellationToken cancellationToken = default)
    {
        var tecnico = await _repository.ObterPorIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Técnico não encontrado.");

        tecnico.Ativo = false;
        tecnico.DataAtualizacao = DateTime.UtcNow;

        await _repository.AtualizarAsync(tecnico, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);
    }

    public async Task ReativarAsync(int id, CancellationToken cancellationToken = default)
    {
        var tecnico = await _repository.ObterPorIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Técnico não encontrado.");

        tecnico.Ativo = true;
        tecnico.DataAtualizacao = DateTime.UtcNow;

        await _repository.AtualizarAsync(tecnico, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new InvalidOperationException("O nome é obrigatório.");

        if (nome.Trim().Length < 3)
            throw new InvalidOperationException("O nome deve possuir pelo menos 3 caracteres.");
    }

    private static string? NormalizarNumero(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return null;
        var numeros = new string(valor.Where(char.IsDigit).ToArray());
        return string.IsNullOrWhiteSpace(numeros) ? null : numeros;
    }

    private static string? NormalizarEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        return email.Trim().ToLowerInvariant();
    }

    private static string? NormalizarTexto(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return null;
        var trimmed = valor.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

    private static TecnicoDto MapearParaDto(Tecnico tecnico)
    {
        return new TecnicoDto
        {
            Id = tecnico.Id,
            Nome = tecnico.Nome,
            Telefone = tecnico.Telefone,
            WhatsApp = tecnico.WhatsApp,
            Email = tecnico.Email,
            Especialidade = tecnico.Especialidade,
            Ativo = tecnico.Ativo,
            DataCadastro = tecnico.DataCadastro,
            DataAtualizacao = tecnico.DataAtualizacao
        };
    }

    private static TecnicoListagemDto MapearParaListagemDto(Tecnico tecnico)
    {
        return new TecnicoListagemDto
        {
            Id = tecnico.Id,
            Nome = tecnico.Nome,
            Telefone = tecnico.Telefone,
            WhatsApp = tecnico.WhatsApp,
            Especialidade = tecnico.Especialidade,
            Ativo = tecnico.Ativo,
            DataCadastro = tecnico.DataCadastro
        };
    }
}
