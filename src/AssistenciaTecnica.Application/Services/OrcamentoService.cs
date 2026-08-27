using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Services;

public class OrcamentoService : IOrcamentoService
{
    private readonly IOrcamentoRepository _repository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IAparelhoRepository _aparelhoRepository;
    private readonly IOrdemServicoRepository _ordemServicoRepository;

    public OrcamentoService(
        IOrcamentoRepository repository,
        IClienteRepository clienteRepository,
        IAparelhoRepository aparelhoRepository,
        IOrdemServicoRepository ordemServicoRepository)
    {
        _repository = repository;
        _clienteRepository = clienteRepository;
        _aparelhoRepository = aparelhoRepository;
        _ordemServicoRepository = ordemServicoRepository;
    }

    public async Task<OrcamentoDto> CriarAsync(CriarOrcamentoDto dto, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(dto.ClienteId, cancellationToken)
            ?? throw new InvalidOperationException("Cliente não encontrado.");

        var aparelho = await _aparelhoRepository.ObterPorIdAsync(dto.AparelhoId, cancellationToken)
            ?? throw new InvalidOperationException("Aparelho não encontrado.");

        if (string.IsNullOrWhiteSpace(dto.DefeitoRelatado))
            throw new InvalidOperationException("O defeito relatado é obrigatório.");

        if (dto.Itens.Count == 0)
            throw new InvalidOperationException("O orçamento deve ter pelo menos um item.");

        var numero = await _repository.GerarProximoNumeroAsync(cancellationToken);

        var orcamento = new Orcamento
        {
            Numero = numero,
            ClienteId = dto.ClienteId,
            AparelhoId = dto.AparelhoId,
            DefeitoRelatado = dto.DefeitoRelatado.Trim(),
            Diagnostico = NormalizarTexto(dto.Diagnostico),
            Observacoes = NormalizarTexto(dto.Observacoes),
            DataValidade = dto.DataValidade,
            Desconto = dto.Desconto,
            Status = StatusOrcamento.Rascunho,
            DataCriacao = DateTime.UtcNow,
            Itens = dto.Itens.Select(i => new OrcamentoItem
            {
                Tipo = i.Tipo,
                Descricao = i.Descricao.Trim(),
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario
            }).ToList()
        };

        orcamento.CalcularValorTotal();

        await _repository.AdicionarAsync(orcamento, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);

        return MapearParaDto(orcamento, cliente, aparelho);
    }

    public async Task<OrcamentoDto> AtualizarAsync(EditarOrcamentoDto dto, CancellationToken cancellationToken = default)
    {
        var orcamento = await _repository.ObterPorIdAsync(dto.Id, cancellationToken)
            ?? throw new InvalidOperationException("Orçamento não encontrado.");

        if (orcamento.Status != StatusOrcamento.Rascunho)
            throw new InvalidOperationException("Somente orçamentos em rascunho podem ser editados.");

        orcamento.Diagnostico = NormalizarTexto(dto.Diagnostico);
        orcamento.Observacoes = NormalizarTexto(dto.Observacoes);
        orcamento.DataValidade = dto.DataValidade;
        orcamento.Desconto = dto.Desconto;

        orcamento.Itens.Clear();
        foreach (var itemDto in dto.Itens)
        {
            orcamento.Itens.Add(new OrcamentoItem
            {
                OrcamentoId = orcamento.Id,
                Tipo = itemDto.Tipo,
                Descricao = itemDto.Descricao.Trim(),
                Quantidade = itemDto.Quantidade,
                ValorUnitario = itemDto.ValorUnitario
            });
        }

        orcamento.CalcularValorTotal();

        await _repository.AtualizarAsync(orcamento, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);

        return MapearParaDto(orcamento, orcamento.Cliente, orcamento.Aparelho);
    }

    public async Task<OrcamentoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var orcamento = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (orcamento is null) return null;

