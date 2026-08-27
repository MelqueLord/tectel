namespace AssistenciaTecnica.Application.DTOs;

public class NotificacaoDto
{
    public string Tipo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string Icone { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    public DateTime? DataReferencia { get; set; }
}

public class NotificacaoResumoDto
{
    public int Total { get; set; }
    public int EstoqueBaixo { get; set; }
    public int GarantiaVencendo { get; set; }
    public int OrdensPendentes { get; set; }
    public List<NotificacaoDto> Notificacoes { get; set; } = [];
}
