using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Interfaces;

public interface IProdutoRepository
{
    Task<Produto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Produto?> ObterPorCodigoAsync(string codigo, CancellationToken cancellationToken = default);
    Task<IEnumerable<Produto>> ListarAtivosAsync(int limite = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<Produto>> ListarEstoqueBaixoAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Produto>> PesquisarAsync(string termo, int limite = 100, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Produto produto, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Produto produto, CancellationToken cancellationToken = default);
    Task SalvarAsync(CancellationToken cancellationToken = default);
}
