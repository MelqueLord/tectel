using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;
using AssistenciaTecnica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Infrastructure.Repositories;

public class TecnicoRepository : ITecnicoRepository
{
    private readonly AppDbContext _context;

    public TecnicoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Tecnico?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Tecnicos
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Tecnico>> ListarAtivosAsync(int limite = 100, CancellationToken cancellationToken = default)
    {
        return await _context.Tecnicos
            .AsNoTracking()
            .Where(t => t.Ativo)
            .OrderBy(t => t.Nome)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tecnico>> ListarTodosAsync(int limite = 100, CancellationToken cancellationToken = default)
    {
        return await _context.Tecnicos
            .AsNoTracking()
            .OrderBy(t => t.Nome)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tecnico>> PesquisarAsync(string termo, int limite = 100, CancellationToken cancellationToken = default)
    {
        var termoLower = termo.ToLower();

        return await _context.Tecnicos
            .AsNoTracking()
            .Where(t =>
                t.Nome.ToLower().Contains(termoLower) ||
                (t.Especialidade != null && t.Especialidade.ToLower().Contains(termoLower)) ||
                (t.Telefone != null && t.Telefone.Contains(termo)) ||
                (t.WhatsApp != null && t.WhatsApp.Contains(termo)))
            .OrderBy(t => t.Nome)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task AdicionarAsync(Tecnico tecnico, CancellationToken cancellationToken = default)
    {
        await _context.Tecnicos.AddAsync(tecnico, cancellationToken);
    }

    public async Task AtualizarAsync(Tecnico tecnico, CancellationToken cancellationToken = default)
    {
        _context.Tecnicos.Update(tecnico);
        await Task.CompletedTask;
    }

    public async Task SalvarAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
