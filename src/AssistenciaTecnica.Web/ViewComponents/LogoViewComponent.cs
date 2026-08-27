using AssistenciaTecnica.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AssistenciaTecnica.Web.ViewComponents;

public class LogoViewComponent : ViewComponent
{
    private readonly IConfiguracaoEmpresaService _configService;

    public LogoViewComponent(IConfiguracaoEmpresaService configService)
    {
        _configService = configService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var config = await _configService.ObterAsync(CancellationToken.None);
            ViewData["LogoCaminho"] = config?.LogoCaminho;
            return View();
        }
        catch
        {
            ViewData["LogoCaminho"] = null;
            return View();
        }
    }
}
