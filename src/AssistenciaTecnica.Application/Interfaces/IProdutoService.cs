using AssistenciaTecnica.Application.DTOs;

namespace AssistenciaTecnica.Application.Interfaces;

public interface IProdutoService
{
    Task<ProdutoDto> CriarAsync(CriarProdutoDto dto, CancellationToken cancellationToken = default);
    Task<ProdutoDto> AtualizarAsync(EditarProdutoDto dto, CancellationToken cancellationToken = default);
    Task<ProdutoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProdutoListagemDto>> ListarAtivosAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ProdutoListagemDto>> ListarEstoqueBaixoAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ProdutoListagemDto>> PesquisarAsync(string termo, CancellationToken cancellationToken = default);
    Task RegistrarEntradaAsync(int produtoId, int quantidade, string? motivo, CancellationToken cancellationToken = default);
    Task RegistrarSaidaAsync(int produtoId, int quantidade, string? motivo, int? ordemServicoId, CancellationToken cancellationToken = default);
    Task RegistrarAjusteAsync(int produtoId, int novaQuantidade, string? motivo, CancellationToken cancellationToken = default);
}
