namespace AssistenciaTecnica.Application.DTOs;

public class RelatorioFaturamentoDto
{
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public decimal FaturamentoTotal { get; set; }
    public decimal TicketMedio { get; set; }
    public int TotalOrdens { get; set; }
    public List<FaturamentoPeriodoDto> FaturamentoPorPeriodo { get; set; } = [];
}

public class FaturamentoPeriodoDto
{
    public string Periodo { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int Quantidade { get; set; }
}

public class RelatorioOrdensStatusDto
{
    public int TotalOrdens { get; set; }
    public List<OrdensStatusDto> OrdensPorStatus { get; set; } = [];
}

public class OrdensStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public string Cor { get; set; } = string.Empty;
}

public class RelatorioPagamentosDto
{
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public decimal TotalPago { get; set; }
    public List<PagamentoFormaDto> PagamentosPorForma { get; set; } = [];
}

public class PagamentoFormaDto
{
    public string FormaPagamento { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int Quantidade { get; set; }
    public string Cor { get; set; } = string.Empty;
}

public class RelatorioTopClientesDto
{
    public List<TopClienteDto> TopClientes { get; set; } = [];
}

public class TopClienteDto
{
    public int ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int TotalOrdens { get; set; }
    public decimal TotalGasto { get; set; }
}

public class RelatorioDashboardDto
{
    public RelatorioFaturamentoDto Faturamento { get; set; } = new();
    public RelatorioOrdensStatusDto OrdensPorStatus { get; set; } = new();
    public RelatorioPagamentosDto Pagamentos { get; set; } = new();
    public RelatorioTopClientesDto TopClientes { get; set; } = new();
}
