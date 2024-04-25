using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BNB.ProjetoReferencia.Core.Domain.ExternalServices.Interfaces
{
    public interface ITokenService
    {
        Task<string> GetToken();
        
    }
}
