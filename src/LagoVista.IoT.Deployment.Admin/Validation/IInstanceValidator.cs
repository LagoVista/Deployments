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
