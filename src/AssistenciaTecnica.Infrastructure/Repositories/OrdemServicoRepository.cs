using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;
using AssistenciaTecnica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Infrastructure.Repositories;

public class OrdemServicoRepository : IOrdemServicoRepository
{
    private readonly AppDbContext _context;

    public OrdemServicoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrdemServico?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.OrdensServico
            .Include(o => o.Cliente)
            .Include(o => o.Aparelho)
            .Include(o => o.Tecnico)
            .Include(o => o.Pagamentos)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<OrdemServico?> ObterPorNumeroAsync(string numero, CancellationToken cancellationToken = default)
    {
        return await _context.OrdensServico
            .Include(o => o.Cliente)
            .Include(o => o.Aparelho)
            .Include(o => o.Tecnico)
            .FirstOrDefaultAsync(o => o.Numero == numero, cancellationToken);
    }

    public async Task<IEnumerable<OrdemServico>> ListarAsync(int limite = 100, CancellationToken cancellationToken = default)
    {
        return await _context.OrdensServico
            .AsNoTracking()
            .Include(o => o.Cliente)
            .Include(o => o.Aparelho)
            .Include(o => o.Tecnico)
            .OrderByDescending(o => o.DataEntrada)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OrdemServico>> ListarPorStatusAsync(StatusOrdemServico status, int limite = 100, CancellationToken cancellationToken = default)
    {
        return await _context.OrdensServico
            .AsNoTracking()
            .Include(o => o.Cliente)
            .Include(o => o.Aparelho)
            .Include(o => o.Tecnico)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.DataEntrada)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OrdemServico>> PesquisarAsync(string termo, int limite = 100, CancellationToken cancellationToken = default)
    {
        var termoLower = termo.ToLower();

        return await _context.OrdensServico
            .AsNoTracking()
            .Include(o => o.Cliente)
            .Include(o => o.Aparelho)
            .Include(o => o.Tecnico)
            .Where(o =>
                o.Numero.Contains(termo) ||
                o.Cliente.Nome.ToLower().Contains(termoLower) ||
                o.Aparelho.Marca.ToLower().Contains(termoLower) ||
                o.Aparelho.Modelo.ToLower().Contains(termoLower) ||
                (o.Aparelho.Imei != null && o.Aparelho.Imei.Contains(termo)) ||
                (o.Tecnico != null && o.Tecnico.Nome.ToLower().Contains(termoLower)))
            .OrderByDescending(o => o.DataEntrada)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> ContarPorStatusAsync(StatusOrdemServico status, CancellationToken cancellationToken = default)
    {
        return await _context.OrdensServico
            .AsNoTracking()
            .CountAsync(o => o.Status == status, cancellationToken);
    }

    public async Task<string> GerarProximoNumeroAsync(CancellationToken cancellationToken = default)
    {
        var ultimoNumero = await _context.OrdensServico
            .AsNoTracking()
            .OrderByDescending(o => o.Id)
            .Select(o => o.Numero)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(ultimoNumero))
            return "OS0001";

        if (int.TryParse(ultimoNumero.Replace("OS", ""), out var num))
            return $"OS{(num + 1):D4}";

        return "OS0001";
    }

    public async Task AdicionarAsync(OrdemServico ordem, CancellationToken cancellationToken = default)
    {
        await _context.OrdensServico.AddAsync(ordem, cancellationToken);
    }

    public async Task AtualizarAsync(OrdemServico ordem, CancellationToken cancellationToken = default)
    {
        _context.OrdensServico.Update(ordem);
        await Task.CompletedTask;
    }

    public async Task SalvarAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
