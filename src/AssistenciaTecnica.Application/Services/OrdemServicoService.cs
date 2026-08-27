using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Services;

public class OrdemServicoService : IOrdemServicoService
{
    private readonly IOrdemServicoRepository _repository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IAparelhoRepository _aparelhoRepository;
    private readonly ITecnicoRepository _tecnicoRepository;

    public OrdemServicoService(
        IOrdemServicoRepository repository,
        IClienteRepository clienteRepository,
        IAparelhoRepository aparelhoRepository,
        ITecnicoRepository tecnicoRepository)
    {
        _repository = repository;
        _clienteRepository = clienteRepository;
        _aparelhoRepository = aparelhoRepository;
        _tecnicoRepository = tecnicoRepository;
    }

    public async Task<OrdemServicoDto> CriarAsync(CriarOrdemServicoDto dto, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(dto.ClienteId, cancellationToken)
            ?? throw new InvalidOperationException("Cliente não encontrado.");

        var aparelho = await _aparelhoRepository.ObterPorIdAsync(dto.AparelhoId, cancellationToken)
            ?? throw new InvalidOperationException("Aparelho não encontrado.");

        if (string.IsNullOrWhiteSpace(dto.DefeitoRelatado))
            throw new InvalidOperationException("O defeito relatado é obrigatório.");

        var numero = await _repository.GerarProximoNumeroAsync(cancellationToken);

        var ordem = new OrdemServico
        {
            Numero = numero,
            ClienteId = dto.ClienteId,
            AparelhoId = dto.AparelhoId,
            TecnicoId = dto.TecnicoId,
            DefeitoRelatado = dto.DefeitoRelatado.Trim(),
            PrevisaoEntrega = dto.PrevisaoEntrega,
            Observacoes = NormalizarTexto(dto.Observacoes),
            PrazoGarantiaDias = dto.PrazoGarantiaDias,
            Status = StatusOrdemServico.Aberta,
            DataEntrada = DateTime.UtcNow
        };

        await _repository.AdicionarAsync(ordem, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);

        return MapearParaDto(ordem, cliente.Nome, $"{aparelho.Marca} {aparelho.Modelo}");
    }

    public async Task<OrdemServicoDto> AtualizarAsync(EditarOrdemServicoDto dto, CancellationToken cancellationToken = default)
    {
        var ordem = await _repository.ObterPorIdAsync(dto.Id, cancellationToken)
            ?? throw new InvalidOperationException("Ordem de serviço não encontrada.");

        ordem.TecnicoId = dto.TecnicoId;
        ordem.Diagnostico = NormalizarTexto(dto.Diagnostico);
        ordem.ServicoRealizado = NormalizarTexto(dto.ServicoRealizado);
        ordem.PrevisaoEntrega = dto.PrevisaoEntrega;
        ordem.ValorServico = dto.ValorServico;
        ordem.ValorPecas = dto.ValorPecas;
        ordem.Desconto = dto.Desconto;
        ordem.Observacoes = NormalizarTexto(dto.Observacoes);
        ordem.PrazoGarantiaDias = dto.PrazoGarantiaDias;
        ordem.CalcularValorTotal();

        await _repository.AtualizarAsync(ordem, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);

        return MapearParaDto(ordem, ordem.Cliente?.Nome ?? "",
            ordem.Aparelho != null ? $"{ordem.Aparelho.Marca} {ordem.Aparelho.Modelo}" : "");
    }

    public async Task<OrdemServicoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var ordem = await _repository.ObterPorIdAsync(id, cancellationToken);
        if (ordem is null) return null;

