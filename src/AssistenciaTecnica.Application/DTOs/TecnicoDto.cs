namespace AssistenciaTecnica.Application.DTOs;

public class TecnicoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? WhatsApp { get; set; }
    public string? Email { get; set; }
    public string? Especialidade { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}

public class TecnicoListagemDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? WhatsApp { get; set; }
    public string? Especialidade { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
}

public class TecnicoFormDto
{
    public int? Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? WhatsApp { get; set; }
    public string? Email { get; set; }
    public string? Especialidade { get; set; }
    public bool Ativo { get; set; } = true;
    public bool IsEdicao => Id.HasValue;
}
