//-------------------------------------------------------------------------------------			
// <copyright file="BaseController.cs" company="BNB">
//    Copyright statement. All right reserved
// </copyright>	
// <summary>
//   Descrição do arquivo
// </summary>		
//-------------------------------------------------------------------------------------

using AutoMapper;
using BNB.ProjetoReferencia.Util.Mensagens;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using BNB.ProjetoReferencia.Core.Domain.ExternalServices.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.ExternalServices.Entities;
using BNB.ProjetoReferencia.WebUI.Filters;
using BNB.ProjetoReferencia.WebUI.ViewModel.Util.Mensagens;

namespace BNB.ProjetoReferencia.WebUI
{
    /// <summary>
    /// Controller base
    /// </summary>
    [ServiceFilter(typeof(SegurancaAutorizaActionFilter))]
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Logger Service
        /// </summary>
        private ILogger<BaseController> _logger;

        /// <summary>
        /// Auto Mapper Service
        /// </summary>
        private IMapper _mapper;

        /// <summary>
        /// Autorizador Service
        /// </summary>
        private IAuthService _autorizadorService;

        protected ILogger<BaseController> Logger => _logger ?? (_logger = HttpContext.RequestServices?.GetService<ILogger<BaseController>>());
        
        protected IMapper Mapper => _mapper ?? (_mapper = HttpContext?.RequestServices?.GetService<IMapper>());
        
        protected IAuthService AutorizadorService => _autorizadorService ?? (_autorizadorService = HttpContext.RequestServices?.GetService<IAuthService>());
        
        protected ISession Session => HttpContext.Session;

        /// <summary>
        /// Colaborador logado.
        /// </summary>
        /// <value>Objeto ColaboradorBNBViewModel.</value>
        protected ICredencial Colaborador
        {
            get
            {
                return new Credencial
                {
                    Matricula = AutorizadorService.Matricula,
                    Nome = AutorizadorService.NomeCompleto
                };
            }
        }

        /// <summary>
        /// Creates a System.Web.Mvc.JsonResult object that serializes the specified object
        ///     to JavaScript Object Notation (JSON).
        /// </summary>
        /// <param name="data">The JavaScript object graph to serialize.</param>
        /// <returns>
        /// The JSON result object that serializes the specified object to JSON format. The
        /// result object that is prepared by this method is written to the response by the
        /// ASP.NET MVC framework when the object is executed.
        /// </returns>
        protected JsonResult Json(object data)
        {
            //return Ok(data);

            return new JsonResult(data, new JsonSerializerOptions());

            //return new JsonResult( data,
            //    new JsonSerializerSettings());

            //return new JsonResult()
            //{
            //    Data = data,
            //    MaxJsonLength = int.MaxValue
            //};
        }

        /// <summary>
        /// Creates a System.Web.Mvc.JsonResult object that serializes the specified object
        ///     to JavaScript Object Notation (JSON).
        /// </summary>
        /// <param name="data">The JavaScript object graph to serialize.</param>
        /// <returns>
        /// The JSON result object that serializes the specified object to JSON format. The
        /// result object that is prepared by this method is written to the response by the
        /// ASP.NET MVC framework when the object is executed.
        /// </returns>
        protected JsonResult JsonDeny(object data)
        {
            //return new JsonResult(data);

            //return new JsonResult(data);
            return new JsonResult(data, new JsonSerializerOptions());

            //return new JsonResult()
            //{
            //    Data = data,
            //    MaxJsonLength = int.MaxValue
            //};
        }

        /// <summary>
        /// Credencial do Usuário Logado
        /// </summary>
        /// <value> A propriedade Credencial recupera a credencial do usuário logado.</value>
        public Credencial Credencial => new Credencial()
        {
            Matricula = AutorizadorService.Matricula,
            Nome = AutorizadorService.NomeCompleto
        };

        /// <summary>
        /// Copia os dados entre objetos.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto.</typeparam>
        /// <param name="target">Objeto destino.</param>
        /// <param name="source">Objeto origem.</param>
        public void CopyValues<T>(T target, T source)
        {
            Type t = typeof(T);
            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source, null);

                if (value != null)
                {
                    prop.SetValue(target, value, null);
                }
            }
        }

        /// <summary>
        /// Mapear mudança das entidades.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto.</typeparam>
        /// <param name="originalEntity">Objeto original.</param>
        /// <param name="model">Objeto destino.</param>
        /// <returns>Entidade mapeada.</returns>
        public T MapChangesEntity<T>(T originalEntity, object model)
        {
            T defaultEntity = default(T);
            this.CopyValues<T>(defaultEntity, originalEntity);
            T mapped = Mapper.Map(model, originalEntity);
            return mapped;
        }

        /// <summary>
        /// Retorna uma mensagem de exibição para a interface.
        /// </summary>
        /// <param name="aobjFormCollection">Dados da mensagem.</param>
        /// <returns>Partial view contendo a mensagem.</returns>
        [HttpPost]
        public ActionResult exibirMensagem(IFormCollection aobjFormCollection)
        {
            if (aobjFormCollection != null)
            {
                int lintTipoMensagem = Convert.ToInt32(aobjFormCollection["TipoMensagem"]);
                string lstrTituloMensagem = aobjFormCollection["TituloMensagem"]; // "Dados obrigatórios não informados ou preenchidos incorretamente"
                string lstrTextoMensagem = aobjFormCollection["TextoMensagem"] == "SyntaxError: Unexpected token < in JSON at position 4" ? string.Empty : aobjFormCollection["TextoMensagem"];

                if (lintTipoMensagem > 0 && !string.IsNullOrEmpty(lstrTituloMensagem) && !string.IsNullOrEmpty(lstrTextoMensagem))
                {
                    return this.PartialView("_Mensagens", new MensagemViewModel { TipoMensagem = (TipoMensagem)lintTipoMensagem, Titulo = lstrTituloMensagem, Mensagem = lstrTextoMensagem });
                }
            }

            return null;
        }
    }
}
