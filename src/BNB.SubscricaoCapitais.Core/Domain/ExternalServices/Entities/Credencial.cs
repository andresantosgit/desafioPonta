//-------------------------------------------------------------------------------------			
// <copyright file="Credencial.cs" company="BNB">
//    Copyright statement. All right reserved
// </copyright>	
// <summary>
//   Descrição do arquivo
// </summary>		
//-------------------------------------------------------------------------------------

using BNB.ProjetoReferencia.Core.Domain.ExternalServices.Interfaces;
using System.Collections.Generic;

namespace BNB.ProjetoReferencia.Core.Domain.ExternalServices.Entities
{
    /// <summary>
    /// Entidade Credential
    /// </summary>
    public class Credencial : ICredencial
    {
        /// <summary>
        /// Matricula colaborador 
        /// </summary>
        /// <value>Matricula</value>
        public string Matricula { get; set; } = "";

        /// <summary>
        /// Nome colaborador 
        /// </summary>
        /// <value>Nome</value>
        public string? Nome { get; set; }

        /// <summary>
        /// Email colaborador 
        /// </summary>
        /// <value>Email</value>
        public string? Email { get; set; }

        /// <summary>
        /// Possui permissão para o menu registrar
        /// </summary>
        /// <value>PossuiPermissaoMenuRegistrar</value>
        public bool? PossuiPermissaoMenuRegistrar { get; set; }

        /// <summary>
        /// Possui permissão para o menu consultar
        /// </summary>
        /// <value>PossuiPermissaoMenuConsultar</value>
        public bool? PossuiPermissaoMenuConsultar { get; set; }

    }
}