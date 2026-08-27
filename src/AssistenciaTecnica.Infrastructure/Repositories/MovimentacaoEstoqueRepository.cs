using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;
using AssistenciaTecnica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Infrastructure.Repositories;

public class MovimentacaoEstoqueRepository : IMovimentacaoEstoqueRepository
{
    private readonly AppDbContext _context;

    public MovimentacaoEstoqueRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> ListarAsync(int limite = 100, CancellationToken cancellationToken = default)
    {
        return await _context.MovimentacoesEstoque
            .AsNoTracking()
            .Include(m => m.Produto)
            .OrderByDescending(m => m.DataMovimentacao)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> ListarPorProdutoAsync(int produtoId, CancellationToken cancellationToken = default)
    {
        return await _context.MovimentacoesEstoque
            .AsNoTracking()
            .Include(m => m.Produto)
            .Where(m => m.ProdutoId == produtoId)
            .OrderByDescending(m => m.DataMovimentacao)
            .ToListAsync(cancellationToken);
    }

    public async Task AdicionarAsync(MovimentacaoEstoque movimentacao, CancellationToken cancellationToken = default)
    {
        await _context.MovimentacoesEstoque.AddAsync(movimentacao, cancellationToken);
    }

    public async Task SalvarAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
