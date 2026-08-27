using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AssistenciaTecnica.Web.Controllers;

[Authorize]
public class OrcamentosController : Controller
{
    private readonly IOrcamentoService _orcamentoService;
    private readonly IClienteService _clienteService;
    private readonly IAparelhoService _aparelhoService;

    public OrcamentosController(
        IOrcamentoService orcamentoService,
        IClienteService clienteService,
        IAparelhoService aparelhoService)
    {
        _orcamentoService = orcamentoService;
        _clienteService = clienteService;
        _aparelhoService = aparelhoService;
    }

    public async Task<IActionResult> Index(string? pesquisa, StatusOrcamento? status, CancellationToken cancellationToken)
    {
        IEnumerable<OrcamentoListagemDto> orcamentos;

        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            orcamentos = await _orcamentoService.PesquisarAsync(pesquisa, cancellationToken);
        }
        else if (status.HasValue)
        {
            orcamentos = await _orcamentoService.ListarPorStatusAsync(status.Value, cancellationToken);
        }
        else
        {
            orcamentos = await _orcamentoService.ListarAsync(cancellationToken);
        }

        ViewBag.Pesquisa = pesquisa;
        ViewBag.StatusFiltro = status;
        return View(orcamentos);
    }

    public async Task<IActionResult> Criar(CancellationToken cancellationToken)
    {
        await CarregarViewBagsAsync(cancellationToken);
        return View(new CriarOrcamentoDto
        {
            DataValidade = DateTime.UtcNow.AddDays(15),
            Itens = [new CriarOrcamentoItemDto { Quantidade = 1 }]
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(CriarOrcamentoDto dto, CancellationToken cancellationToken)
    {
        try
        {
            dto.Itens = dto.Itens.Where(i => !string.IsNullOrWhiteSpace(i.Descricao)).ToList();
            await _orcamentoService.CriarAsync(dto, cancellationToken);
            TempData["Sucesso"] = "Orçamento criado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["Erro"] = ex.Message;
            await CarregarViewBagsAsync(cancellationToken);
            return View(dto);
        }
    }

    public async Task<IActionResult> Detalhes(int id, CancellationToken cancellationToken)
    {
        var orcamento = await _orcamentoService.ObterPorIdAsync(id, cancellationToken);
        if (orcamento is null) return NotFound();
        return View(orcamento);
    }

    public async Task<IActionResult> Editar(int id, CancellationToken cancellationToken)
    {
        var orcamento = await _orcamentoService.ObterPorIdAsync(id, cancellationToken);
        if (orcamento is null) return NotFound();

        if (orcamento.Status != StatusOrcamento.Rascunho)
        {
            TempData["Erro"] = "Somente orçamentos em rascunho podem ser editados.";
            return RedirectToAction(nameof(Detalhes), new { id });
        }

        var dto = new EditarOrcamentoDto
        {
            Id = orcamento.Id,
            Diagnostico = orcamento.Diagnostico,
            Observacoes = orcamento.Observacoes,
            DataValidade = orcamento.DataValidade,
            Desconto = orcamento.Desconto,
            Itens = orcamento.Itens.Select(i => new CriarOrcamentoItemDto
            {
                Tipo = i.Tipo,
                Descricao = i.Descricao,
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario
            }).ToList()
        };

        await CarregarViewBagsAsync(cancellationToken);
        ViewBag.ClienteNome = orcamento.ClienteNome;
        ViewBag.AparelhoDescricao = orcamento.AparelhoDescricao;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(EditarOrcamentoDto dto, CancellationToken cancellationToken)
    {
        try
        {
            dto.Itens = dto.Itens.Where(i => !string.IsNullOrWhiteSpace(i.Descricao)).ToList();
            await _orcamentoService.AtualizarAsync(dto, cancellationToken);
            TempData["Sucesso"] = "Orçamento atualizado com sucesso!";
            return RedirectToAction(nameof(Detalhes), new { id = dto.Id });
        }
        catch (InvalidOperationException ex)
        {
            TempData["Erro"] = ex.Message;
            await CarregarViewBagsAsync(cancellationToken);
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarStatus(int id, StatusOrcamento novoStatus, CancellationToken cancellationToken)
    {
        try
        {
            await _orcamentoService.AlterarStatusAsync(id, novoStatus, cancellationToken);
            TempData["Sucesso"] = "Status alterado com sucesso!";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Detalhes), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConverterEmOS(int id, CancellationToken cancellationToken)
    {
        try
        {
            var ordem = await _orcamentoService.ConverterEmOrdemServicoAsync(id, cancellationToken);
            TempData["Sucesso"] = $"Ordem de serviço {ordem.Numero} criada com sucesso!";
            return RedirectToAction("Detalhes", "OrdensServico", new { id = ordem.Id });
        }
        catch (InvalidOperationException ex)
        {
            TempData["Erro"] = ex.Message;
            return RedirectToAction(nameof(Detalhes), new { id });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ObterAparelhos(int clienteId, CancellationToken cancellationToken)
    {
        var aparelhos = await _aparelhoService.ListarPorClienteAsync(clienteId, cancellationToken);
        return Json(aparelhos.Select(a => new { value = a.Id, text = $"{a.Marca} {a.Modelo}" }));
    }

    private async Task CarregarViewBagsAsync(CancellationToken cancellationToken)
    {
        var clientes = await _clienteService.ListarAtivosAsync(cancellationToken);
        ViewBag.Clientes = clientes.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Nome
        }).ToList();
    }
}
