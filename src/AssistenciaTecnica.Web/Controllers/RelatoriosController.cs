using AssistenciaTecnica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenciaTecnica.Web.Controllers;

[Authorize]
public class RelatoriosController : Controller
{
    private readonly IRelatorioService _relatorioService;

    public RelatoriosController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var relatorio = await _relatorioService.ObterRelatorioDashboardAsync(cancellationToken);
        return View(relatorio);
    }

    public async Task<IActionResult> Faturamento(DateTime? dataInicio, DateTime? dataFim, CancellationToken cancellationToken)
    {
        var inicio = dataInicio ?? DateTime.UtcNow.AddMonths(-11).Date;
        var fim = dataFim ?? DateTime.UtcNow.Date;

        var relatorio = await _relatorioService.ObterFaturamentoAsync(inicio, fim, cancellationToken);
        return View(relatorio);
    }

    public async Task<IActionResult> OrdensPorStatus(CancellationToken cancellationToken)
    {
        var relatorio = await _relatorioService.ObterOrdensPorStatusAsync(cancellationToken);
        return View(relatorio);
    }

    public async Task<IActionResult> Pagamentos(DateTime? dataInicio, DateTime? dataFim, CancellationToken cancellationToken)
    {
        var inicio = dataInicio ?? DateTime.UtcNow.AddMonths(-11).Date;
        var fim = dataFim ?? DateTime.UtcNow.Date;

        var relatorio = await _relatorioService.ObterPagamentosPorFormaAsync(inicio, fim, cancellationToken);
        return View(relatorio);
    }

    public async Task<IActionResult> TopClientes(CancellationToken cancellationToken)
    {
        var relatorio = await _relatorioService.ObterTopClientesAsync(10, cancellationToken);
        return View(relatorio);
    }
}
