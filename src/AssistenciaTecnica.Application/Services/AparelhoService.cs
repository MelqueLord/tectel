using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Services;

public class AparelhoService : IAparelhoService
{
    private readonly IAparelhoRepository _repository;
    private readonly IClienteRepository _clienteRepository;

    public AparelhoService(IAparelhoRepository repository, IClienteRepository clienteRepository)
    {
        _repository = repository;
        _clienteRepository = clienteRepository;
    }

    public async Task<AparelhoDto> CriarAsync(CriarAparelhoDto dto, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(dto.ClienteId, cancellationToken)
            ?? throw new InvalidOperationException("Cliente não encontrado.");

        ValidarCamposObrigatorios(dto.Tipo, dto.Marca, dto.Modelo);

        var aparelho = new Aparelho
        {
            ClienteId = dto.ClienteId,
            Tipo = dto.Tipo.Trim(),
            Marca = dto.Marca.Trim(),
            Modelo = dto.Modelo.Trim(),
            Cor = NormalizarTexto(dto.Cor),
            Imei = NormalizarTexto(dto.Imei),
            NumeroSerie = NormalizarTexto(dto.NumeroSerie),
            SenhaDesbloqueio = NormalizarTexto(dto.SenhaDesbloqueio),
            EstadoAparelho = NormalizarTexto(dto.EstadoAparelho),
            ItensEntregues = NormalizarTexto(dto.ItensEntregues),
            Observacoes = NormalizarTexto(dto.Observacoes),
            DataCadastro = DateTime.UtcNow
        };

        await _repository.AdicionarAsync(aparelho, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);

        return MapearParaDto(aparelho, cliente.Nome);
    }

    public async Task<AparelhoDto> AtualizarAsync(EditarAparelhoDto dto, CancellationToken cancellationToken = default)
    {
        var aparelho = await _repository.ObterPorIdAsync(dto.Id, cancellationToken)
            ?? throw new InvalidOperationException("Aparelho não encontrado.");

        var cliente = await _clienteRepository.ObterPorIdAsync(dto.ClienteId, cancellationToken)
            ?? throw new InvalidOperationException("Cliente não encontrado.");

        ValidarCamposObrigatorios(dto.Tipo, dto.Marca, dto.Modelo);

        aparelho.ClienteId = dto.ClienteId;
        aparelho.Tipo = dto.Tipo.Trim();
        aparelho.Marca = dto.Marca.Trim();
        aparelho.Modelo = dto.Modelo.Trim();
        aparelho.Cor = NormalizarTexto(dto.Cor);
        aparelho.Imei = NormalizarTexto(dto.Imei);
        aparelho.NumeroSerie = NormalizarTexto(dto.NumeroSerie);
        aparelho.SenhaDesbloqueio = NormalizarTexto(dto.SenhaDesbloqueio);
        aparelho.EstadoAparelho = NormalizarTexto(dto.EstadoAparelho);
        aparelho.ItensEntregues = NormalizarTexto(dto.ItensEntregues);
        aparelho.Observacoes = NormalizarTexto(dto.Observacoes);

        await _repository.AtualizarAsync(aparelho, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);

        return MapearParaDto(aparelho, cliente.Nome);
    }

    public async Task<AparelhoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var aparelho = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (aparelho is null) return null;

        return MapearParaDto(aparelho, aparelho.Cliente?.Nome ?? "");
    }

    public async Task<IEnumerable<AparelhoDto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var aparelhos = await _repository.ListarAsync(100, cancellationToken);
        return aparelhos.Select(a => MapearParaDto(a, a.Cliente?.Nome ?? ""));
    }

    public async Task<IEnumerable<AparelhoDto>> ListarPorClienteAsync(int clienteId, CancellationToken cancellationToken = default)
    {
        var aparelhos = await _repository.ListarPorClienteAsync(clienteId, cancellationToken);
        return aparelhos.Select(a => MapearParaDto(a, a.Cliente?.Nome ?? ""));
    }

    public async Task<IEnumerable<AparelhoDto>> PesquisarAsync(string termo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return [];

        var aparelhos = await _repository.PesquisarAsync(termo.Trim(), 100, cancellationToken);
        return aparelhos.Select(a => MapearParaDto(a, a.Cliente?.Nome ?? ""));
    }

    private static void ValidarCamposObrigatorios(string tipo, string marca, string modelo)
    {
        if (string.IsNullOrWhiteSpace(tipo))
            throw new InvalidOperationException("O tipo é obrigatório.");
        if (string.IsNullOrWhiteSpace(marca))
            throw new InvalidOperationException("A marca é obrigatória.");
        if (string.IsNullOrWhiteSpace(modelo))
            throw new InvalidOperationException("O modelo é obrigatório.");
    }

    private static string? NormalizarTexto(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return null;
        var trimmed = valor.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

    private static AparelhoDto MapearParaDto(Aparelho aparelho, string clienteNome)
    {
        return new AparelhoDto
        {
            Id = aparelho.Id,
            ClienteId = aparelho.ClienteId,
            ClienteNome = clienteNome,
            Tipo = aparelho.Tipo,
            Marca = aparelho.Marca,
            Modelo = aparelho.Modelo,
            Cor = aparelho.Cor,
            Imei = aparelho.Imei,
            NumeroSerie = aparelho.NumeroSerie,
            EstadoAparelho = aparelho.EstadoAparelho,
            ItensEntregues = aparelho.ItensEntregues,
            Observacoes = aparelho.Observacoes,
            DataCadastro = aparelho.DataCadastro
        };
    }
}
