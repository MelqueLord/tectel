namespace AssistenciaTecnica.Domain.Entities;

public class OrdemServico
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public int AparelhoId { get; set; }
    public int? TecnicoId { get; set; }
    public string DefeitoRelatado { get; set; } = string.Empty;
    public string? Diagnostico { get; set; }
    public string? ServicoRealizado { get; set; }
    public DateTime DataEntrada { get; set; } = DateTime.UtcNow;
    public DateTime? PrevisaoEntrega { get; set; }
    public DateTime? DataConclusao { get; set; }
    public DateTime? DataEntrega { get; set; }
    public StatusOrdemServico Status { get; set; } = StatusOrdemServico.Aberta;
    public decimal ValorServico { get; set; }
    public decimal ValorPecas { get; set; }
    public decimal Desconto { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal ValorPago { get; set; }
    public string? Observacoes { get; set; }
    public int PrazoGarantiaDias { get; set; } = 90;

    public Cliente Cliente { get; set; } = null!;
    public Aparelho Aparelho { get; set; } = null!;
    public Tecnico? Tecnico { get; set; }
    public ICollection<Pagamento> Pagamentos { get; set; } = [];
    public ICollection<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } = [];

    public void CalcularValorTotal()
    {
        ValorTotal = ValorServico + ValorPecas - Desconto;
    }

    public decimal SaldoPendente => ValorTotal - ValorPago;
}

public enum StatusOrdemServico
{
    Aberta,
    EmAnalise,
    AguardandoAprovacao,
    EmManutencao,
    Concluida,
    Entregue,
    Cancelada
}
