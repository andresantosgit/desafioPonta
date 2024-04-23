using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BNB.ProjetoReferencia.Core.Domain.ExternalServices.Interfaces
{
    public interface IAuthService
    {
        string Matricula { get; }

        string NomeCompleto { get; }

        string NomeCurto { get; }

        string Email { get; }

        IDictionary<string, object> ValorClaim { get; }

        bool IsAplicacao { get; }

        bool IsUsuarioInternet { get; }

        string GetToken();

        bool HasPermission(string recurso, string? acao);

        bool HasPermission(string recurso);

        void Logout(Microsoft.AspNetCore.Http.HttpRequest request);

        bool PossuiLimitanteGlobal(string identificadorLimitante);

        ICredencial GetCredencial();

    }
}