        return MapearParaDto(orcamento, orcamento.Cliente, orcamento.Aparelho);
    }

    public async Task<IEnumerable<OrcamentoListagemDto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var orcamentos = await _repository.ListarAsync(100, cancellationToken);
        return orcamentos.Select(MapearParaListagemDto);
    }

    public async Task<IEnumerable<OrcamentoListagemDto>> ListarPorStatusAsync(StatusOrcamento status, CancellationToken cancellationToken = default)
    {
        var orcamentos = await _repository.ListarPorStatusAsync(status, 100, cancellationToken);
        return orcamentos.Select(MapearParaListagemDto);
    }

    public async Task<IEnumerable<OrcamentoListagemDto>> PesquisarAsync(string termo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return await ListarAsync(cancellationToken);

        var orcamentos = await _repository.PesquisarAsync(termo.Trim(), 100, cancellationToken);
        return orcamentos.Select(MapearParaListagemDto);
    }

    public async Task AlterarStatusAsync(int id, StatusOrcamento novoStatus, CancellationToken cancellationToken = default)
    {
        var orcamento = await _repository.ObterPorIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Orçamento não encontrado.");

        ValidarTransicaoStatus(orcamento.Status, novoStatus);

        orcamento.Status = novoStatus;

        if (novoStatus == StatusOrcamento.Enviado)
            orcamento.DataEnvio = DateTime.UtcNow;

        if (novoStatus == StatusOrcamento.Aprovado)
            orcamento.DataAprovacao = DateTime.UtcNow;

        await _repository.AtualizarAsync(orcamento, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);
    }

    public async Task<OrdemServicoDto> ConverterEmOrdemServicoAsync(int orcamentoId, CancellationToken cancellationToken = default)
    {
        var orcamento = await _repository.ObterPorIdAsync(orcamentoId, cancellationToken)
            ?? throw new InvalidOperationException("Orçamento não encontrado.");

        if (orcamento.Status != StatusOrcamento.Aprovado)
            throw new InvalidOperationException("Somente orçamentos aprovados podem ser convertidos em OS.");

        var numero = await _ordemServicoRepository.GerarProximoNumeroAsync(cancellationToken);

        var descricaoItens = string.Join("\n", orcamento.Itens.Select(i => $"- {i.Descricao} ({i.Tipo})"));

        var ordem = new OrdemServico
        {
            Numero = numero,
            ClienteId = orcamento.ClienteId,
            AparelhoId = orcamento.AparelhoId,
            DefeitoRelatado = orcamento.DefeitoRelatado,
            Diagnostico = orcamento.Diagnostico,
            Observacoes = $"Convertido do orçamento {orcamento.Numero}\n\nItens do orçamento:\n{descricaoItens}",
            ValorServico = orcamento.ValorServicos,
            ValorPecas = orcamento.ValorPecas,
            Desconto = orcamento.Desconto,
            ValorTotal = orcamento.ValorTotal,
            Status = StatusOrdemServico.Aberta,
            DataEntrada = DateTime.UtcNow,
            PrazoGarantiaDias = 90
        };

        await _ordemServicoRepository.AdicionarAsync(ordem, cancellationToken);
        await _ordemServicoRepository.SalvarAsync(cancellationToken);

        orcamento.Status = StatusOrcamento.Convertido;
        orcamento.OrdemServicoId = ordem.Id;
        await _repository.AtualizarAsync(orcamento, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);

        return new OrdemServicoDto
        {
            Id = ordem.Id,
            Numero = ordem.Numero,
            ClienteId = ordem.ClienteId,
            ClienteNome = orcamento.Cliente?.Nome ?? "",
            AparelhoId = ordem.AparelhoId,
            AparelhoDescricao = orcamento.Aparelho != null ? $"{orcamento.Aparelho.Marca} {orcamento.Aparelho.Modelo}" : "",
            DefeitoRelatado = ordem.DefeitoRelatado,
            Diagnostico = ordem.Diagnostico,
            Observacoes = ordem.Observacoes,
            ValorServico = ordem.ValorServico,
            ValorPecas = ordem.ValorPecas,
            Desconto = ordem.Desconto,
            ValorTotal = ordem.ValorTotal,
            Status = ordem.Status,
            StatusDescricao = "Aberta",
            DataEntrada = ordem.DataEntrada
        };
    }

    private static void ValidarTransicaoStatus(StatusOrcamento atual, StatusOrcamento novo)
    {
        var transicoesPermitidas = new Dictionary<StatusOrcamento, StatusOrcamento[]>
        {
            [StatusOrcamento.Rascunho] = [StatusOrcamento.Enviado, StatusOrcamento.Rejeitado],
            [StatusOrcamento.Enviado] = [StatusOrcamento.Aprovado, StatusOrcamento.Rejeitado, StatusOrcamento.Expirado],
            [StatusOrcamento.Aprovado] = [StatusOrcamento.Convertido, StatusOrcamento.Rejeitado],
            [StatusOrcamento.Rejeitado] = [],
            [StatusOrcamento.Convertido] = [],
            [StatusOrcamento.Expirado] = []
        };

        if (!transicoesPermitidas.ContainsKey(atual) || !transicoesPermitidas[atual].Contains(novo))
            throw new InvalidOperationException($"Não é possível alterar o status de {ObterDescricaoStatus(atual)} para {ObterDescricaoStatus(novo)}.");
    }

    private static string ObterDescricaoStatus(StatusOrcamento status)
    {
        return status switch
        {
            StatusOrcamento.Rascunho => "Rascunho",
            StatusOrcamento.Enviado => "Enviado",
            StatusOrcamento.Aprovado => "Aprovado",
            StatusOrcamento.Rejeitado => "Rejeitado",
            StatusOrcamento.Convertido => "Convertido em OS",
            StatusOrcamento.Expirado => "Expirado",
            _ => status.ToString()
        };
    }

    private static string ObterCorStatus(StatusOrcamento status)
    {
        return status switch
        {
            StatusOrcamento.Rascunho => "#6c757d",
            StatusOrcamento.Enviado => "#0dcaf0",
            StatusOrcamento.Aprovado => "#198754",
            StatusOrcamento.Rejeitado => "#dc3545",
            StatusOrcamento.Convertido => "#0d6efd",
            StatusOrcamento.Expirado => "#ffc107",
            _ => "#6c757d"
        };
    }

    private static string? NormalizarTexto(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return null;
        var trimmed = valor.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

    private static OrcamentoDto MapearParaDto(Orcamento orcamento, Cliente? cliente, Aparelho? aparelho)
    {
        return new OrcamentoDto
        {
            Id = orcamento.Id,
            Numero = orcamento.Numero,
            ClienteId = orcamento.ClienteId,
            ClienteNome = cliente?.Nome ?? orcamento.Cliente?.Nome ?? "",
            ClienteTelefone = cliente?.Telefone ?? orcamento.Cliente?.Telefone ?? "",
            AparelhoId = orcamento.AparelhoId,
            AparelhoDescricao = aparelho != null ? $"{aparelho.Marca} {aparelho.Modelo}" :
                orcamento.Aparelho != null ? $"{orcamento.Aparelho.Marca} {orcamento.Aparelho.Modelo}" : "",
            DefeitoRelatado = orcamento.DefeitoRelatado,
            Diagnostico = orcamento.Diagnostico,
            Observacoes = orcamento.Observacoes,
            DataCriacao = orcamento.DataCriacao,
            DataValidade = orcamento.DataValidade,
            DataEnvio = orcamento.DataEnvio,
            DataAprovacao = orcamento.DataAprovacao,
            Status = orcamento.Status,
            StatusDescricao = ObterDescricaoStatus(orcamento.Status),
            ValorServicos = orcamento.ValorServicos,
            ValorPecas = orcamento.ValorPecas,
            Desconto = orcamento.Desconto,
            ValorTotal = orcamento.ValorTotal,
            OrdemServicoId = orcamento.OrdemServicoId,
            OrdemServicoNumero = orcamento.OrdemServico?.Numero,
            Itens = orcamento.Itens?.Select(i => new OrcamentoItemDto
            {
                Id = i.Id,
                Tipo = i.Tipo,
                TipoDescricao = i.Tipo == TipoItemOrcamento.Servico ? "Serviço" : "Peça",
                Descricao = i.Descricao,
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario,
                Subtotal = i.Subtotal
            }).ToList() ?? []
        };
    }

    private static OrcamentoListagemDto MapearParaListagemDto(Orcamento orcamento)
    {
        return new OrcamentoListagemDto
        {
            Id = orcamento.Id,
            Numero = orcamento.Numero,
            ClienteNome = orcamento.Cliente?.Nome ?? "",
            AparelhoDescricao = orcamento.Aparelho != null ? $"{orcamento.Aparelho.Marca} {orcamento.Aparelho.Modelo}" : "",
            DataCriacao = orcamento.DataCriacao,
            DataValidade = orcamento.DataValidade,
            Status = orcamento.Status,
            StatusDescricao = ObterDescricaoStatus(orcamento.Status),
            ValorTotal = orcamento.ValorTotal,
            PodeConverter = orcamento.Status == StatusOrcamento.Aprovado
        };
    }
}
