using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.DTOs;

public class AgendamentoDto
{
    public int Id { get; set; }
    public int? OrdemServicoId { get; set; }
    public string? OrdemServicoNumero { get; set; }
    public int? ClienteId { get; set; }
    public string? ClienteNome { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public TipoAgendamento Tipo { get; set; }
    public string TipoDescricao { get; set; } = string.Empty;
    public StatusAgendamento Status { get; set; }
    public string StatusDescricao { get; set; } = string.Empty;
    public string? Cor { get; set; }
}

public class AgendamentoFormDto
{
    public int Id { get; set; }
    public int? OrdemServicoId { get; set; }
    public int? ClienteId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataInicio { get; set; } = DateTime.Today;
    public string HoraInicio { get; set; } = "09:00";
    public DateTime DataFim { get; set; } = DateTime.Today;
    public string HoraFim { get; set; } = "10:00";
    public TipoAgendamento Tipo { get; set; } = TipoAgendamento.Entrega;
    public StatusAgendamento Status { get; set; } = StatusAgendamento.Agendado;
}

public class AgendamentoCalendarioDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Color { get; set; } = string.Empty;
    public string? Url { get; set; }
    public bool AllDay { get; set; }
}

public class CriarAgendamentoDto
{
    public int? OrdemServicoId { get; set; }
    public int? ClienteId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public TipoAgendamento Tipo { get; set; }
}
