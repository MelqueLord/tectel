using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Interfaces;

public interface IPagamentoRepository
{
    Task<IEnumerable<Pagamento>> ListarPorOrdemAsync(int ordemServicoId, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Pagamento pagamento, CancellationToken cancellationToken = default);
    Task SalvarAsync(CancellationToken cancellationToken = default);
}
