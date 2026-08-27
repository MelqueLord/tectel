using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repository;
    private readonly IMovimentacaoEstoqueRepository _movimentacaoRepository;

    public ProdutoService(IProdutoRepository repository, IMovimentacaoEstoqueRepository movimentacaoRepository)
    {
        _repository = repository;
        _movimentacaoRepository = movimentacaoRepository;
    }

    public async Task<ProdutoDto> CriarAsync(CriarProdutoDto dto, CancellationToken cancellationToken = default)
    {
        ValidarCamposObrigatorios(dto.Codigo, dto.Nome);

        var existente = await _repository.ObterPorCodigoAsync(dto.Codigo, cancellationToken);
        if (existente is not null)
            throw new InvalidOperationException("Já existe um produto com este código.");

        var produto = new Produto
        {
            Codigo = dto.Codigo.Trim(),
            Nome = dto.Nome.Trim(),
            Descricao = NormalizarTexto(dto.Descricao),
            Categoria = NormalizarTexto(dto.Categoria),
            Marca = NormalizarTexto(dto.Marca),
            ModeloCompativel = NormalizarTexto(dto.ModeloCompativel),
            PrecoCusto = dto.PrecoCusto,
            PrecoVenda = dto.PrecoVenda,
            QuantidadeEstoque = dto.QuantidadeEstoque,
            EstoqueMinimo = dto.EstoqueMinimo,
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        await _repository.AdicionarAsync(produto, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);

        return MapearParaDto(produto);
    }

    public async Task<ProdutoDto> AtualizarAsync(EditarProdutoDto dto, CancellationToken cancellationToken = default)
    {
        var produto = await _repository.ObterPorIdAsync(dto.Id, cancellationToken)
            ?? throw new InvalidOperationException("Produto não encontrado.");

        ValidarCamposObrigatorios(dto.Codigo, dto.Nome);

        var existente = await _repository.ObterPorCodigoAsync(dto.Codigo, cancellationToken);
        if (existente is not null && existente.Id != dto.Id)
            throw new InvalidOperationException("Já existe um produto com este código.");

        produto.Codigo = dto.Codigo.Trim();
        produto.Nome = dto.Nome.Trim();
        produto.Descricao = NormalizarTexto(dto.Descricao);
        produto.Categoria = NormalizarTexto(dto.Categoria);
        produto.Marca = NormalizarTexto(dto.Marca);
        produto.ModeloCompativel = NormalizarTexto(dto.ModeloCompativel);
        produto.PrecoCusto = dto.PrecoCusto;
        produto.PrecoVenda = dto.PrecoVenda;
        produto.EstoqueMinimo = dto.EstoqueMinimo;
        produto.Ativo = dto.Ativo;

        await _repository.AtualizarAsync(produto, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);

        return MapearParaDto(produto);
    }

    public async Task<ProdutoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var produto = await _repository.ObterPorIdAsync(id, cancellationToken);
        return produto is null ? null : MapearParaDto(produto);
    }

    public async Task<IEnumerable<ProdutoListagemDto>> ListarAtivosAsync(CancellationToken cancellationToken = default)
    {
        var produtos = await _repository.ListarAtivosAsync(100, cancellationToken);
        return produtos.Select(MapearParaListagemDto);
    }

    public async Task<IEnumerable<ProdutoListagemDto>> ListarEstoqueBaixoAsync(CancellationToken cancellationToken = default)
    {
        var produtos = await _repository.ListarEstoqueBaixoAsync(cancellationToken);
        return produtos.Select(MapearParaListagemDto);
    }

    public async Task<IEnumerable<ProdutoListagemDto>> PesquisarAsync(string termo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return await ListarAtivosAsync(cancellationToken);

        var produtos = await _repository.PesquisarAsync(termo.Trim(), 100, cancellationToken);
        return produtos.Select(MapearParaListagemDto);
    }

    public async Task RegistrarEntradaAsync(int produtoId, int quantidade, string? motivo, CancellationToken cancellationToken = default)
    {
        if (quantidade <= 0)
            throw new InvalidOperationException("A quantidade deve ser maior que zero.");

        var produto = await _repository.ObterPorIdAsync(produtoId, cancellationToken)
            ?? throw new InvalidOperationException("Produto não encontrado.");

        produto.QuantidadeEstoque += quantidade;
        await _repository.AtualizarAsync(produto, cancellationToken);

        var movimentacao = new MovimentacaoEstoque
        {
            ProdutoId = produtoId,
            Tipo = TipoMovimentacao.Entrada,
            Quantidade = quantidade,
            Motivo = NormalizarTexto(motivo),
            DataMovimentacao = DateTime.UtcNow
        };

        await _movimentacaoRepository.AdicionarAsync(movimentacao, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);
    }

    public async Task RegistrarSaidaAsync(int produtoId, int quantidade, string? motivo, int? ordemServicoId, CancellationToken cancellationToken = default)
    {
        if (quantidade <= 0)
            throw new InvalidOperationException("A quantidade deve ser maior que zero.");

        var produto = await _repository.ObterPorIdAsync(produtoId, cancellationToken)
            ?? throw new InvalidOperationException("Produto não encontrado.");

        if (produto.QuantidadeEstoque < quantidade)
            throw new InvalidOperationException("Estoque insuficiente.");

        produto.QuantidadeEstoque -= quantidade;
        await _repository.AtualizarAsync(produto, cancellationToken);

        var movimentacao = new MovimentacaoEstoque
        {
            ProdutoId = produtoId,
            Tipo = TipoMovimentacao.Saida,
            Quantidade = quantidade,
            Motivo = NormalizarTexto(motivo),
            OrdemServicoId = ordemServicoId,
            DataMovimentacao = DateTime.UtcNow
        };

        await _movimentacaoRepository.AdicionarAsync(movimentacao, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);
    }

    public async Task RegistrarAjusteAsync(int produtoId, int novaQuantidade, string? motivo, CancellationToken cancellationToken = default)
    {
        if (novaQuantidade < 0)
            throw new InvalidOperationException("A quantidade não pode ser negativa.");

        var produto = await _repository.ObterPorIdAsync(produtoId, cancellationToken)
            ?? throw new InvalidOperationException("Produto não encontrado.");

        var diferenca = novaQuantidade - produto.QuantidadeEstoque;
        produto.QuantidadeEstoque = novaQuantidade;
        await _repository.AtualizarAsync(produto, cancellationToken);

        var movimentacao = new MovimentacaoEstoque
        {
            ProdutoId = produtoId,
            Tipo = TipoMovimentacao.Ajuste,
            Quantidade = Math.Abs(diferenca),
            Motivo = NormalizarTexto(motivo),
            DataMovimentacao = DateTime.UtcNow
        };

        await _movimentacaoRepository.AdicionarAsync(movimentacao, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);
    }

    private static void ValidarCamposObrigatorios(string codigo, string nome)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new InvalidOperationException("O código é obrigatório.");
        if (string.IsNullOrWhiteSpace(nome))
            throw new InvalidOperationException("O nome é obrigatório.");
    }

    private static string? NormalizarTexto(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return null;
        var trimmed = valor.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

    private static ProdutoDto MapearParaDto(Produto produto)
    {
        return new ProdutoDto
        {
            Id = produto.Id,
            Codigo = produto.Codigo,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Categoria = produto.Categoria,
            Marca = produto.Marca,
            ModeloCompativel = produto.ModeloCompativel,
            PrecoCusto = produto.PrecoCusto,
            PrecoVenda = produto.PrecoVenda,
            QuantidadeEstoque = produto.QuantidadeEstoque,
            EstoqueMinimo = produto.EstoqueMinimo,
            Ativo = produto.Ativo,
            DataCadastro = produto.DataCadastro
        };
    }

    private static ProdutoListagemDto MapearParaListagemDto(Produto produto)
    {
        return new ProdutoListagemDto
        {
            Id = produto.Id,
            Codigo = produto.Codigo,
            Nome = produto.Nome,
            Categoria = produto.Categoria,
            PrecoVenda = produto.PrecoVenda,
            QuantidadeEstoque = produto.QuantidadeEstoque,
            EstoqueMinimo = produto.EstoqueMinimo,
            EstoqueBaixo = produto.EstoqueBaixo
        };
    }
}
