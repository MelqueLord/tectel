using AssistenciaTecnica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenciaTecnica.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IOrdemServicoService _ordemService;
    private readonly IProdutoService _produtoService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IOrdemServicoService ordemService,
        IProdutoService produtoService,
        ILogger<DashboardController> logger)
    {
        _ordemService = ordemService;
        _produtoService = produtoService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var dashboard = await _ordemService.ObterDashboardAsync(cancellationToken);

        try
        {
            var produtosEstoqueBaixo = await _produtoService.ListarEstoqueBaixoAsync(cancellationToken);
            dashboard.ProdutosEstoqueBaixo = produtosEstoqueBaixo.Count();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao carregar produtos com estoque baixo no dashboard");
            dashboard.ProdutosEstoqueBaixo = 0;
        }

        return View(dashboard);
    }
}
