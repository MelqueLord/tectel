using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.DTOs;

public class PagamentoDto
{
    public int Id { get; set; }
    public int OrdemServicoId { get; set; }
    public DateTime DataPagamento { get; set; }
    public decimal Valor { get; set; }
    public FormaPagamento FormaPagamento { get; set; }
    public string FormaPagamentoDescricao { get; set; } = string.Empty;
    public string? Observacao { get; set; }
}

public class RegistrarPagamentoDto
{
    public int OrdemServicoId { get; set; }
    public decimal Valor { get; set; }
    public FormaPagamento FormaPagamento { get; set; }
    public string? Observacao { get; set; }
}
