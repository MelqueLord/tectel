using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Interfaces;

public interface IOrdemServicoRepository
{
    Task<OrdemServico?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OrdemServico?> ObterPorNumeroAsync(string numero, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrdemServico>> ListarAsync(int limite = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrdemServico>> ListarPorStatusAsync(StatusOrdemServico status, int limite = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrdemServico>> PesquisarAsync(string termo, int limite = 100, CancellationToken cancellationToken = default);
    Task<int> ContarPorStatusAsync(StatusOrdemServico status, CancellationToken cancellationToken = default);
    Task<string> GerarProximoNumeroAsync(CancellationToken cancellationToken = default);
    Task AdicionarAsync(OrdemServico ordem, CancellationToken cancellationToken = default);
    Task AtualizarAsync(OrdemServico ordem, CancellationToken cancellationToken = default);
    Task SalvarAsync(CancellationToken cancellationToken = default);
}
