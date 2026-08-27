namespace AssistenciaTecnica.Domain.Entities;

public class Orcamento
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public int AparelhoId { get; set; }
    public string DefeitoRelatado { get; set; } = string.Empty;
    public string? Diagnostico { get; set; }
    public string? Observacoes { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataValidade { get; set; }
    public DateTime? DataEnvio { get; set; }
    public DateTime? DataAprovacao { get; set; }
    public StatusOrcamento Status { get; set; } = StatusOrcamento.Rascunho;
    public decimal ValorServicos { get; set; }
    public decimal ValorPecas { get; set; }
    public decimal Desconto { get; set; }
    public decimal ValorTotal { get; set; }
    public int? OrdemServicoId { get; set; }

    public Cliente Cliente { get; set; } = null!;
    public Aparelho Aparelho { get; set; } = null!;
    public OrdemServico? OrdemServico { get; set; }
    public ICollection<OrcamentoItem> Itens { get; set; } = [];

    public void CalcularValorTotal()
    {
        ValorServicos = Itens.Where(i => i.Tipo == TipoItemOrcamento.Servico).Sum(i => i.Subtotal);
        ValorPecas = Itens.Where(i => i.Tipo == TipoItemOrcamento.Peca).Sum(i => i.Subtotal);
        ValorTotal = ValorServicos + ValorPecas - Desconto;
    }
}

public class OrcamentoItem
{
    public int Id { get; set; }
    public int OrcamentoId { get; set; }
    public TipoItemOrcamento Tipo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int Quantidade { get; set; } = 1;
    public decimal ValorUnitario { get; set; }
    public decimal Subtotal => Quantidade * ValorUnitario;

    public Orcamento Orcamento { get; set; } = null!;
}

public enum StatusOrcamento
{
    Rascunho,
    Enviado,
    Aprovado,
    Rejeitado,
    Convertido,
    Expirado
}

public enum TipoItemOrcamento
{
    Servico,
    Peca
}
