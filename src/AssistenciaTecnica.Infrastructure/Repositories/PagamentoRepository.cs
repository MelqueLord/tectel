using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;
using AssistenciaTecnica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Infrastructure.Repositories;

public class PagamentoRepository : IPagamentoRepository
{
    private readonly AppDbContext _context;

    public PagamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Pagamento>> ListarPorOrdemAsync(int ordemServicoId, CancellationToken cancellationToken = default)
    {
        return await _context.Pagamentos
            .AsNoTracking()
            .Where(p => p.OrdemServicoId == ordemServicoId)
            .OrderByDescending(p => p.DataPagamento)
            .ToListAsync(cancellationToken);
    }

    public async Task AdicionarAsync(Pagamento pagamento, CancellationToken cancellationToken = default)
    {
        await _context.Pagamentos.AddAsync(pagamento, cancellationToken);
    }

    public async Task SalvarAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
