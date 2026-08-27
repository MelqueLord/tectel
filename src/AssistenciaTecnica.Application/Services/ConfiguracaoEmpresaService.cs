using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Services;

public class ConfiguracaoEmpresaService : IConfiguracaoEmpresaService
{
    private readonly IConfiguracaoEmpresaRepository _repository;

    public ConfiguracaoEmpresaService(IConfiguracaoEmpresaRepository repository)
    {
        _repository = repository;
    }

    public async Task<ConfiguracaoEmpresaDto?> ObterAsync(CancellationToken cancellationToken = default)
    {
        var config = await _repository.ObterAsync(cancellationToken);
        return config is null ? null : MapearParaDto(config);
    }

    public async Task SalvarAsync(ConfiguracaoEmpresaDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new InvalidOperationException("O nome da assistência é obrigatório.");

        var config = await _repository.ObterAsync(cancellationToken) ?? new ConfiguracaoEmpresa();

        config.Nome = dto.Nome.Trim();
        config.CpfCnpj = NormalizarTexto(dto.CpfCnpj);
        config.Telefone = NormalizarTexto(dto.Telefone);
        config.WhatsApp = NormalizarTexto(dto.WhatsApp);
        config.Endereco = NormalizarTexto(dto.Endereco);
        config.LogoCaminho = NormalizarTexto(dto.LogoCaminho);
        config.TextoPadraoOrdem = NormalizarTexto(dto.TextoPadraoOrdem);
        config.PrazoPadraoGarantiaDias = dto.PrazoPadraoGarantiaDias;

        await _repository.SalvarAsync(config, cancellationToken);
    }

    private static string? NormalizarTexto(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return null;
        var trimmed = valor.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

    private static ConfiguracaoEmpresaDto MapearParaDto(ConfiguracaoEmpresa config)
    {
        return new ConfiguracaoEmpresaDto
        {
            Id = config.Id,
            Nome = config.Nome,
            CpfCnpj = config.CpfCnpj,
            Telefone = config.Telefone,
            WhatsApp = config.WhatsApp,
            Endereco = config.Endereco,
            LogoCaminho = config.LogoCaminho,
            TextoPadraoOrdem = config.TextoPadraoOrdem,
            PrazoPadraoGarantiaDias = config.PrazoPadraoGarantiaDias
        };
    }
}
