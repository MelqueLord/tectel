using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.DTOs;

public class OrcamentoDto
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string ClienteTelefone { get; set; } = string.Empty;
    public int AparelhoId { get; set; }
    public string AparelhoDescricao { get; set; } = string.Empty;
    public string DefeitoRelatado { get; set; } = string.Empty;
    public string? Diagnostico { get; set; }
    public string? Observacoes { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataValidade { get; set; }
    public DateTime? DataEnvio { get; set; }
    public DateTime? DataAprovacao { get; set; }
    public StatusOrcamento Status { get; set; }
    public string StatusDescricao { get; set; } = string.Empty;
    public decimal ValorServicos { get; set; }
    public decimal ValorPecas { get; set; }
    public decimal Desconto { get; set; }
    public decimal ValorTotal { get; set; }
    public int? OrdemServicoId { get; set; }
    public string? OrdemServicoNumero { get; set; }
    public List<OrcamentoItemDto> Itens { get; set; } = [];
}

public class OrcamentoItemDto
{
    public int Id { get; set; }
    public TipoItemOrcamento Tipo { get; set; }
    public string TipoDescricao { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal Subtotal { get; set; }
}

public class OrcamentoListagemDto
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public string ClienteNome { get; set; } = string.Empty;
    public string AparelhoDescricao { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime? DataValidade { get; set; }
    public StatusOrcamento Status { get; set; }
    public string StatusDescricao { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public bool PodeConverter { get; set; }
}

public class CriarOrcamentoDto
{
    public int ClienteId { get; set; }
    public int AparelhoId { get; set; }
    public string DefeitoRelatado { get; set; } = string.Empty;
    public string? Diagnostico { get; set; }
    public string? Observacoes { get; set; }
    public DateTime? DataValidade { get; set; }
    public decimal Desconto { get; set; }
    public List<CriarOrcamentoItemDto> Itens { get; set; } = [];
}

public class CriarOrcamentoItemDto
{
    public TipoItemOrcamento Tipo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int Quantidade { get; set; } = 1;
    public decimal ValorUnitario { get; set; }
}

public class EditarOrcamentoDto
{
    public int Id { get; set; }
    public string? Diagnostico { get; set; }
    public string? Observacoes { get; set; }
    public DateTime? DataValidade { get; set; }
    public decimal Desconto { get; set; }
    public List<CriarOrcamentoItemDto> Itens { get; set; } = [];
}
