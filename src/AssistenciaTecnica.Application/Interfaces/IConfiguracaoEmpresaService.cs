using AssistenciaTecnica.Application.DTOs;

namespace AssistenciaTecnica.Application.Interfaces;

public interface IConfiguracaoEmpresaService
{
    Task<ConfiguracaoEmpresaDto?> ObterAsync(CancellationToken cancellationToken = default);
    Task SalvarAsync(ConfiguracaoEmpresaDto dto, CancellationToken cancellationToken = default);
}
