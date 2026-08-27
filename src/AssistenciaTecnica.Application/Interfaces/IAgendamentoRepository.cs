using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Interfaces;

public interface IAgendamentoRepository
{
    Task<Agendamento?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ListarAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ListarPorPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ListarPorOrdemAsync(int ordemServicoId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ListarPorClienteAsync(int clienteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Agendamento>> ListarHojeAsync(CancellationToken cancellationToken = default);
    Task AdicionarAsync(Agendamento agendamento, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Agendamento agendamento, CancellationToken cancellationToken = default);
    Task SalvarAsync(CancellationToken cancellationToken = default);
}
