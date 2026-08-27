using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenciaTecnica.Web.Controllers;

[Authorize]
public class ProdutosController : Controller
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    public async Task<IActionResult> Index(string? pesquisa, string? filtro, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Produtos";
        ViewData["Pesquisa"] = pesquisa;
        ViewData["Filtro"] = filtro ?? "ativos";

        IEnumerable<ProdutoListagemDto> produtos;

        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            produtos = await _produtoService.PesquisarAsync(pesquisa, cancellationToken);
        }
        else if (filtro == "estoque-baixo")
        {
            produtos = await _produtoService.ListarEstoqueBaixoAsync(cancellationToken);
        }
        else
        {
            produtos = await _produtoService.ListarAtivosAsync(cancellationToken);
        }

        return View(produtos);
    }

    public IActionResult Criar()
    {
        ViewData["Title"] = "Novo Produto";
        return View("Form", new ProdutoFormDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(ProdutoFormDto form, CancellationToken cancellationToken)
    {
        try
        {
            var dto = new CriarProdutoDto
            {
                Codigo = form.Codigo,
                Nome = form.Nome,
                Descricao = form.Descricao,
                Categoria = form.Categoria,
                Marca = form.Marca,
                ModeloCompativel = form.ModeloCompativel,
                PrecoCusto = form.PrecoCusto,
                PrecoVenda = form.PrecoVenda,
                QuantidadeEstoque = form.QuantidadeEstoque,
                EstoqueMinimo = form.EstoqueMinimo
            };

            await _produtoService.CriarAsync(dto, cancellationToken);
            TempData["Sucesso"] = "Produto cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ViewData["Title"] = "Novo Produto";
        return View("Form", form);
    }

    public async Task<IActionResult> Editar(int id, CancellationToken cancellationToken)
    {
        var produto = await _produtoService.ObterPorIdAsync(id, cancellationToken);
        if (produto is null)
            return NotFound();

        ViewData["Title"] = "Editar Produto";

        var form = new ProdutoFormDto
        {
            Id = produto.Id,
            Codigo = produto.Codigo,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Categoria = produto.Categoria,
            Marca = produto.Marca,
            ModeloCompativel = produto.ModeloCompativel,
            PrecoCusto = produto.PrecoCusto,
            PrecoVenda = produto.PrecoVenda,
            QuantidadeEstoque = produto.QuantidadeEstoque,
            EstoqueMinimo = produto.EstoqueMinimo,
            Ativo = produto.Ativo
        };

        return View("Form", form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(ProdutoFormDto form, CancellationToken cancellationToken)
    {
        try
        {
            var dto = new EditarProdutoDto
            {
                Id = form.Id ?? 0,
                Codigo = form.Codigo,
                Nome = form.Nome,
                Descricao = form.Descricao,
                Categoria = form.Categoria,
                Marca = form.Marca,
                ModeloCompativel = form.ModeloCompativel,
                PrecoCusto = form.PrecoCusto,
                PrecoVenda = form.PrecoVenda,
                EstoqueMinimo = form.EstoqueMinimo,
                Ativo = form.Ativo
            };

            await _produtoService.AtualizarAsync(dto, cancellationToken);
            TempData["Sucesso"] = "Produto atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ViewData["Title"] = "Editar Produto";
        return View("Form", form);
    }

    public async Task<IActionResult> Detalhes(int id, CancellationToken cancellationToken)
    {
        var produto = await _produtoService.ObterPorIdAsync(id, cancellationToken);
        if (produto is null)
            return NotFound();

        ViewData["Title"] = "Detalhes do Produto";
        return View(produto);
    }
}
