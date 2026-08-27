namespace AssistenciaTecnica.Application.DTOs;

public class AparelhoFormDto
{
    public int? Id { get; set; }
    public int ClienteId { get; set; }
    public string Tipo { get; set; } = "Celular";
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string? Cor { get; set; }
    public string? Imei { get; set; }
    public string? NumeroSerie { get; set; }
    public string? SenhaDesbloqueio { get; set; }
    public string? EstadoAparelho { get; set; }
    public string? ItensEntregues { get; set; }
    public string? Observacoes { get; set; }
    public bool IsEdicao => Id.HasValue;
}
