using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Interfaces;

public interface IAgendamentoService
{
    Task<AgendamentoDto> CriarAsync(CriarAgendamentoDto dto, CancellationToken cancellationToken = default);
    Task<AgendamentoDto> AtualizarAsync(AgendamentoFormDto dto, CancellationToken cancellationToken = default);
    Task<AgendamentoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AgendamentoDto>> ListarAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AgendamentoCalendarioDto>> ListarCalendarioAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default);
    Task<IEnumerable<AgendamentoDto>> ListarHojeAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AgendamentoDto>> ListarPorOrdemAsync(int ordemServicoId, CancellationToken cancellationToken = default);
    Task AlterarStatusAsync(int id, StatusAgendamento novoStatus, CancellationToken cancellationToken = default);
}
