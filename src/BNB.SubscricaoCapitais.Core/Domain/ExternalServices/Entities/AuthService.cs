
using bnbauth=BNB.S095.BNBAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using BNB.ProjetoReferencia.Core.Domain.ExternalServices.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Util;

namespace BNB.ProjetoReferencia.Core.Domain.ExternalServices.Entities
{
    /// <summary>
    /// Classe de implementação do serviço de autorização
    /// </summary>
    public class AuthService : IAuthService
    {
        /// <summary>
        /// Interface de usuário BNB Auth
        /// </summary>
        private readonly bnbauth.IUsuario _usuarioLogado;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="autorizadorWebService">Implementação do Web service do Autorizador</param>
        /// <param name="colaboradorBNBService">Implementação do serviço de colaborador BNB</param>
        /// <param name="contextAccessor">Implementação do serviço de contexto HTTP</param>
        /// <param name="options">Implementação do serviço de configurações</param>
        public AuthService(IHttpContextAccessor contextAccessor)
        {
            if (contextAccessor == null || contextAccessor.HttpContext == null) 
                throw new ArgumentNullException(nameof(contextAccessor));

            _usuarioLogado = bnbauth.ContextoSeguranca.GetUsuarioLogado(contextAccessor?.HttpContext?.Request);
        }

        public string Matricula => _usuarioLogado.Username;

        public string NomeCompleto => _usuarioLogado.NomeCompleto;

        public string NomeCurto => _usuarioLogado.NomeCurto;

        public string Email
        {
            get
            {
                try
                {
                    return _usuarioLogado.Email;
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public IDictionary<string, object> ValorClaim => _usuarioLogado.ValorClaim;

        public bool IsAplicacao => _usuarioLogado.IsAplicacao;

        public bool IsUsuarioInternet => _usuarioLogado.IsUsuarioInternet;

        public string GetToken()
        {
            return _usuarioLogado.GetCredencial().Token;
        }

        public bool HasPermission(string recurso, string? acao)
        {
            return true;
            //return _usuarioLogado.HasPermission(recurso, acao);// .usuarioPossuiLimitanteAsync(this.Matricula, Constantes.CodigoSistema, recurso, acao ?? string.Empty);            
        }

        public bool HasPermission(string recurso)
        {
            return true;
            //return _usuarioLogado.HasPermission(Constantes.CodigoSistema, recurso);
        }

        public void Logout(HttpRequest request)
        {
            _usuarioLogado.Logout(request);
        }

        public bool PossuiLimitanteGlobal(string identificadorLimitante)
        {
            return true;
            //return _usuarioLogado.HasPermission(Constantes.CodigoSistema, identificadorLimitante);
        }

        public ICredencial GetCredencial()
        {
            return new Credencial
            {
                Matricula = this.Matricula,
                Nome = this.NomeCompleto,
                Email = this.Email
            };
        }

    }
}
