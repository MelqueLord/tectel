using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;
using AssistenciaTecnica.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Infrastructure.Repositories;

public class ConfiguracaoEmpresaRepository : IConfiguracaoEmpresaRepository
{
    private readonly AppDbContext _context;

    public ConfiguracaoEmpresaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ConfiguracaoEmpresa?> ObterAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ConfiguracaoEmpresa
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SalvarAsync(ConfiguracaoEmpresa configuracao, CancellationToken cancellationToken = default)
    {
        if (configuracao.Id == 0)
            await _context.ConfiguracaoEmpresa.AddAsync(configuracao, cancellationToken);
        else
            _context.ConfiguracaoEmpresa.Update(configuracao);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
