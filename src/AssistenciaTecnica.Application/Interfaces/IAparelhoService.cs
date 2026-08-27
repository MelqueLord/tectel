using AssistenciaTecnica.Application.DTOs;

namespace AssistenciaTecnica.Application.Interfaces;

public interface IAparelhoService
{
    Task<AparelhoDto> CriarAsync(CriarAparelhoDto dto, CancellationToken cancellationToken = default);
    Task<AparelhoDto> AtualizarAsync(EditarAparelhoDto dto, CancellationToken cancellationToken = default);
    Task<AparelhoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AparelhoDto>> ListarAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AparelhoDto>> ListarPorClienteAsync(int clienteId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AparelhoDto>> PesquisarAsync(string termo, CancellationToken cancellationToken = default);
}
