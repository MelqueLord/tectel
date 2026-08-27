namespace AssistenciaTecnica.Application.DTOs;

public class ProdutoFormDto
{
    public int? Id { get; set; }
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
    public bool IsEdicao => Id.HasValue;
}

public class MovimentacaoEstoqueFormDto
{
    public int ProdutoId { get; set; }
    public string? ProdutoNome { get; set; }
    public string Tipo { get; set; } = "Entrada";
    public int Quantidade { get; set; }
    public string? Motivo { get; set; }
}
