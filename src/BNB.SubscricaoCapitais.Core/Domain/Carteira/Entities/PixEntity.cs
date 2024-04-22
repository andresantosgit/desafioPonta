using BNB.ProjetoReferencia.Core.Common.Helper;

namespace BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;


public class PixEntity
{
    public string endToEndId { get; set; }
    public string txid { get; set; }
    public string valor { get; set; }
    public DateTime horario { get; set; }
    public string infoPagador { get; set; }
    public Devolucoes? devolucoes { get; set; }

}
public class Devolucoes
{
    public string id { get; set; }
    public string rtrId { get; set; }
    public string valor { get; set; }
    public Horario horario { get; set; }
    public string status { get; set; }
}

public class Horario
{
    public DateTime solicitacao { get; set; }
}