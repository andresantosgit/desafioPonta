using BNB.ProjetoReferencia.Core.Common.Helper;

namespace BNB.ProjetoReferencia.Core.Domain.Cobranca.Entities;

public class CobrancaEntity : Entity<string>
{
    public Calendario Calendario { get; set; }
    public int Revisao { get; set; }
    public Loc Loc { get; set; }
    public string Location { get; set; }
    public string Status { get; set; }
    public Devedor Devedor { get; set; }
    public Valor Valor { get; set; }
    public string Chave { get; set; }
    public string SolicitacaoPagador { get; set; }
    public List<InfoAdicional> InfoAdicionais { get; set; }
}

public class Calendario
{
    public DateTime Criacao { get; set; }
    public int Expiracao { get; set; }
}

public class Loc
{
    public int Id { get; set; }
    public string Location { get; set; }
    public string TipoCob { get; set; }
}

public class Devedor
{
    public string Cnpj { get; set; }
    public string Nome { get; set; }
}

public class Valor
{
    public string Original { get; set; }
    public int ModalidadeAlteracao { get; set; }
}

public class InfoAdicional
{
    public string Nome { get; set; }
    public string Valor { get; set; }
}
