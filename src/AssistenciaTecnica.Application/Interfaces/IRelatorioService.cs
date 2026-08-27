namespace AssistenciaTecnica.Application.Interfaces;

using AssistenciaTecnica.Application.DTOs;

public interface IRelatorioService
{
    Task<RelatorioFaturamentoDto> ObterFaturamentoAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default);
    Task<RelatorioOrdensStatusDto> ObterOrdensPorStatusAsync(CancellationToken cancellationToken = default);
    Task<RelatorioPagamentosDto> ObterPagamentosPorFormaAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default);
    Task<RelatorioTopClientesDto> ObterTopClientesAsync(int quantidade = 10, CancellationToken cancellationToken = default);
    Task<RelatorioDashboardDto> ObterRelatorioDashboardAsync(CancellationToken cancellationToken = default);
}
