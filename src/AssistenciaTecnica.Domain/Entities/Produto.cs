namespace AssistenciaTecnica.Domain.Entities;

public class Produto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? Categoria { get; set; }
    public string? Marca { get; set; }
    public string? ModeloCompativel { get; set; }
    public decimal PrecoCusto { get; set; }
    public decimal PrecoVenda { get; set; }
    public int QuantidadeEstoque { get; set; }
    public int EstoqueMinimo { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = [];

    public bool EstoqueBaixo => QuantidadeEstoque <= EstoqueMinimo;
}
