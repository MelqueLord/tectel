using AssistenciaTecnica.Application.DTOs;

namespace AssistenciaTecnica.Application.Interfaces;

public interface INotificacaoService
{
    Task<NotificacaoResumoDto> ObterNotificacoesAsync(CancellationToken cancellationToken = default);
}
