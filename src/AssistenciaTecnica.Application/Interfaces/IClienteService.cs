using AssistenciaTecnica.Application.DTOs;

namespace AssistenciaTecnica.Application.Interfaces;

public interface IClienteService
{
    Task<ClienteDto> CriarAsync(CriarClienteDto dto, CancellationToken cancellationToken = default);
    Task<ClienteDto> AtualizarAsync(EditarClienteDto dto, CancellationToken cancellationToken = default);
    Task<ClienteDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClienteListagemDto>> ListarAtivosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ClienteListagemDto>> ListarInativosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ClienteListagemDto>> ListarTodosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ClienteListagemDto>> PesquisarAsync(string termo, CancellationToken cancellationToken = default);
    Task InativarAsync(int id, CancellationToken cancellationToken = default);
    Task ReativarAsync(int id, CancellationToken cancellationToken = default);
}
