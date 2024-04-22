using BNB.ProjetoReferencia.Core.Common.Helper;
using BNB.ProjetoReferencia.Core.Common.Interfaces;

namespace BNB.ProjetoReferencia.Core.Common.Validations;

public class AuditoriaRules :
    IRules<Auditoria>
{
    public async Task<Rules> FactoryAsync(Auditoria model, CancellationToken ctx)
    {
        if (model == null)
            throw new ArgumentException(string.Format("Modelo é obrigatorio", nameof(Auditoria)), nameof(model));

        var rules = Rules.Create()
            .NotEmpty(nameof(model.CriadoPor), model.CriadoPor, "CriadoPor é obrigatorio")
            .NotEmpty(nameof(model.Origem), model.Origem, "Origem é Obrigatorio");

        return rules;
    }
}
