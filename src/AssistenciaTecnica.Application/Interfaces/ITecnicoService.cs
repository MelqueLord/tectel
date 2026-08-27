using AssistenciaTecnica.Application.DTOs;

namespace AssistenciaTecnica.Application.Interfaces;

public interface ITecnicoService
{
    Task<TecnicoDto> CriarAsync(TecnicoFormDto dto, CancellationToken cancellationToken = default);
    Task<TecnicoDto> AtualizarAsync(TecnicoFormDto dto, CancellationToken cancellationToken = default);
    Task<TecnicoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TecnicoListagemDto>> ListarAtivosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TecnicoListagemDto>> ListarTodosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TecnicoListagemDto>> PesquisarAsync(string termo, CancellationToken cancellationToken = default);
    Task InativarAsync(int id, CancellationToken cancellationToken = default);
    Task ReativarAsync(int id, CancellationToken cancellationToken = default);
}
