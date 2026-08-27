using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenciaTecnica.Web.Controllers;

[Authorize]
public class OrdensServicoController : Controller
{
    private readonly IOrdemServicoService _ordemService;
    private readonly IClienteService _clienteService;
    private readonly IAparelhoService _aparelhoService;
    private readonly IPagamentoService _pagamentoService;
    private readonly IConfiguracaoEmpresaService _configService;
    private readonly ITecnicoService _tecnicoService;

    public OrdensServicoController(
        IOrdemServicoService ordemService,
        IClienteService clienteService,
        IAparelhoService aparelhoService,
        IPagamentoService pagamentoService,
        IConfiguracaoEmpresaService configService,
        ITecnicoService tecnicoService)
    {
        _ordemService = ordemService;
        _clienteService = clienteService;
        _aparelhoService = aparelhoService;
        _pagamentoService = pagamentoService;
        _configService = configService;
        _tecnicoService = tecnicoService;
    }

    public async Task<IActionResult> Index(string? pesquisa, string? status, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Ordens de Serviço";
        ViewData["Pesquisa"] = pesquisa;
        ViewData["Status"] = status;

        IEnumerable<OrdemServicoListagemDto> ordens;

        if (!string.IsNullOrWhiteSpace(pesquisa))
        {
            ordens = await _ordemService.PesquisarAsync(pesquisa, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<StatusOrdemServico>(status, out var statusEnum))
        {
            ordens = await _ordemService.ListarPorStatusAsync(statusEnum, cancellationToken);
        }
        else
        {
            ordens = await _ordemService.ListarAsync(cancellationToken);
        }

        return View(ordens);
    }

    public async Task<IActionResult> Criar(int? clienteId, int? aparelhoId, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Nova Ordem de Serviço";
        await CarregarDadosFormularioAsync(clienteId, aparelhoId, cancellationToken);

        return View("Form", new OrdemServicoFormDto
        {
            ClienteId = clienteId ?? 0,
            AparelhoId = aparelhoId ?? 0
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Criar(OrdemServicoFormDto form, CancellationToken cancellationToken)
    {
        try
        {
            var dto = new CriarOrdemServicoDto
            {
                ClienteId = form.ClienteId,
                AparelhoId = form.AparelhoId,
                TecnicoId = form.TecnicoId,
                DefeitoRelatado = form.DefeitoRelatado,
                PrevisaoEntrega = form.PrevisaoEntrega,
                Observacoes = form.Observacoes,
                PrazoGarantiaDias = form.PrazoGarantiaDias
            };

            await _ordemService.CriarAsync(dto, cancellationToken);
            TempData["Sucesso"] = "Ordem de serviço criada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ViewData["Title"] = "Nova Ordem de Serviço";
        await CarregarDadosFormularioAsync(null, null, cancellationToken);
        return View("Form", form);
    }

    public async Task<IActionResult> Editar(int id, CancellationToken cancellationToken)
    {
        var ordem = await _ordemService.ObterPorIdAsync(id, cancellationToken);
        if (ordem is null)
            return NotFound();

        ViewData["Title"] = "Editar Ordem de Serviço";

        var form = new OrdemServicoFormDto
        {
            Id = ordem.Id,
            Numero = ordem.Numero,
            ClienteId = ordem.ClienteId,
            ClienteNome = ordem.ClienteNome,
            AparelhoId = ordem.AparelhoId,
            AparelhoDescricao = ordem.AparelhoDescricao,
            TecnicoId = ordem.TecnicoId,
            TecnicoNome = ordem.TecnicoNome,
            DefeitoRelatado = ordem.DefeitoRelatado,
            Diagnostico = ordem.Diagnostico,
            ServicoRealizado = ordem.ServicoRealizado,
            PrevisaoEntrega = ordem.PrevisaoEntrega,
            ValorServico = ordem.ValorServico,
            ValorPecas = ordem.ValorPecas,
            Desconto = ordem.Desconto,
            Observacoes = ordem.Observacoes,
            PrazoGarantiaDias = ordem.PrazoGarantiaDias,
            DataEntrada = ordem.DataEntrada,
            Status = ordem.Status,
            StatusDescricao = ordem.StatusDescricao
        };

        return View("Form", form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(OrdemServicoFormDto form, CancellationToken cancellationToken)
    {
        try
        {
            var dto = new EditarOrdemServicoDto
            {
                Id = form.Id ?? 0,
                TecnicoId = form.TecnicoId,
                Diagnostico = form.Diagnostico,
                ServicoRealizado = form.ServicoRealizado,
                PrevisaoEntrega = form.PrevisaoEntrega,
                ValorServico = form.ValorServico,
                ValorPecas = form.ValorPecas,
                Desconto = form.Desconto,
                Observacoes = form.Observacoes,
                PrazoGarantiaDias = form.PrazoGarantiaDias
            };

            await _ordemService.AtualizarAsync(dto, cancellationToken);
            TempData["Sucesso"] = "Ordem de serviço atualizada com sucesso!";
            return RedirectToAction(nameof(Detalhes), new { id = form.Id });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        ViewData["Title"] = "Editar Ordem de Serviço";
        return View("Form", form);
    }

    public async Task<IActionResult> Detalhes(int id, CancellationToken cancellationToken)
    {
        var ordem = await _ordemService.ObterPorIdAsync(id, cancellationToken);
        if (ordem is null)
            return NotFound();

        var pagamentos = await _pagamentoService.ListarPorOrdemAsync(id, cancellationToken);
        ViewData["Pagamentos"] = pagamentos;
        ViewData["Title"] = $"Ordem {ordem.Numero}";
        return View(ordem);
    }

    public async Task<IActionResult> Imprimir(int id, CancellationToken cancellationToken)
    {
        var ordem = await _ordemService.ObterPorIdAsync(id, cancellationToken);
        if (ordem is null)
            return NotFound();

        var pagamentos = await _pagamentoService.ListarPorOrdemAsync(id, cancellationToken);
        var config = await _configService.ObterAsync(cancellationToken);

        ViewData["Pagamentos"] = pagamentos;
        ViewData["Configuracao"] = config;
        ViewData["Title"] = $"Imprimir OS {ordem.Numero}";
        return View(ordem);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarPagamento(int ordemId, decimal valor, FormaPagamento formaPagamento, string? observacao, CancellationToken cancellationToken)
    {
        try
        {
            var dto = new RegistrarPagamentoDto
            {
                OrdemServicoId = ordemId,
                Valor = valor,
                FormaPagamento = formaPagamento,
                Observacao = observacao
            };

            await _pagamentoService.RegistrarAsync(dto, cancellationToken);
            TempData["Sucesso"] = "Pagamento registrado com sucesso!";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Detalhes), new { id = ordemId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarStatus(int id, StatusOrdemServico novoStatus, CancellationToken cancellationToken)
    {
        try
        {
            await _ordemService.AlterarStatusAsync(id, novoStatus, cancellationToken);
            TempData["Sucesso"] = "Status alterado com sucesso!";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Detalhes), new { id });
    }

    private async Task CarregarDadosFormularioAsync(int? clienteId, int? aparelhoId, CancellationToken cancellationToken)
    {
        var clientes = await _clienteService.ListarAtivosAsync(cancellationToken);
        ViewData["Clientes"] = clientes;

        var tecnicos = await _tecnicoService.ListarAtivosAsync(cancellationToken);
        ViewData["Tecnicos"] = tecnicos;

        if (clienteId.HasValue)
        {
            var aparelhos = await _aparelhoService.ListarPorClienteAsync(clienteId.Value, cancellationToken);
            ViewData["Aparelhos"] = aparelhos;
        }
        else
        {
            ViewData["Aparelhos"] = Enumerable.Empty<AparelhoDto>();
        }
    }
}
