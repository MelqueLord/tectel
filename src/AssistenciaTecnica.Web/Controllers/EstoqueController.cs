using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenciaTecnica.Web.Controllers;

[Authorize]
public class EstoqueController : Controller
{
    private readonly IProdutoService _produtoService;
    private readonly IMovimentacaoEstoqueRepository _movimentacaoRepository;

    public EstoqueController(IProdutoService produtoService, IMovimentacaoEstoqueRepository movimentacaoRepository)
    {
        _produtoService = produtoService;
        _movimentacaoRepository = movimentacaoRepository;
    }

    public async Task<IActionResult> Index(string? pesquisa, string? filtro, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Estoque";
        ViewData["Pesquisa"] = pesquisa;
        ViewData["Filtro"] = filtro ?? "todos";

        IEnumerable<ProdutoListagemDto> produtos;

        if (filtro == "estoque-baixo")
        {
            produtos = await _produtoService.ListarEstoqueBaixoAsync(cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            produtos = await _produtoService.PesquisarAsync(pesquisa, cancellationToken);
        }
        else
        {
            produtos = await _produtoService.ListarAtivosAsync(cancellationToken);
        }

        return View(produtos);
    }

    public async Task<IActionResult> Movimentacao(int produtoId, CancellationToken cancellationToken)
    {
        var produto = await _produtoService.ObterPorIdAsync(produtoId, cancellationToken);
        if (produto is null)
            return NotFound();

        ViewData["Title"] = "Movimentação de Estoque";
        ViewData["Movimentacoes"] = await _movimentacaoRepository.ListarPorProdutoAsync(produtoId, cancellationToken);

        return View(new MovimentacaoEstoqueFormDto
        {
            ProdutoId = produtoId,
            ProdutoNome = produto.Nome
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarEntrada(MovimentacaoEstoqueFormDto form, CancellationToken cancellationToken)
    {
        try
        {
            await _produtoService.RegistrarEntradaAsync(form.ProdutoId, form.Quantidade, form.Motivo, cancellationToken);
            TempData["Sucesso"] = "Entrada registrada com sucesso!";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Movimentacao), new { produtoId = form.ProdutoId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarSaida(MovimentacaoEstoqueFormDto form, CancellationToken cancellationToken)
    {
        try
        {
            await _produtoService.RegistrarSaidaAsync(form.ProdutoId, form.Quantidade, form.Motivo, null, cancellationToken);
            TempData["Sucesso"] = "Saída registrada com sucesso!";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Movimentacao), new { produtoId = form.ProdutoId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarAjuste(MovimentacaoEstoqueFormDto form, CancellationToken cancellationToken)
    {
        try
        {
            await _produtoService.RegistrarAjusteAsync(form.ProdutoId, form.Quantidade, form.Motivo, cancellationToken);
            TempData["Sucesso"] = "Ajuste registrado com sucesso!";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Movimentacao), new { produtoId = form.ProdutoId });
    }
}
