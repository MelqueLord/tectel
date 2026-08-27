using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;
using AssistenciaTecnica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Infrastructure.Repositories;

public class OrcamentoRepository : IOrcamentoRepository
{
    private readonly AppDbContext _context;

    public OrcamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Orcamento?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Orcamentos
            .Include(o => o.Cliente)
            .Include(o => o.Aparelho)
            .Include(o => o.Itens)
            .Include(o => o.OrdemServico)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Orcamento?> ObterPorNumeroAsync(string numero, CancellationToken cancellationToken = default)
    {
        return await _context.Orcamentos
            .Include(o => o.Cliente)
            .Include(o => o.Aparelho)
            .Include(o => o.Itens)
            .FirstOrDefaultAsync(o => o.Numero == numero, cancellationToken);
    }

    public async Task<IEnumerable<Orcamento>> ListarAsync(int limite = 100, CancellationToken cancellationToken = default)
    {
        return await _context.Orcamentos
            .AsNoTracking()
            .Include(o => o.Cliente)
            .Include(o => o.Aparelho)
            .OrderByDescending(o => o.DataCriacao)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Orcamento>> ListarPorStatusAsync(StatusOrcamento status, int limite = 100, CancellationToken cancellationToken = default)
    {
        return await _context.Orcamentos
            .AsNoTracking()
            .Include(o => o.Cliente)
            .Include(o => o.Aparelho)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.DataCriacao)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Orcamento>> PesquisarAsync(string termo, int limite = 100, CancellationToken cancellationToken = default)
    {
        var termoLower = termo.ToLower();

        return await _context.Orcamentos
            .AsNoTracking()
            .Include(o => o.Cliente)
            .Include(o => o.Aparelho)
            .Where(o =>
                o.Numero.Contains(termo) ||
                o.Cliente.Nome.ToLower().Contains(termoLower) ||
                o.Aparelho.Marca.ToLower().Contains(termoLower) ||
                o.Aparelho.Modelo.ToLower().Contains(termoLower))
            .OrderByDescending(o => o.DataCriacao)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> ContarPorStatusAsync(StatusOrcamento status, CancellationToken cancellationToken = default)
    {
        return await _context.Orcamentos
            .AsNoTracking()
            .CountAsync(o => o.Status == status, cancellationToken);
    }

    public async Task<string> GerarProximoNumeroAsync(CancellationToken cancellationToken = default)
    {
        var ultimoNumero = await _context.Orcamentos
            .AsNoTracking()
            .OrderByDescending(o => o.Id)
            .Select(o => o.Numero)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(ultimoNumero))
            return "ORC0001";

        if (int.TryParse(ultimoNumero.Replace("ORC", ""), out var num))
            return $"ORC{(num + 1):D4}";

        return "ORC0001";
    }

    public async Task AdicionarAsync(Orcamento orcamento, CancellationToken cancellationToken = default)
    {
        await _context.Orcamentos.AddAsync(orcamento, cancellationToken);
    }

    public async Task AtualizarAsync(Orcamento orcamento, CancellationToken cancellationToken = default)
    {
        _context.Orcamentos.Update(orcamento);
        await Task.CompletedTask;
    }

    public async Task SalvarAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
