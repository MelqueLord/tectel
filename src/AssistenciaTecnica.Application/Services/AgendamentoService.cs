using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Services;

public class AgendamentoService : IAgendamentoService
{
    private readonly IAgendamentoRepository _repository;

    public AgendamentoService(IAgendamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<AgendamentoDto> CriarAsync(CriarAgendamentoDto dto, CancellationToken cancellationToken = default)
    {
        var agendamento = new Agendamento
        {
            OrdemServicoId = dto.OrdemServicoId,
            ClienteId = dto.ClienteId,
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            DataInicio = dto.DataInicio,
            DataFim = dto.DataFim,
            Tipo = dto.Tipo,
            Status = StatusAgendamento.Agendado,
            Cor = ObterCorPorTipo(dto.Tipo)
        };

        await _repository.AdicionarAsync(agendamento, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);

        return MapearParaDto(agendamento);
    }

    public async Task<AgendamentoDto> AtualizarAsync(AgendamentoFormDto dto, CancellationToken cancellationToken = default)
    {
        var agendamento = await _repository.ObterPorIdAsync(dto.Id, cancellationToken)
            ?? throw new InvalidOperationException("Agendamento não encontrado.");

        agendamento.OrdemServicoId = dto.OrdemServicoId;
        agendamento.ClienteId = dto.ClienteId;
        agendamento.Titulo = dto.Titulo;
        agendamento.Descricao = dto.Descricao;
        agendamento.DataInicio = dto.DataInicio;
        agendamento.DataFim = dto.DataFim;
        agendamento.Tipo = dto.Tipo;
        agendamento.Status = dto.Status;
        agendamento.Cor = ObterCorPorTipo(dto.Tipo);

        await _repository.AtualizarAsync(agendamento, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);

        return MapearParaDto(agendamento);
    }

    public async Task<AgendamentoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var agendamento = await _repository.ObterPorIdAsync(id, cancellationToken);
        return agendamento is not null ? MapearParaDto(agendamento) : null;
    }

    public async Task<IEnumerable<AgendamentoDto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var agendamentos = await _repository.ListarAsync(cancellationToken);
        return agendamentos.Select(MapearParaDto);
    }

    public async Task<IEnumerable<AgendamentoCalendarioDto>> ListarCalendarioAsync(DateTime inicio, DateTime fim, CancellationToken cancellationToken = default)
    {
        var agendamentos = await _repository.ListarPorPeriodoAsync(inicio, fim, cancellationToken);

        return agendamentos.Select(a => new AgendamentoCalendarioDto
        {
            Id = a.Id,
            Title = a.Titulo,
            Start = a.DataInicio,
            End = a.DataFim,
            Color = a.Cor ?? ObterCorPorTipo(a.Tipo),
            Url = $"/Agendamentos/Detalhes/{a.Id}",
            AllDay = a.DataInicio.Date == a.DataFim.Date && a.DataInicio.TimeOfDay == TimeSpan.Zero
        });
    }

    public async Task<IEnumerable<AgendamentoDto>> ListarHojeAsync(CancellationToken cancellationToken = default)
    {
        var agendamentos = await _repository.ListarHojeAsync(cancellationToken);
        return agendamentos.Select(MapearParaDto);
    }

    public async Task<IEnumerable<AgendamentoDto>> ListarPorOrdemAsync(int ordemServicoId, CancellationToken cancellationToken = default)
    {
        var agendamentos = await _repository.ListarPorOrdemAsync(ordemServicoId, cancellationToken);
        return agendamentos.Select(MapearParaDto);
    }

    public async Task AlterarStatusAsync(int id, StatusAgendamento novoStatus, CancellationToken cancellationToken = default)
    {
        var agendamento = await _repository.ObterPorIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Agendamento não encontrado.");

        agendamento.Status = novoStatus;
        await _repository.AtualizarAsync(agendamento, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);
    }

    private static AgendamentoDto MapearParaDto(Agendamento a)
    {
        return new AgendamentoDto
        {
            Id = a.Id,
            OrdemServicoId = a.OrdemServicoId,
            OrdemServicoNumero = a.OrdemServico?.Numero,
            ClienteId = a.ClienteId,
            ClienteNome = a.Cliente?.Nome ?? a.OrdemServico?.Cliente?.Nome,
            Titulo = a.Titulo,
            Descricao = a.Descricao,
            DataInicio = a.DataInicio,
            DataFim = a.DataFim,
            Tipo = a.Tipo,
            TipoDescricao = a.Tipo.ToString(),
            Status = a.Status,
            StatusDescricao = a.Status.ToString(),
            Cor = a.Cor
        };
    }

    private static string ObterCorPorTipo(TipoAgendamento tipo)
    {
        return tipo switch
        {
            TipoAgendamento.Entrega => "#198754",
            TipoAgendamento.Revisao => "#0d6efd",
            TipoAgendamento.Retirada => "#6c757d",
            TipoAgendamento.Manutencao => "#fd7e14",
            _ => "#6f42c1"
        };
    }
}
