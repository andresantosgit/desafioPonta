namespace BNB.ProjetoReferencia.Models;

/// <summary>
/// Modelo de carteira
/// </summary>
public class PixModel
{
    public string endToEndId { get; set; }
    public string txid { get; set; }
    public string valor { get; set; }
    public DateTime horario { get; set; }
    public string infoPagador { get; set; }
    public DevolucoesModel? devolucoes { get; set; }

}
public class DevolucoesModel
{
    public string id { get; set; }
    public string rtrId { get; set; }
    public string valor { get; set; }
    public HorarioModel horario { get; set; }
    public string status { get; set; }
}

public class HorarioModel
{
    public DateTime solicitacao { get; set; }
}