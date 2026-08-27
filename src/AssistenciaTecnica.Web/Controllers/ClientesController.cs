using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenciaTecnica.Web.Controllers;

[Authorize]
public class ClientesController : Controller
{
    private readonly IClienteService _clienteService;
    private readonly IAparelhoService _aparelhoService;
    private readonly IOrdemServicoService _ordemService;

    public ClientesController(
        IClienteService clienteService,
        IAparelhoService aparelhoService,
        IOrdemServicoService ordemService)
    {
        _clienteService = clienteService;
        _aparelhoService = aparelhoService;
        _ordemService = ordemService;
    }

    public async Task<IActionResult> Index(string? pesquisa, string? situacao, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Clientes";
        ViewData["Pesquisa"] = pesquisa;
        ViewData["Situacao"] = situacao ?? "ativos";

        IEnumerable<ClienteListagemDto> clientes;

        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            clientes = await _clienteService.PesquisarAsync(pesquisa, cancellationToken);
        }
        else
        {
            clientes = (situacao?.ToLower()) switch
            {
                "inativos" => await _clienteService.ListarInativosAsync(cancellationToken),
                "todos" => await _clienteService.ListarTodosAsync(cancellationToken),
                _ => await _clienteService.ListarAtivosAsync(cancellationToken)
            };
        }

        return View(clientes);
    }

    public IActionResult Criar()
    {
        ViewData["Title"] = "Novo Cliente";
        return View("Form", new ClienteFormDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(ClienteFormDto form, CancellationToken cancellationToken)
    {
        try
        {
            var dto = new CriarClienteDto
            {
                Nome = form.Nome,
                CpfCnpj = form.CpfCnpj,
                Telefone = form.Telefone,
                WhatsApp = form.WhatsApp,
                Email = form.Email,
                Endereco = form.Endereco,
                Bairro = form.Bairro,
                Cidade = form.Cidade,
                Estado = form.Estado,
                Cep = form.Cep,
                Observacoes = form.Observacoes
            };

            await _clienteService.CriarAsync(dto, cancellationToken);
            TempData["Sucesso"] = "Cliente cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ViewData["Title"] = "Novo Cliente";
        return View("Form", form);
    }

    public async Task<IActionResult> Editar(int id, CancellationToken cancellationToken)
    {
        var cliente = await _clienteService.ObterPorIdAsync(id, cancellationToken);
        if (cliente is null)
            return NotFound();

        ViewData["Title"] = "Editar Cliente";

        var form = new ClienteFormDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            CpfCnpj = cliente.CpfCnpj,
            Telefone = cliente.Telefone,
            WhatsApp = cliente.WhatsApp,
            Email = cliente.Email,
            Endereco = cliente.Endereco,
            Bairro = cliente.Bairro,
            Cidade = cliente.Cidade,
            Estado = cliente.Estado,
            Cep = cliente.Cep,
            Observacoes = cliente.Observacoes,
            Ativo = cliente.Ativo
        };

        return View("Form", form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(ClienteFormDto form, CancellationToken cancellationToken)
    {
        try
        {
            var dto = new EditarClienteDto
            {
                Id = form.Id ?? 0,
                Nome = form.Nome,
                CpfCnpj = form.CpfCnpj,
                Telefone = form.Telefone,
                WhatsApp = form.WhatsApp,
                Email = form.Email,
                Endereco = form.Endereco,
                Bairro = form.Bairro,
                Cidade = form.Cidade,
                Estado = form.Estado,
                Cep = form.Cep,
                Observacoes = form.Observacoes,
                Ativo = form.Ativo
            };

            await _clienteService.AtualizarAsync(dto, cancellationToken);
            TempData["Sucesso"] = "Cliente atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ViewData["Title"] = "Editar Cliente";
        return View("Form", form);
    }

    public async Task<IActionResult> Detalhes(int id, CancellationToken cancellationToken)
    {
        var cliente = await _clienteService.ObterPorIdAsync(id, cancellationToken);
        if (cliente is null)
            return NotFound();

        ViewData["Title"] = "Detalhes do Cliente";
        return View(cliente);
    }

    public async Task<IActionResult> Historico(int id, CancellationToken cancellationToken)
    {
        var cliente = await _clienteService.ObterPorIdAsync(id, cancellationToken);
        if (cliente is null)
            return NotFound();

        var aparelhos = await _aparelhoService.ListarPorClienteAsync(id, cancellationToken);
        var todasOrdens = await _ordemService.ListarAsync(cancellationToken);
        var ordensCliente = todasOrdens.Where(o => o.ClienteId == id).ToList();

        var historico = new HistoricoClienteDto
        {
            Cliente = cliente,
            Aparelhos = aparelhos.ToList(),
            OrdensServico = ordensCliente,
            TotalGasto = ordensCliente.Sum(o => o.ValorTotal),
            TotalOrdens = ordensCliente.Count,
            OrdensAbertas = ordensCliente.Count(o => o.Status == Domain.Entities.StatusOrdemServico.Aberta ||
                                                      o.Status == Domain.Entities.StatusOrdemServico.EmAnalise ||
                                                      o.Status == Domain.Entities.StatusOrdemServico.AguardandoAprovacao ||
                                                      o.Status == Domain.Entities.StatusOrdemServico.EmManutencao),
            OrdensConcluidas = ordensCliente.Count(o => o.Status == Domain.Entities.StatusOrdemServico.Concluida ||
                                                        o.Status == Domain.Entities.StatusOrdemServico.Entregue)
        };

        ViewData["Title"] = $"Histórico - {cliente.Nome}";
        return View(historico);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Inativar(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _clienteService.InativarAsync(id, cancellationToken);
            TempData["Sucesso"] = "Cliente inativado com sucesso!";
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
            await _clienteService.ReativarAsync(id, cancellationToken);
            TempData["Sucesso"] = "Cliente reativado com sucesso!";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
