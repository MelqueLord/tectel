using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Services;

public class RelatorioService : IRelatorioService
{
    private readonly IOrdemServicoRepository _ordemRepository;
    private readonly IPagamentoRepository _pagamentoRepository;
    private readonly IClienteRepository _clienteRepository;

    public RelatorioService(
        IOrdemServicoRepository ordemRepository,
        IPagamentoRepository pagamentoRepository,
        IClienteRepository clienteRepository)
    {
        _ordemRepository = ordemRepository;
        _pagamentoRepository = pagamentoRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<RelatorioFaturamentoDto> ObterFaturamentoAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default)
    {
        var ordens = await _ordemRepository.ListarAsync(1000, cancellationToken);
        var ordensPeriodo = ordens.Where(o => o.DataEntrada >= dataInicio && o.DataEntrada <= dataFim).ToList();

        var faturamentoTotal = ordensPeriodo.Sum(o => o.ValorTotal);
        var ticketMedio = ordensPeriodo.Count > 0 ? faturamentoTotal / ordensPeriodo.Count : 0;

        var faturamentoPorPeriodo = new List<FaturamentoPeriodoDto>();
        var periodoAtual = dataInicio;

        while (periodoAtual <= dataFim)
        {
            var periodoFim = periodoAtual.AddMonths(1).AddDays(-1);
            if (periodoFim > dataFim) periodoFim = dataFim;

            var ordensPeriodoItem = ordensPeriodo
                .Where(o => o.DataEntrada >= periodoAtual && o.DataEntrada <= periodoFim)
                .ToList();

            faturamentoPorPeriodo.Add(new FaturamentoPeriodoDto
            {
                Periodo = periodoAtual.ToString("MMM/yyyy"),
                Valor = ordensPeriodoItem.Sum(o => o.ValorTotal),
                Quantidade = ordensPeriodoItem.Count
            });

            periodoAtual = periodoAtual.AddMonths(1);
        }

        return new RelatorioFaturamentoDto
        {
            DataInicio = dataInicio,
            DataFim = dataFim,
            FaturamentoTotal = faturamentoTotal,
            TicketMedio = ticketMedio,
            TotalOrdens = ordensPeriodo.Count,
            FaturamentoPorPeriodo = faturamentoPorPeriodo
        };
    }

    public async Task<RelatorioOrdensStatusDto> ObterOrdensPorStatusAsync(CancellationToken cancellationToken = default)
    {
        var ordens = await _ordemRepository.ListarAsync(1000, cancellationToken);
        var totalOrdens = ordens.Count();

        var ordensPorStatus = new List<OrdensStatusDto>();
        var statusList = Enum.GetValues<StatusOrdemServico>();

        foreach (var status in statusList)
        {
            var quantidade = ordens.Count(o => o.Status == status);
            if (quantidade > 0)
            {
                ordensPorStatus.Add(new OrdensStatusDto
                {
                    Status = ObterDescricaoStatus(status),
                    Quantidade = quantidade,
                    Cor = ObterCorStatus(status)
                });
            }
        }

        return new RelatorioOrdensStatusDto
        {
            TotalOrdens = totalOrdens,
            OrdensPorStatus = ordensPorStatus
        };
    }

    public async Task<RelatorioPagamentosDto> ObterPagamentosPorFormaAsync(DateTime dataInicio, DateTime dataFim, CancellationToken cancellationToken = default)
    {
        var ordens = await _ordemRepository.ListarAsync(1000, cancellationToken);
        var ordensPeriodo = ordens.Where(o => o.DataEntrada >= dataInicio && o.DataEntrada <= dataFim).ToList();

        var todosPagamentos = new List<Pagamento>();
        foreach (var ordem in ordensPeriodo)
        {
            var pagamentos = await _pagamentoRepository.ListarPorOrdemAsync(ordem.Id, cancellationToken);
            todosPagamentos.AddRange(pagamentos);
        }

        var pagamentosPeriodo = todosPagamentos
            .Where(p => p.DataPagamento >= dataInicio && p.DataPagamento <= dataFim)
            .ToList();

        var totalPago = pagamentosPeriodo.Sum(p => p.Valor);

        var pagamentosPorForma = new List<PagamentoFormaDto>();
        var formasPagamento = Enum.GetValues<FormaPagamento>();

        foreach (var forma in formasPagamento)
        {
            var pagamentosForma = pagamentosPeriodo.Where(p => p.FormaPagamento == forma).ToList();
            if (pagamentosForma.Count > 0)
            {
                pagamentosPorForma.Add(new PagamentoFormaDto
                {
                    FormaPagamento = ObterDescricaoFormaPagamento(forma),
                    Valor = pagamentosForma.Sum(p => p.Valor),
                    Quantidade = pagamentosForma.Count,
                    Cor = ObterCorFormaPagamento(forma)
                });
            }
        }

        return new RelatorioPagamentosDto
        {
            DataInicio = dataInicio,
            DataFim = dataFim,
            TotalPago = totalPago,
            PagamentosPorForma = pagamentosPorForma
        };
    }

