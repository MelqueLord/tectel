using AssistenciaTecnica.Application.DTOs;

namespace AssistenciaTecnica.Application.Interfaces;

public interface IPagamentoService
{
    Task<PagamentoDto> RegistrarAsync(RegistrarPagamentoDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<PagamentoDto>> ListarPorOrdemAsync(int ordemServicoId, CancellationToken cancellationToken = default);
}
