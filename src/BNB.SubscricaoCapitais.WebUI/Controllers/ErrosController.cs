//-------------------------------------------------------------------------------------			
// <copyright file="ErrosController.cs" company="BNB">
//    Copyright statement. All right reserved
// </copyright>	
// <summary>
//   Descrição do arquivo
// </summary>		
//-------------------------------------------------------------------------------------

using BNB.ProjetoReferencia.WebUI;
using BNB.ProjetoReferencia.WebUI.Helpers.Erros;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BNB.ProjetoReferencia.WebUI.Controllers
{
    /// <summary>
    /// Controller erros
    /// </summary>
    public class ErrosController : BaseController
    {
        /// <summary>
        /// Erros POST
        /// </summary>
        /// <param name="erros">Name = "erros"</param>
        /// <returns>Retorna view</returns>
        [HttpPost]
        public ActionResult MostrarErrosValidacao([FromBody]ListaErrosValidacao? erros)
        {
            IList<ErroValidacao> listaErros = new List<ErroValidacao>();

            if (erros == null || erros.Erros == null)
            {
                return Ok();
            }

            foreach (var item in erros.Erros)
            {
                listaErros.Add(item);
            }            

            return this.PartialView("_ErrosDeValidacao", listaErros);
        }

    }
}