using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AssistenciaTecnica.Web.ViewComponents;

public class NotificacoesViewComponent : ViewComponent
{
    private readonly INotificacaoService _notificacaoService;
    private readonly ILogger<NotificacoesViewComponent> _logger;

    public NotificacoesViewComponent(
        INotificacaoService notificacaoService,
        ILogger<NotificacoesViewComponent> logger)
    {
        _notificacaoService = notificacaoService;
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var resumo = await _notificacaoService.ObterNotificacoesAsync(CancellationToken.None);
            ViewData["Notificacoes"] = resumo;
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao carregar notificações");
            ViewData["Notificacoes"] = new NotificacaoResumoDto();
            return View();
        }
    }
}
