using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AssistenciaTecnica.Web.ViewComponents;

public class NotificacoesViewComponent : ViewComponent
{
    private readonly INotificacaoService _notificacaoService;

    public NotificacoesViewComponent(INotificacaoService notificacaoService)
    {
        _notificacaoService = notificacaoService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var resumo = await _notificacaoService.ObterNotificacoesAsync(CancellationToken.None);
            ViewData["Notificacoes"] = resumo;
            return View();
        }
        catch
        {
            ViewData["Notificacoes"] = new NotificacaoResumoDto();
            return View();
        }
    }
}
