// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: ec9075608f51ed84a64fe3b297e20627df0b9d883c529bc1c238ea7641729c76
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Validation
{
    public interface IInstanceValidator
    {
        ValidationResult ValidateInstance(DeploymentInstance intance);
    }
}
