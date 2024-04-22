//-------------------------------------------------------------------------------------			
// <copyright file="Credencial.cs" company="BNB">
//    Copyright statement. All right reserved
// </copyright>	
// <summary>
//   Descrição do arquivo
// </summary>		
//-------------------------------------------------------------------------------------


namespace BNB.ProjetoReferencia.Core.Domain.ExternalServices.Interfaces
{
    public interface ICredencial
    {
        string Matricula { get; set; }
        string? Nome { get; set; }
        string? Email { get; set; }
    }
}