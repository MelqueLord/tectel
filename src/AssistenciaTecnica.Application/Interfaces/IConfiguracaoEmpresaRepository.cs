using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Interfaces;

public interface IConfiguracaoEmpresaRepository
{
    Task<ConfiguracaoEmpresa?> ObterAsync(CancellationToken cancellationToken = default);
    Task SalvarAsync(ConfiguracaoEmpresa configuracao, CancellationToken cancellationToken = default);
}