        return MapearParaDto(ordem, ordem.Cliente?.Nome ?? "",
            ordem.Aparelho != null ? $"{ordem.Aparelho.Marca} {ordem.Aparelho.Modelo}" : "");
    }

    public async Task<IEnumerable<OrdemServicoListagemDto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var ordens = await _repository.ListarAsync(100, cancellationToken);
        return ordens.Select(MapearParaListagemDto);
    }

    public async Task<IEnumerable<OrdemServicoListagemDto>> ListarPorStatusAsync(StatusOrdemServico status, CancellationToken cancellationToken = default)
    {
        var ordens = await _repository.ListarPorStatusAsync(status, 100, cancellationToken);
        return ordens.Select(MapearParaListagemDto);
    }

    public async Task<IEnumerable<OrdemServicoListagemDto>> PesquisarAsync(string termo, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return await ListarAsync(cancellationToken);

        var ordens = await _repository.PesquisarAsync(termo.Trim(), 100, cancellationToken);
        return ordens.Select(MapearParaListagemDto);
    }

    public async Task AlterarStatusAsync(int id, StatusOrdemServico novoStatus, CancellationToken cancellationToken = default)
    {
        var ordem = await _repository.ObterPorIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Ordem de serviço não encontrada.");

        ValidarTransicaoStatus(ordem.Status, novoStatus);

        ordem.Status = novoStatus;

        if (novoStatus == StatusOrdemServico.Concluida)
            ordem.DataConclusao = DateTime.UtcNow;

        if (novoStatus == StatusOrdemServico.Entregue)
            ordem.DataEntrega = DateTime.UtcNow;

        await _repository.AtualizarAsync(ordem, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);
    }

    public async Task<DashboardDto> ObterDashboardAsync(CancellationToken cancellationToken = default)
    {
        var abertas = await _repository.ContarPorStatusAsync(StatusOrdemServico.Aberta, cancellationToken);
        var emManutencao = await _repository.ContarPorStatusAsync(StatusOrdemServico.EmManutencao, cancellationToken);
        var aguardandoAprovacao = await _repository.ContarPorStatusAsync(StatusOrdemServico.AguardandoAprovacao, cancellationToken);
        var aguardandoRetirada = await _repository.ContarPorStatusAsync(StatusOrdemServico.Concluida, cancellationToken);
        var concluidasNoMes = await _repository.ContarPorStatusAsync(StatusOrdemServico.Entregue, cancellationToken);

        var ordensRecentes = await _repository.ListarAsync(10, cancellationToken);

        return new DashboardDto
        {
            OrdensAbertas = abertas,
            EmManutencao = emManutencao,
            AguardandoAprovacao = aguardandoAprovacao,
            AguardandoRetirada = aguardandoRetirada,
            ConcluidasNoMes = concluidasNoMes,
            OrdensRecentes = ordensRecentes.Select(MapearParaListagemDto).ToList()
        };
    }

    private static void ValidarTransicaoStatus(StatusOrdemServico atual, StatusOrdemServico novo)
    {
        var transicoesPermitidas = new Dictionary<StatusOrdemServico, StatusOrdemServico[]>
        {
            [StatusOrdemServico.Aberta] = [StatusOrdemServico.EmAnalise, StatusOrdemServico.Cancelada],
            [StatusOrdemServico.EmAnalise] = [StatusOrdemServico.AguardandoAprovacao, StatusOrdemServico.EmManutencao, StatusOrdemServico.Cancelada],
            [StatusOrdemServico.AguardandoAprovacao] = [StatusOrdemServico.EmManutencao, StatusOrdemServico.Cancelada],
            [StatusOrdemServico.EmManutencao] = [StatusOrdemServico.Concluida, StatusOrdemServico.Cancelada],
            [StatusOrdemServico.Concluida] = [StatusOrdemServico.Entregue],
            [StatusOrdemServico.Entregue] = [],
            [StatusOrdemServico.Cancelada] = []
        };

        if (!transicoesPermitidas.ContainsKey(atual) || !transicoesPermitidas[atual].Contains(novo))
            throw new InvalidOperationException($"Não é possível alterar o status de {ObterDescricaoStatus(atual)} para {ObterDescricaoStatus(novo)}.");
    }

    private static string ObterDescricaoStatus(StatusOrdemServico status)
    {
        return status switch
        {
            StatusOrdemServico.Aberta => "Aberta",
            StatusOrdemServico.EmAnalise => "Em Análise",
            StatusOrdemServico.AguardandoAprovacao => "Aguardando Aprovação",
            StatusOrdemServico.EmManutencao => "Em Manutenção",
            StatusOrdemServico.Concluida => "Concluída",
            StatusOrdemServico.Entregue => "Entregue",
            StatusOrdemServico.Cancelada => "Cancelada",
            _ => status.ToString()
        };
    }

    private static string? NormalizarTexto(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return null;
        var trimmed = valor.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

    private static OrdemServicoDto MapearParaDto(OrdemServico ordem, string clienteNome, string aparelhoDescricao)
    {
        return new OrdemServicoDto
        {
            Id = ordem.Id,
            Numero = ordem.Numero,
            ClienteId = ordem.ClienteId,
            ClienteNome = clienteNome,
            AparelhoId = ordem.AparelhoId,
            AparelhoDescricao = aparelhoDescricao,
            TecnicoId = ordem.TecnicoId,
            TecnicoNome = ordem.Tecnico?.Nome,
            DefeitoRelatado = ordem.DefeitoRelatado,
            Diagnostico = ordem.Diagnostico,
            ServicoRealizado = ordem.ServicoRealizado,
            DataEntrada = ordem.DataEntrada,
            PrevisaoEntrega = ordem.PrevisaoEntrega,
            DataConclusao = ordem.DataConclusao,
            DataEntrega = ordem.DataEntrega,
            Status = ordem.Status,
            StatusDescricao = ObterDescricaoStatus(ordem.Status),
            ValorServico = ordem.ValorServico,
            ValorPecas = ordem.ValorPecas,
            Desconto = ordem.Desconto,
            ValorTotal = ordem.ValorTotal,
            ValorPago = ordem.ValorPago,
            SaldoPendente = ordem.SaldoPendente,
            Observacoes = ordem.Observacoes,
            PrazoGarantiaDias = ordem.PrazoGarantiaDias,
            Pagamentos = ordem.Pagamentos?.Select(p => new PagamentoDto
            {
                Id = p.Id,
                OrdemServicoId = p.OrdemServicoId,
                DataPagamento = p.DataPagamento,
                Valor = p.Valor,
                FormaPagamento = p.FormaPagamento,
                FormaPagamentoDescricao = p.FormaPagamento.ToString(),
                Observacao = p.Observacao
            }).ToList() ?? []
        };
    }

    private static OrdemServicoListagemDto MapearParaListagemDto(OrdemServico ordem)
    {
        return new OrdemServicoListagemDto
        {
            Id = ordem.Id,
            Numero = ordem.Numero,
            ClienteId = ordem.ClienteId,
            ClienteNome = ordem.Cliente?.Nome ?? "",
            AparelhoDescricao = ordem.Aparelho != null ? $"{ordem.Aparelho.Marca} {ordem.Aparelho.Modelo}" : "",
            TecnicoNome = ordem.Tecnico?.Nome,
            DefeitoRelatado = ordem.DefeitoRelatado,
            DataEntrada = ordem.DataEntrada,
            PrevisaoEntrega = ordem.PrevisaoEntrega,
            Status = ordem.Status,
            StatusDescricao = ObterDescricaoStatus(ordem.Status),
            ValorTotal = ordem.ValorTotal
        };
    }
}
