using BNB.ProjetoReferencia.Core.Common.Helper;
using System.Text.Json.Serialization;

namespace BNB.ProjetoReferencia.Core.Domain.Cobranca.Entities;

public class CobrancaEntity : Entity
{    
    public Calendario Calendario { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public int Revisao { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Loc Loc { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Location { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Status { get; set; }
    public Devedor Devedor { get; set; }
    public Valor Valor { get; set; }    
    public string Chave { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string TxId { get; set; }
    public string SolicitacaoPagador { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<InfoAdicional> InfoAdicionais { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string PixCopiaECola { get; set; }
}

public class Calendario
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public DateTime Criacao { get; set; }
    public int Expiracao { get; set; }
}

public class Loc
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public int Id { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Location { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string TipoCob { get; set; }
}

public class Devedor
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Cpf { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
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
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Nome { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Valor { get; set; }
}
