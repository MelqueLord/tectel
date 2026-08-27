namespace AssistenciaTecnica.Application.DTOs;

public class ClienteFormDto
{
    public int? Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CpfCnpj { get; set; }
    public string? Telefone { get; set; }
    public string? WhatsApp { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Cep { get; set; }
    public string? Observacoes { get; set; }
    public bool Ativo { get; set; } = true;
    public bool IsEdicao => Id.HasValue;
}
