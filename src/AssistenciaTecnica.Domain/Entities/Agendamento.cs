namespace AssistenciaTecnica.Domain.Entities;

public class Agendamento
{
    public int Id { get; set; }
    public int? OrdemServicoId { get; set; }
    public int? ClienteId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public TipoAgendamento Tipo { get; set; }
    public StatusAgendamento Status { get; set; } = StatusAgendamento.Agendado;
    public string? Cor { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public OrdemServico? OrdemServico { get; set; }
    public Cliente? Cliente { get; set; }
}

public enum TipoAgendamento
{
    Entrega,
    Revisao,
    Retirada,
    Manutencao,
    Outro
}

public enum StatusAgendamento
{
    Agendado,
    Confirmado,
    Concluido,
    Cancelado
}
