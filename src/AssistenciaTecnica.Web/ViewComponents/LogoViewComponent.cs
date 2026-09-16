using AssistenciaTecnica.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AssistenciaTecnica.Web.ViewComponents;

public class LogoViewComponent : ViewComponent
{
    private readonly IConfiguracaoEmpresaService _configService;
    private readonly ILogger<LogoViewComponent> _logger;

    public LogoViewComponent(
        IConfiguracaoEmpresaService configService,
        ILogger<LogoViewComponent> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var config = await _configService.ObterAsync(CancellationToken.None);
            ViewData["LogoCaminho"] = config?.LogoCaminho;
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao carregar logo da empresa");
            ViewData["LogoCaminho"] = null;
            return View();
        }
    }
}
