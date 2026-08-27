using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.DTOs;

public class OrdemServicoFormDto
{
    public int? Id { get; set; }
    public string? Numero { get; set; }
    public int ClienteId { get; set; }
    public string? ClienteNome { get; set; }
    public int AparelhoId { get; set; }
    public string? AparelhoDescricao { get; set; }
    public int? TecnicoId { get; set; }
    public string? TecnicoNome { get; set; }
    public string DefeitoRelatado { get; set; } = string.Empty;
    public string? Diagnostico { get; set; }
    public string? ServicoRealizado { get; set; }
    public DateTime? PrevisaoEntrega { get; set; }
    public decimal ValorServico { get; set; }
    public decimal ValorPecas { get; set; }
    public decimal Desconto { get; set; }
    public string? Observacoes { get; set; }
    public int PrazoGarantiaDias { get; set; } = 90;
    public DateTime DataEntrada { get; set; }
    public StatusOrdemServico Status { get; set; }
    public string? StatusDescricao { get; set; }
    public bool IsEdicao => Id.HasValue;
}
