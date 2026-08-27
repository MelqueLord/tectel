using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;
using AssistenciaTecnica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Produto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Produto?> ObterPorCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .FirstOrDefaultAsync(p => p.Codigo == codigo, cancellationToken);
    }

    public async Task<IEnumerable<Produto>> ListarAtivosAsync(int limite = 100, CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .AsNoTracking()
            .Where(p => p.Ativo)
            .OrderBy(p => p.Nome)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Produto>> ListarEstoqueBaixoAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Produtos
            .AsNoTracking()
            .Where(p => p.Ativo && p.QuantidadeEstoque <= p.EstoqueMinimo)
            .OrderBy(p => p.QuantidadeEstoque)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Produto>> PesquisarAsync(string termo, int limite = 100, CancellationToken cancellationToken = default)
    {
        var termoLower = termo.ToLower();

        return await _context.Produtos
            .AsNoTracking()
            .Where(p =>
                p.Nome.ToLower().Contains(termoLower) ||
                p.Codigo.Contains(termo) ||
                (p.Categoria != null && p.Categoria.ToLower().Contains(termoLower)))
            .OrderBy(p => p.Nome)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task AdicionarAsync(Produto produto, CancellationToken cancellationToken = default)
    {
        await _context.Produtos.AddAsync(produto, cancellationToken);
    }

    public async Task AtualizarAsync(Produto produto, CancellationToken cancellationToken = default)
    {
        _context.Produtos.Update(produto);
        await Task.CompletedTask;
    }

    public async Task SalvarAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
