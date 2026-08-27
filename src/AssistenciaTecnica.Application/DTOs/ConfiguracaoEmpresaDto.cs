namespace AssistenciaTecnica.Application.DTOs;

public class ConfiguracaoEmpresaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CpfCnpj { get; set; }
    public string? Telefone { get; set; }
    public string? WhatsApp { get; set; }
    public string? Endereco { get; set; }
    public string? LogoCaminho { get; set; }
    public string? TextoPadraoOrdem { get; set; }
    public int PrazoPadraoGarantiaDias { get; set; } = 90;
}
