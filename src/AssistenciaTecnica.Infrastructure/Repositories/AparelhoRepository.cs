using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;
using AssistenciaTecnica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Infrastructure.Repositories;

public class AparelhoRepository : IAparelhoRepository
{
    private readonly AppDbContext _context;

    public AparelhoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Aparelho?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Aparelhos
            .Include(a => a.Cliente)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Aparelho>> ListarAsync(int limite = 100, CancellationToken cancellationToken = default)
    {
        return await _context.Aparelhos
            .AsNoTracking()
            .Include(a => a.Cliente)
            .OrderBy(a => a.Marca)
            .ThenBy(a => a.Modelo)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Aparelho>> ListarPorClienteAsync(int clienteId, CancellationToken cancellationToken = default)
    {
        return await _context.Aparelhos
            .AsNoTracking()
            .Include(a => a.Cliente)
            .Where(a => a.ClienteId == clienteId)
            .OrderBy(a => a.Marca)
            .ThenBy(a => a.Modelo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Aparelho>> PesquisarAsync(string termo, int limite = 100, CancellationToken cancellationToken = default)
    {
        var termoLower = termo.ToLower();

        return await _context.Aparelhos
            .AsNoTracking()
            .Include(a => a.Cliente)
            .Where(a =>
                a.Marca.ToLower().Contains(termoLower) ||
                a.Modelo.ToLower().Contains(termoLower) ||
                (a.Imei != null && a.Imei.Contains(termo)) ||
                (a.NumeroSerie != null && a.NumeroSerie.Contains(termo)) ||
                a.Cliente.Nome.ToLower().Contains(termoLower))
            .OrderBy(a => a.Marca)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task AdicionarAsync(Aparelho aparelho, CancellationToken cancellationToken = default)
    {
        await _context.Aparelhos.AddAsync(aparelho, cancellationToken);
    }

    public async Task AtualizarAsync(Aparelho aparelho, CancellationToken cancellationToken = default)
    {
        _context.Aparelhos.Update(aparelho);
        await Task.CompletedTask;
    }

    public async Task SalvarAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
