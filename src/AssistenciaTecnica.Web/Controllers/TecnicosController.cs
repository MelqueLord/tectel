using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenciaTecnica.Web.Controllers;

[Authorize]
public class TecnicosController : Controller
{
    private readonly ITecnicoService _tecnicoService;

    public TecnicosController(ITecnicoService tecnicoService)
    {
        _tecnicoService = tecnicoService;
    }

    public async Task<IActionResult> Index(string? pesquisa, string? situacao, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Técnicos";
        ViewData["Pesquisa"] = pesquisa;
        ViewData["Situacao"] = situacao;

        IEnumerable<TecnicoListagemDto> tecnicos;

        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            tecnicos = await _tecnicoService.PesquisarAsync(pesquisa, cancellationToken);
        }
        else if (situacao == "inativos")
        {
            tecnicos = await _tecnicoService.ListarTodosAsync(cancellationToken);
        }
        else
        {
            tecnicos = await _tecnicoService.ListarAtivosAsync(cancellationToken);
        }

        return View(tecnicos);
    }

    public IActionResult Criar()
    {
        ViewData["Title"] = "Novo Técnico";
        return View("Form", new TecnicoFormDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(TecnicoFormDto form, CancellationToken cancellationToken)
    {
        try
        {
            await _tecnicoService.CriarAsync(form, cancellationToken);
            TempData["Sucesso"] = "Técnico cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ViewData["Title"] = "Novo Técnico";
        return View("Form", form);
    }

    public async Task<IActionResult> Editar(int id, CancellationToken cancellationToken)
    {
        var tecnico = await _tecnicoService.ObterPorIdAsync(id, cancellationToken);
        if (tecnico is null)
            return NotFound();

        ViewData["Title"] = "Editar Técnico";

        var form = new TecnicoFormDto
        {
            Id = tecnico.Id,
            Nome = tecnico.Nome,
            Telefone = tecnico.Telefone,
            WhatsApp = tecnico.WhatsApp,
            Email = tecnico.Email,
            Especialidade = tecnico.Especialidade,
            Ativo = tecnico.Ativo
        };

        return View("Form", form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(TecnicoFormDto form, CancellationToken cancellationToken)
    {
        try
        {
            await _tecnicoService.AtualizarAsync(form, cancellationToken);
            TempData["Sucesso"] = "Técnico atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ViewData["Title"] = "Editar Técnico";
        return View("Form", form);
    }

    public async Task<IActionResult> Detalhes(int id, CancellationToken cancellationToken)
    {
        var tecnico = await _tecnicoService.ObterPorIdAsync(id, cancellationToken);
        if (tecnico is null)
            return NotFound();

        ViewData["Title"] = tecnico.Nome;
        return View(tecnico);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inativar(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _tecnicoService.InativarAsync(id, cancellationToken);
            TempData["Sucesso"] = "Técnico inativado com sucesso!";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reativar(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _tecnicoService.ReativarAsync(id, cancellationToken);
            TempData["Sucesso"] = "Técnico reativado com sucesso!";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
