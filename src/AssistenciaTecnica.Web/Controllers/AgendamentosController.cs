using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenciaTecnica.Web.Controllers;

[Authorize]
public class AgendamentosController : Controller
{
    private readonly IAgendamentoService _agendamentoService;
    private readonly IClienteService _clienteService;
    private readonly IOrdemServicoService _ordemService;

    public AgendamentosController(
        IAgendamentoService agendamentoService,
        IClienteService clienteService,
        IOrdemServicoService ordemService)
    {
        _agendamentoService = agendamentoService;
        _clienteService = clienteService;
        _ordemService = ordemService;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Agendamentos";
        return View();
    }

    public async Task<IActionResult> Calendario(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Calendário";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Eventos(DateTime start, DateTime end, CancellationToken cancellationToken)
    {
        var eventos = await _agendamentoService.ListarCalendarioAsync(start, end, cancellationToken);
        return Json(eventos);
    }

    public async Task<IActionResult> Hoje(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Agendamentos de Hoje";
        var agendamentos = await _agendamentoService.ListarHojeAsync(cancellationToken);
        return View("Lista", agendamentos);
    }

    public async Task<IActionResult> Detalhes(int id, CancellationToken cancellationToken)
    {
        var agendamento = await _agendamentoService.ObterPorIdAsync(id, cancellationToken);
        if (agendamento is null)
            return NotFound();

        ViewData["Title"] = "Detalhes do Agendamento";
        return View(agendamento);
    }

    public async Task<IActionResult> Criar(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Novo Agendamento";
        ViewData["Clientes"] = await _clienteService.ListarAtivosAsync(cancellationToken);
        return View("Form", new AgendamentoFormDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(AgendamentoFormDto form, CancellationToken cancellationToken)
    {
        try
        {
            var dataInicio = form.DataInicio.Date + TimeSpan.Parse(form.HoraInicio);
            var dataFim = form.DataFim.Date + TimeSpan.Parse(form.HoraFim);

            var dto = new CriarAgendamentoDto
            {
                OrdemServicoId = form.OrdemServicoId,
                ClienteId = form.ClienteId,
                Titulo = form.Titulo,
                Descricao = form.Descricao,
                DataInicio = dataInicio,
                DataFim = dataFim,
                Tipo = form.Tipo
            };

            await _agendamentoService.CriarAsync(dto, cancellationToken);
            TempData["Sucesso"] = "Agendamento criado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ViewData["Title"] = "Novo Agendamento";
        ViewData["Clientes"] = await _clienteService.ListarAtivosAsync(cancellationToken);
        return View("Form", form);
    }

    public async Task<IActionResult> Editar(int id, CancellationToken cancellationToken)
    {
        var agendamento = await _agendamentoService.ObterPorIdAsync(id, cancellationToken);
        if (agendamento is null)
            return NotFound();

        var form = new AgendamentoFormDto
        {
            Id = agendamento.Id,
            OrdemServicoId = agendamento.OrdemServicoId,
            ClienteId = agendamento.ClienteId,
            Titulo = agendamento.Titulo,
            Descricao = agendamento.Descricao,
            DataInicio = agendamento.DataInicio.Date,
            HoraInicio = agendamento.DataInicio.ToString("HH:mm"),
            DataFim = agendamento.DataFim.Date,
            HoraFim = agendamento.DataFim.ToString("HH:mm"),
            Tipo = agendamento.Tipo,
            Status = agendamento.Status
        };

        ViewData["Title"] = "Editar Agendamento";
        ViewData["Clientes"] = await _clienteService.ListarAtivosAsync(cancellationToken);
        return View("Form", form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(AgendamentoFormDto form, CancellationToken cancellationToken)
    {
        try
        {
            form.DataInicio = form.DataInicio.Date + TimeSpan.Parse(form.HoraInicio);
            form.DataFim = form.DataFim.Date + TimeSpan.Parse(form.HoraFim);

            await _agendamentoService.AtualizarAsync(form, cancellationToken);
            TempData["Sucesso"] = "Agendamento atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ViewData["Title"] = "Editar Agendamento";
        ViewData["Clientes"] = await _clienteService.ListarAtivosAsync(cancellationToken);
        return View("Form", form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarStatus(int id, StatusAgendamento novoStatus, CancellationToken cancellationToken)
    {
        try
        {
            await _agendamentoService.AlterarStatusAsync(id, novoStatus, cancellationToken);
            TempData["Sucesso"] = "Status atualizado com sucesso!";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Detalhes), new { id });
    }
}
