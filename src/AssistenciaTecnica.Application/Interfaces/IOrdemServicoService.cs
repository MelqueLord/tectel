using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Interfaces;

public interface IOrdemServicoService
{
    Task<OrdemServicoDto> CriarAsync(CriarOrdemServicoDto dto, CancellationToken cancellationToken = default);
    Task<OrdemServicoDto> AtualizarAsync(EditarOrdemServicoDto dto, CancellationToken cancellationToken = default);
    Task<OrdemServicoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrdemServicoListagemDto>> ListarAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<OrdemServicoListagemDto>> ListarPorStatusAsync(StatusOrdemServico status, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrdemServicoListagemDto>> PesquisarAsync(string termo, CancellationToken cancellationToken = default);
    Task AlterarStatusAsync(int id, StatusOrdemServico novoStatus, CancellationToken cancellationToken = default);
    Task<DashboardDto> ObterDashboardAsync(CancellationToken cancellationToken = default);
}
