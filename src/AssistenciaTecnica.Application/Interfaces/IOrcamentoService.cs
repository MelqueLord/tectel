using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Interfaces;

public interface IOrcamentoService
{
    Task<OrcamentoDto> CriarAsync(CriarOrcamentoDto dto, CancellationToken cancellationToken = default);
    Task<OrcamentoDto> AtualizarAsync(EditarOrcamentoDto dto, CancellationToken cancellationToken = default);
    Task<OrcamentoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrcamentoListagemDto>> ListarAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<OrcamentoListagemDto>> ListarPorStatusAsync(StatusOrcamento status, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrcamentoListagemDto>> PesquisarAsync(string termo, CancellationToken cancellationToken = default);
    Task AlterarStatusAsync(int id, StatusOrcamento novoStatus, CancellationToken cancellationToken = default);
    Task<OrdemServicoDto> ConverterEmOrdemServicoAsync(int orcamentoId, CancellationToken cancellationToken = default);
}
