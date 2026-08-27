using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.DTOs;

public class OrdemServicoDto
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public int AparelhoId { get; set; }
    public string AparelhoDescricao { get; set; } = string.Empty;
    public int? TecnicoId { get; set; }
    public string? TecnicoNome { get; set; }
    public string DefeitoRelatado { get; set; } = string.Empty;
    public string? Diagnostico { get; set; }
    public string? ServicoRealizado { get; set; }
    public DateTime DataEntrada { get; set; }
    public DateTime? PrevisaoEntrega { get; set; }
    public DateTime? DataConclusao { get; set; }
    public DateTime? DataEntrega { get; set; }
    public StatusOrdemServico Status { get; set; }
    public string StatusDescricao { get; set; } = string.Empty;
    public decimal ValorServico { get; set; }
    public decimal ValorPecas { get; set; }
    public decimal Desconto { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal ValorPago { get; set; }
    public decimal SaldoPendente { get; set; }
    public string? Observacoes { get; set; }
    public int PrazoGarantiaDias { get; set; }
    public List<PagamentoDto> Pagamentos { get; set; } = [];
}

public class OrdemServicoListagemDto
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string AparelhoDescricao { get; set; } = string.Empty;
    public string? TecnicoNome { get; set; }
    public string DefeitoRelatado { get; set; } = string.Empty;
    public DateTime DataEntrada { get; set; }
    public DateTime? PrevisaoEntrega { get; set; }
    public StatusOrdemServico Status { get; set; }
    public string StatusDescricao { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
}

public class CriarOrdemServicoDto
{
    public int ClienteId { get; set; }
    public int AparelhoId { get; set; }
    public int? TecnicoId { get; set; }
    public string DefeitoRelatado { get; set; } = string.Empty;
    public DateTime? PrevisaoEntrega { get; set; }
    public string? Observacoes { get; set; }
    public int PrazoGarantiaDias { get; set; } = 90;
}

public class EditarOrdemServicoDto
{
    public int Id { get; set; }
    public int? TecnicoId { get; set; }
    public string? Diagnostico { get; set; }
    public string? ServicoRealizado { get; set; }
    public DateTime? PrevisaoEntrega { get; set; }
    public decimal ValorServico { get; set; }
    public decimal ValorPecas { get; set; }
    public decimal Desconto { get; set; }
    public string? Observacoes { get; set; }
    public int PrazoGarantiaDias { get; set; } = 90;
}

public class DashboardDto
{
    public int OrdensAbertas { get; set; }
    public int EmManutencao { get; set; }
    public int AguardandoAprovacao { get; set; }
    public int AguardandoRetirada { get; set; }
    public int ConcluidasNoMes { get; set; }
    public int ProdutosEstoqueBaixo { get; set; }
    public List<OrdemServicoListagemDto> OrdensRecentes { get; set; } = [];
}
