using desafioPonta.Core.Common.Helper;
using desafioPonta.Core.Common.Interfaces;

namespace desafioPonta.Core.Common.Validations;

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
