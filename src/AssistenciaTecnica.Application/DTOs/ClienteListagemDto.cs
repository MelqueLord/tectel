namespace AssistenciaTecnica.Application.DTOs;

public class ClienteListagemDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CpfCnpj { get; set; }
    public string? Telefone { get; set; }
    public string? WhatsApp { get; set; }
    public string? Cidade { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
}
