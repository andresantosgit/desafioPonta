//-------------------------------------------------------------------------------------			
// <copyright file="MensagemViewModel.cs" company="BNB">
//    Copyright statement. All right reserved
// </copyright>	
// <summary>
//   Descrição do arquivo
// </summary>		
//-------------------------------------------------------------------------------------

using BNB.ProjetoReferencia.Util.Mensagens;

namespace BNB.ProjetoReferencia.WebUI.ViewModel.Util.Mensagens
{
    /// <summary>
    /// Mensagem ViewModel
    /// </summary>
    public class MensagemViewModel
    {
        /// <summary>
        /// Titulo mensagem
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        public string Titulo { get; set; }

        /// <summary>
        /// Mensagem view model
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        public string Mensagem { get; set; }

        /// <summary>
        /// Tipo mensagem
        /// </summary>
        /// <value>O parâmetro não é usado</value>
        public TipoMensagem TipoMensagem { get; set; }
    }
}