    public async Task<RelatorioTopClientesDto> ObterTopClientesAsync(int quantidade = 10, CancellationToken cancellationToken = default)
    {
        var ordens = await _ordemRepository.ListarAsync(1000, cancellationToken);
        var clientes = await _clienteRepository.ListarTodosAsync(1000, cancellationToken);

        var topClientes = ordens
            .GroupBy(o => o.ClienteId)
            .Select(g => new TopClienteDto
            {
                ClienteId = g.Key,
                Nome = clientes.FirstOrDefault(c => c.Id == g.Key)?.Nome ?? "Cliente não encontrado",
                TotalOrdens = g.Count(),
                TotalGasto = g.Sum(o => o.ValorTotal)
            })
            .OrderByDescending(c => c.TotalGasto)
            .Take(quantidade)
            .ToList();

        return new RelatorioTopClientesDto
        {
            TopClientes = topClientes
        };
    }

    public async Task<RelatorioDashboardDto> ObterRelatorioDashboardAsync(CancellationToken cancellationToken = default)
    {
        var dataInicio = DateTime.UtcNow.AddMonths(-11).Date;
        var dataFim = DateTime.UtcNow.Date;

        var faturamento = await ObterFaturamentoAsync(dataInicio, dataFim, cancellationToken);
        var ordensPorStatus = await ObterOrdensPorStatusAsync(cancellationToken);
        var pagamentos = await ObterPagamentosPorFormaAsync(dataInicio, dataFim, cancellationToken);
        var topClientes = await ObterTopClientesAsync(5, cancellationToken);

        return new RelatorioDashboardDto
        {
            Faturamento = faturamento,
            OrdensPorStatus = ordensPorStatus,
            Pagamentos = pagamentos,
            TopClientes = topClientes
        };
    }

    private static string ObterDescricaoStatus(StatusOrdemServico status)
    {
        return status switch
        {
            StatusOrdemServico.Aberta => "Aberta",
            StatusOrdemServico.EmAnalise => "Em Análise",
            StatusOrdemServico.AguardandoAprovacao => "Aguardando Aprovação",
            StatusOrdemServico.EmManutencao => "Em Manutenção",
            StatusOrdemServico.Concluida => "Concluída",
            StatusOrdemServico.Entregue => "Entregue",
            StatusOrdemServico.Cancelada => "Cancelada",
            _ => status.ToString()
        };
    }

    private static string ObterCorStatus(StatusOrdemServico status)
    {
        return status switch
        {
            StatusOrdemServico.Aberta => "#0d6efd",
            StatusOrdemServico.EmAnalise => "#0dcaf0",
            StatusOrdemServico.AguardandoAprovacao => "#ffc107",
            StatusOrdemServico.EmManutencao => "#fd7e14",
            StatusOrdemServico.Concluida => "#198754",
            StatusOrdemServico.Entregue => "#6c757d",
            StatusOrdemServico.Cancelada => "#dc3545",
            _ => "#6c757d"
        };
    }

    private static string ObterDescricaoFormaPagamento(FormaPagamento forma)
    {
        return forma switch
        {
            FormaPagamento.Dinheiro => "Dinheiro",
            FormaPagamento.Pix => "PIX",
            FormaPagamento.CartaoCredito => "Cartão de Crédito",
            FormaPagamento.CartaoDebito => "Cartão de Débito",
            FormaPagamento.Transferencia => "Transferência",
            FormaPagamento.Outro => "Outro",
            _ => forma.ToString()
        };
    }

    private static string ObterCorFormaPagamento(FormaPagamento forma)
    {
        return forma switch
        {
            FormaPagamento.Dinheiro => "#198754",
            FormaPagamento.Pix => "#0dcaf0",
            FormaPagamento.CartaoCredito => "#6f42c1",
            FormaPagamento.CartaoDebito => "#d63384",
            FormaPagamento.Transferencia => "#fd7e14",
            FormaPagamento.Outro => "#6c757d",
            _ => "#6c757d"
        };
    }
}
