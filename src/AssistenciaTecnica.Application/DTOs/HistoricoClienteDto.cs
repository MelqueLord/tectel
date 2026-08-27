namespace AssistenciaTecnica.Application.DTOs;

public class HistoricoClienteDto
{
    public ClienteDto Cliente { get; set; } = null!;
    public List<AparelhoDto> Aparelhos { get; set; } = [];
    public List<OrdemServicoListagemDto> OrdensServico { get; set; } = [];
    public decimal TotalGasto { get; set; }
    public int TotalOrdens { get; set; }
    public int OrdensAbertas { get; set; }
    public int OrdensConcluidas { get; set; }
}
