namespace AssistenciaTecnica.Application.DTOs;

public class ProdutoDto
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
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
}

public class ProdutoListagemDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public decimal PrecoVenda { get; set; }
    public int QuantidadeEstoque { get; set; }
    public int EstoqueMinimo { get; set; }
    public bool EstoqueBaixo { get; set; }
}

public class CriarProdutoDto
{
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
}

public class EditarProdutoDto
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
    public int EstoqueMinimo { get; set; }
    public bool Ativo { get; set; }
}

public class MovimentacaoEstoqueDto
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public DateTime DataMovimentacao { get; set; }
    public string? Motivo { get; set; }
    public int? OrdemServicoId { get; set; }
}
