using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Interfaces;

public interface IAparelhoRepository
{
    Task<Aparelho?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Aparelho>> ListarAsync(int limite = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<Aparelho>> ListarPorClienteAsync(int clienteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Aparelho>> PesquisarAsync(string termo, int limite = 100, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Aparelho aparelho, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Aparelho aparelho, CancellationToken cancellationToken = default);
    Task SalvarAsync(CancellationToken cancellationToken = default);
}
