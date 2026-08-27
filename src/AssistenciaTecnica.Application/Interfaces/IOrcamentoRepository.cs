using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Interfaces;

public interface IOrcamentoRepository
{
    Task<Orcamento?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Orcamento?> ObterPorNumeroAsync(string numero, CancellationToken cancellationToken = default);
    Task<IEnumerable<Orcamento>> ListarAsync(int limite = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<Orcamento>> ListarPorStatusAsync(StatusOrcamento status, int limite = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<Orcamento>> PesquisarAsync(string termo, int limite = 100, CancellationToken cancellationToken = default);
    Task<int> ContarPorStatusAsync(StatusOrcamento status, CancellationToken cancellationToken = default);
    Task<string> GerarProximoNumeroAsync(CancellationToken cancellationToken = default);
    Task AdicionarAsync(Orcamento orcamento, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Orcamento orcamento, CancellationToken cancellationToken = default);
    Task SalvarAsync(CancellationToken cancellationToken = default);
}
