using AssistenciaTecnica.Application.DTOs;
using AssistenciaTecnica.Application.Interfaces;
using AssistenciaTecnica.Domain.Entities;

namespace AssistenciaTecnica.Application.Services;

public class NotificacaoService : INotificacaoService
{
    private readonly IOrdemServicoRepository _ordemRepository;
    private readonly IProdutoRepository _produtoRepository;

    public NotificacaoService(IOrdemServicoRepository ordemRepository, IProdutoRepository produtoRepository)
    {
        _ordemRepository = ordemRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<NotificacaoResumoDto> ObterNotificacoesAsync(CancellationToken cancellationToken = default)
    {
        var notificacoes = new List<NotificacaoDto>();

        // Estoque baixo
        var produtosEstoqueBaixo = await _produtoRepository.ListarEstoqueBaixoAsync(cancellationToken);
        foreach (var produto in produtosEstoqueBaixo)
        {
            notificacoes.Add(new NotificacaoDto
            {
                Tipo = "EstoqueBaixo",
                Titulo = "Estoque Baixo",
                Mensagem = $"{produto.Nome} - {produto.QuantidadeEstoque} un. (mín: {produto.EstoqueMinimo})",
                Link = "/Estoque",
                Icone = "bi-exclamation-triangle",
                Cor = "danger"
            });
        }

        // Garantia vencendo (próximos 7 dias)
        var todasOrdens = await _ordemRepository.ListarAsync(1000, cancellationToken);
        var hoje = DateTime.UtcNow;

        foreach (var ordem in todasOrdens.Where(o => o.Status == StatusOrdemServico.Entregue))
        {
            var dataFimGarantia = ordem.DataEntrega?.AddDays(ordem.PrazoGarantiaDias)
                                  ?? ordem.DataConclusao?.AddDays(ordem.PrazoGarantiaDias);

            if (dataFimGarantia.HasValue)
            {
                var diasRestantes = (dataFimGarantia.Value - hoje).Days;

                if (diasRestantes < 0)
                {
                    notificacoes.Add(new NotificacaoDto
                    {
                        Tipo = "GarantiaVencida",
                        Titulo = "Garantia Vencida",
                        Mensagem = $"OS {ordem.Numero} - {ordem.Cliente.Nome} - venceu há {Math.Abs(diasRestantes)} dias",
                        Link = $"/OrdensServico/Detalhes/{ordem.Id}",
                        Icone = "bi-shield-x",
                        Cor = "danger",
                        DataReferencia = dataFimGarantia
                    });
                }
                else if (diasRestantes <= 7)
                {
                    notificacoes.Add(new NotificacaoDto
                    {
                        Tipo = "GarantiaVencendo",
                        Titulo = "Garantia Vencendo",
                        Mensagem = $"OS {ordem.Numero} - {ordem.Cliente.Nome} - vence em {diasRestantes} dias",
                        Link = $"/OrdensServico/Detalhes/{ordem.Id}",
                        Icone = "bi-shield-exclamation",
                        Cor = "warning",
                        DataReferencia = dataFimGarantia
                    });
                }
            }
        }

        // Ordens com previsão de entrega hoje ou atrasadas
        foreach (var ordem in todasOrdens.Where(o =>
            o.Status != StatusOrdemServico.Entregue &&
            o.Status != StatusOrdemServico.Cancelada &&
            o.PrevisaoEntrega.HasValue))
        {
            var diasAteEntrega = (ordem.PrevisaoEntrega!.Value - hoje).Days;

            if (diasAteEntrega < 0)
            {
                notificacoes.Add(new NotificacaoDto
                {
                    Tipo = "EntregaAtrasada",
                    Titulo = "Entrega Atrasada",
                    Mensagem = $"OS {ordem.Numero} - {ordem.Cliente.Nome} - atrasada {Math.Abs(diasAteEntrega)} dias",
                    Link = $"/OrdensServico/Detalhes/{ordem.Id}",
                    Icone = "bi-clock-history",
                    Cor = "danger",
                    DataReferencia = ordem.PrevisaoEntrega
                });
            }
            else if (diasAteEntrega == 0)
            {
                notificacoes.Add(new NotificacaoDto
                {
                    Tipo = "EntregaHoje",
                    Titulo = "Entrega Hoje",
                    Mensagem = $"OS {ordem.Numero} - {ordem.Cliente.Nome}",
                    Link = $"/OrdensServico/Detalhes/{ordem.Id}",
                    Icone = "bi-calendar-check",
                    Cor = "info",
                    DataReferencia = ordem.PrevisaoEntrega
                });
            }
        }

        // Ordenar por prioridade (danger > warning > info)
        var prioridadeCor = new Dictionary<string, int>
        {
            ["danger"] = 0,
            ["warning"] = 1,
            ["info"] = 2
        };
        notificacoes = notificacoes
            .OrderBy(n => prioridadeCor.GetValueOrDefault(n.Cor, 3))
            .ThenBy(n => n.DataReferencia)
            .ToList();

        return new NotificacaoResumoDto
        {
            Total = notificacoes.Count,
            EstoqueBaixo = notificacoes.Count(n => n.Tipo == "EstoqueBaixo"),
            GarantiaVencendo = notificacoes.Count(n => n.Tipo is "GarantiaVencendo" or "GarantiaVencida"),
            OrdensPendentes = notificacoes.Count(n => n.Tipo is "EntregaAtrasada" or "EntregaHoje"),
            Notificacoes = notificacoes
        };
    }
}
