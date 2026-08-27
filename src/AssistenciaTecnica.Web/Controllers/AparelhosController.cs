using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenciaTecnica.Web.Controllers;

[Authorize]
public class AparelhosController : Controller
{
    private readonly IAparelhoService _aparelhoService;
    private readonly IClienteService _clienteService;
    private readonly IOrdemServicoService _ordemService;

    public AparelhosController(
        IAparelhoService aparelhoService,
        IClienteService clienteService,
        IOrdemServicoService ordemService)
    {
        _aparelhoService = aparelhoService;
        _clienteService = clienteService;
        _ordemService = ordemService;
    }

    public async Task<IActionResult> Index(string? pesquisa, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Aparelhos";
        ViewData["Pesquisa"] = pesquisa;

        var aparelhos = string.IsNullOrWhiteSpace(pesquisa)
            ? await _aparelhoService.ListarAsync(cancellationToken)
            : await _aparelhoService.PesquisarAsync(pesquisa, cancellationToken);

        return View(aparelhos);
    }

    public async Task<IActionResult> Criar(int? clienteId, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Novo Aparelho";
        await CarregarClientesAsync(clienteId, cancellationToken);

        return View("Form", new AparelhoFormDto { ClienteId = clienteId ?? 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(AparelhoFormDto form, CancellationToken cancellationToken)
    {
        try
        {
            var dto = new CriarAparelhoDto
            {
                ClienteId = form.ClienteId,
                Tipo = form.Tipo,
                Marca = form.Marca,
                Modelo = form.Modelo,
                Cor = form.Cor,
                Imei = form.Imei,
                NumeroSerie = form.NumeroSerie,
                SenhaDesbloqueio = form.SenhaDesbloqueio,
                EstadoAparelho = form.EstadoAparelho,
                ItensEntregues = form.ItensEntregues,
                Observacoes = form.Observacoes
            };

            await _aparelhoService.CriarAsync(dto, cancellationToken);
            TempData["Sucesso"] = "Aparelho cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ViewData["Title"] = "Novo Aparelho";
        await CarregarClientesAsync(null, cancellationToken);
        return View("Form", form);
    }

    public async Task<IActionResult> Editar(int id, CancellationToken cancellationToken)
    {
        var aparelho = await _aparelhoService.ObterPorIdAsync(id, cancellationToken);
        if (aparelho is null)
            return NotFound();

        ViewData["Title"] = "Editar Aparelho";
        await CarregarClientesAsync(null, cancellationToken);

        var form = new AparelhoFormDto
        {
            Id = aparelho.Id,
            ClienteId = aparelho.ClienteId,
            Tipo = aparelho.Tipo,
            Marca = aparelho.Marca,
            Modelo = aparelho.Modelo,
            Cor = aparelho.Cor,
            Imei = aparelho.Imei,
            NumeroSerie = aparelho.NumeroSerie,
            EstadoAparelho = aparelho.EstadoAparelho,
            ItensEntregues = aparelho.ItensEntregues,
            Observacoes = aparelho.Observacoes
        };

        return View("Form", form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(AparelhoFormDto form, CancellationToken cancellationToken)
    {
        try
        {
            var dto = new EditarAparelhoDto
            {
                Id = form.Id ?? 0,
                ClienteId = form.ClienteId,
                Tipo = form.Tipo,
                Marca = form.Marca,
                Modelo = form.Modelo,
                Cor = form.Cor,
                Imei = form.Imei,
                NumeroSerie = form.NumeroSerie,
                SenhaDesbloqueio = form.SenhaDesbloqueio,
                EstadoAparelho = form.EstadoAparelho,
                ItensEntregues = form.ItensEntregues,
                Observacoes = form.Observacoes
            };

            await _aparelhoService.AtualizarAsync(dto, cancellationToken);
            TempData["Sucesso"] = "Aparelho atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ViewData["Title"] = "Editar Aparelho";
        await CarregarClientesAsync(null, cancellationToken);
        return View("Form", form);
    }

    public async Task<IActionResult> Detalhes(int id, CancellationToken cancellationToken)
    {
        var aparelho = await _aparelhoService.ObterPorIdAsync(id, cancellationToken);
        if (aparelho is null)
            return NotFound();

        var ordens = await _ordemService.PesquisarAsync(aparelho.Imei ?? "", cancellationToken);

        ViewData["Title"] = "Detalhes do Aparelho";
        ViewData["Ordens"] = ordens.Where(o => o.AparelhoDescricao.Contains(aparelho.Marca) &&
                                                o.AparelhoDescricao.Contains(aparelho.Modelo)).ToList();

        return View(aparelho);
    }

    [HttpGet]
    public async Task<IActionResult> ObterPorCliente(int clienteId, CancellationToken cancellationToken)
    {
        var aparelhos = await _aparelhoService.ListarPorClienteAsync(clienteId, cancellationToken);
        return Json(aparelhos.Select(a => new { a.Id, Descricao = $"{a.Marca} {a.Modelo}" }));
    }

    private async Task CarregarClientesAsync(int? clienteSelecionado, CancellationToken cancellationToken)
    {
        var clientes = await _clienteService.ListarAtivosAsync(cancellationToken);
        ViewData["Clientes"] = clientes;
        ViewData["ClienteSelecionado"] = clienteSelecionado;
    }
}
