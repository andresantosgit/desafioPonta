//-------------------------------------------------------------------------------------			
// <copyright file="ErroValidacao.cs" company="BNB">
//    Copyright statement. All right reserved
// </copyright>	
// <summary>
//   Descrição do arquivo
// </summary>		
//-------------------------------------------------------------------------------------

namespace BNB.ProjetoReferencia.WebUI.Helpers.Erros
{
    /// <summary>
    /// Erro Validacao
    /// </summary>
    public class ErroValidacao
    {
        /// <summary>
        /// Erros validação
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        public IList<string>? Erros { get; set; }

        /// <summary>
        /// Propriedade validação
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        public string? Propriedade { get; set; }

        /// <summary>
        /// Propriedade tabr
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        public int? Tab { get; set; }
    }
}