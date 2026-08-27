using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;
using AssistenciaTecnica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Cliente?> ObterPorCpfCnpjAsync(string cpfCnpj, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .FirstOrDefaultAsync(c => c.CpfCnpj == cpfCnpj, cancellationToken);
    }

    public async Task<bool> ExisteCpfCnpjAsync(string cpfCnpj, int? excluirId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Clientes.Where(c => c.CpfCnpj == cpfCnpj);

        if (excluirId.HasValue)
            query = query.Where(c => c.Id != excluirId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<Cliente>> ListarAtivosAsync(int limite = 100, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .AsNoTracking()
            .Where(c => c.Ativo)
            .OrderBy(c => c.Nome)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Cliente>> ListarInativosAsync(int limite = 100, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .AsNoTracking()
            .Where(c => !c.Ativo)
            .OrderBy(c => c.Nome)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Cliente>> ListarTodosAsync(int limite = 100, CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Cliente>> PesquisarAsync(string termo, int limite = 100, CancellationToken cancellationToken = default)
    {
        var termoLower = termo.ToLower();

        return await _context.Clientes
            .AsNoTracking()
            .Where(c =>
                c.Nome.ToLower().Contains(termoLower) ||
                (c.CpfCnpj != null && c.CpfCnpj.Contains(termo)) ||
                (c.Telefone != null && c.Telefone.Contains(termo)) ||
                (c.WhatsApp != null && c.WhatsApp.Contains(termo)))
            .OrderBy(c => c.Nome)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        await _context.Clientes.AddAsync(cliente, cancellationToken);
    }

    public async Task AtualizarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        _context.Clientes.Update(cliente);
        await Task.CompletedTask;
    }

    public async Task SalvarAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
