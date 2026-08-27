namespace AssistenciaTecnica.Domain.Entities;

public class MovimentacaoEstoque
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public TipoMovimentacao Tipo { get; set; }
    public int Quantidade { get; set; }
    public DateTime DataMovimentacao { get; set; } = DateTime.UtcNow;
    public string? Motivo { get; set; }
    public int? OrdemServicoId { get; set; }
    public string? UsuarioResponsavel { get; set; }

    public Produto Produto { get; set; } = null!;
    public OrdemServico? OrdemServico { get; set; }
}

public enum TipoMovimentacao
{
    Entrada,
    Saida,
    Ajuste
}
