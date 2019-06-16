using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Interfaces
{
    public interface IServiceTicketCreator
    {
        Task<InvokeResult<string>> CreateServiceTicketAsync(string templateId, string repoId, string deviceId);
    }
}
