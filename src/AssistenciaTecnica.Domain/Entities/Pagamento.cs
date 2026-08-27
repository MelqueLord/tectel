namespace AssistenciaTecnica.Domain.Entities;

public class Pagamento
{
    public int Id { get; set; }
    public int OrdemServicoId { get; set; }
    public DateTime DataPagamento { get; set; } = DateTime.UtcNow;
    public decimal Valor { get; set; }
    public FormaPagamento FormaPagamento { get; set; }
    public string? Observacao { get; set; }

    public OrdemServico OrdemServico { get; set; } = null!;
}

public enum FormaPagamento
{
    Dinheiro,
    Pix,
    CartaoCredito,
    CartaoDebito,
    Transferencia,
    Outro
}
