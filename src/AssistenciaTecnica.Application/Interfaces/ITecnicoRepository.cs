using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Interfaces;

public interface ITecnicoRepository
{
    Task<Tecnico?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tecnico>> ListarAtivosAsync(int limite = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tecnico>> ListarTodosAsync(int limite = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tecnico>> PesquisarAsync(string termo, int limite = 100, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Tecnico tecnico, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Tecnico tecnico, CancellationToken cancellationToken = default);
    Task SalvarAsync(CancellationToken cancellationToken = default);
}
