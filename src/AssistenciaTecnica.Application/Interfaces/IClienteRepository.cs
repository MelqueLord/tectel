using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Interfaces;

public interface IClienteRepository
{
    Task<Cliente?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Cliente?> ObterPorCpfCnpjAsync(string cpfCnpj, CancellationToken cancellationToken = default);
    Task<bool> ExisteCpfCnpjAsync(string cpfCnpj, int? excluirId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cliente>> ListarAtivosAsync(int limite = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cliente>> ListarInativosAsync(int limite = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cliente>> ListarTodosAsync(int limite = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cliente>> PesquisarAsync(string termo, int limite = 100, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task SalvarAsync(CancellationToken cancellationToken = default);
}
