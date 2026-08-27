using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Services;

public class PagamentoService : IPagamentoService
{
    private readonly IPagamentoRepository _repository;
    private readonly IOrdemServicoRepository _ordemRepository;

    public PagamentoService(IPagamentoRepository repository, IOrdemServicoRepository ordemRepository)
    {
        _repository = repository;
        _ordemRepository = ordemRepository;
    }

    public async Task<PagamentoDto> RegistrarAsync(RegistrarPagamentoDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Valor <= 0)
            throw new InvalidOperationException("O valor do pagamento deve ser maior que zero.");

        var ordem = await _ordemRepository.ObterPorIdAsync(dto.OrdemServicoId, cancellationToken)
            ?? throw new InvalidOperationException("Ordem de serviço não encontrada.");

        if (ordem.SaldoPendente <= 0)
            throw new InvalidOperationException("Esta ordem não possui saldo pendente.");

        if (dto.Valor > ordem.SaldoPendente)
            throw new InvalidOperationException($"O valor do pagamento (R$ {dto.Valor:N2}) excede o saldo pendente (R$ {ordem.SaldoPendente:N2}).");

        var pagamento = new Pagamento
        {
            OrdemServicoId = dto.OrdemServicoId,
            Valor = dto.Valor,
            FormaPagamento = dto.FormaPagamento,
            Observacao = NormalizarTexto(dto.Observacao),
            DataPagamento = DateTime.UtcNow
        };

        await _repository.AdicionarAsync(pagamento, cancellationToken);

        ordem.ValorPago += dto.Valor;
        await _ordemRepository.AtualizarAsync(ordem, cancellationToken);
        await _repository.SalvarAsync(cancellationToken);

        return MapearParaDto(pagamento);
    }

    public async Task<IEnumerable<PagamentoDto>> ListarPorOrdemAsync(int ordemServicoId, CancellationToken cancellationToken = default)
    {
        var pagamentos = await _repository.ListarPorOrdemAsync(ordemServicoId, cancellationToken);
        return pagamentos.Select(MapearParaDto);
    }

    private static string? NormalizarTexto(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return null;
        var trimmed = valor.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

    private static PagamentoDto MapearParaDto(Pagamento pagamento)
    {
        return new PagamentoDto
        {
            Id = pagamento.Id,
            OrdemServicoId = pagamento.OrdemServicoId,
            DataPagamento = pagamento.DataPagamento,
            Valor = pagamento.Valor,
            FormaPagamento = pagamento.FormaPagamento,
            FormaPagamentoDescricao = pagamento.FormaPagamento.ToString(),
            Observacao = pagamento.Observacao
        };
    }
}
