using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Interfaces;

public interface IMovimentacaoEstoqueRepository
{
    Task<IEnumerable<MovimentacaoEstoque>> ListarAsync(int limite = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<MovimentacaoEstoque>> ListarPorProdutoAsync(int produtoId, CancellationToken cancellationToken = default);
    Task AdicionarAsync(MovimentacaoEstoque movimentacao, CancellationToken cancellationToken = default);
    Task SalvarAsync(CancellationToken cancellationToken = default);
}
