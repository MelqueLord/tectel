namespace AssistenciaTecnica.Domain.Entities;

public class Aparelho
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string? Cor { get; set; }
    public string? Imei { get; set; }
    public string? NumeroSerie { get; set; }
    public string? SenhaDesbloqueio { get; set; }
    public string? EstadoAparelho { get; set; }
    public string? ItensEntregues { get; set; }
    public string? Observacoes { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public Cliente Cliente { get; set; } = null!;
}
