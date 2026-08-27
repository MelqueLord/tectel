using AssistenciaTecnica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenciaTecnica.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IOrdemServicoService _ordemService;
    private readonly IProdutoService _produtoService;

    public DashboardController(IOrdemServicoService ordemService, IProdutoService produtoService)
    {
        _ordemService = ordemService;
        _produtoService = produtoService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var dashboard = await _ordemService.ObterDashboardAsync(cancellationToken);

        try
        {
            var produtosEstoqueBaixo = await _produtoService.ListarEstoqueBaixoAsync(cancellationToken);
            dashboard.ProdutosEstoqueBaixo = produtosEstoqueBaixo.Count();
        }
        catch
        {
            dashboard.ProdutosEstoqueBaixo = 0;
        }

        return View(dashboard);
    }
}
