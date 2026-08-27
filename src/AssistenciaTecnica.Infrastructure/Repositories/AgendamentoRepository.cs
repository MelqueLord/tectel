using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;
using AssistenciaTecnica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Infrastructure.Repositories;

public class AgendamentoRepository : IAgendamentoRepository
{
    private readonly AppDbContext _context;

    public AgendamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Agendamento?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.OrdemServico)
            .ThenInclude(o => o.Cliente)
            .Include(a => a.Cliente)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.OrdemServico)
            .ThenInclude(o => o.Cliente)
            .Include(a => a.Cliente)
            .OrderBy(a => a.DataInicio)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ListarPorPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.OrdemServico)
            .ThenInclude(o => o.Cliente)
            .Include(a => a.Cliente)
            .Where(a => a.DataInicio >= inicio && a.DataInicio <= fim)
            .OrderBy(a => a.DataInicio)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ListarPorOrdemAsync(int ordemServicoId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.OrdemServico)
            .ThenInclude(o => o.Cliente)
            .Include(a => a.Cliente)
            .Where(a => a.OrdemServicoId == ordemServicoId)
            .OrderBy(a => a.DataInicio)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ListarPorClienteAsync(int clienteId, CancellationToken cancellationToken = default)
    {
        return await _context.Agendamentos
            .Include(a => a.OrdemServico)
            .ThenInclude(o => o.Cliente)
            .Include(a => a.Cliente)
            .Where(a => a.ClienteId == clienteId)
            .OrderBy(a => a.DataInicio)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Agendamento>> ListarHojeAsync(CancellationToken cancellationToken = default)
    {
        var hoje = DateTime.Today;
        var amanha = hoje.AddDays(1);

        return await _context.Agendamentos
            .Include(a => a.OrdemServico)
            .ThenInclude(o => o.Cliente)
            .Include(a => a.Cliente)
            .Where(a => a.DataInicio >= hoje && a.DataInicio < amanha)
            .OrderBy(a => a.DataInicio)
            .ToListAsync(cancellationToken);
    }

    public async Task AdicionarAsync(Agendamento agendamento, CancellationToken cancellationToken = default)
    {
        await _context.Agendamentos.AddAsync(agendamento, cancellationToken);
    }

    public async Task AtualizarAsync(Agendamento agendamento, CancellationToken cancellationToken = default)
    {
        _context.Agendamentos.Update(agendamento);
        await Task.CompletedTask;
    }

    public async Task SalvarAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
