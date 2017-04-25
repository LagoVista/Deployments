using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Validation
{
    public class InstanceValidator : IInstanceValidator
    {
        public ValidationResult ValidateInstance(DeploymentInstance intance)
        {
            var result = new ValidationResult();
            
            /* TODO Need to do a full sweep of the instance, make sure everything is populated that needs to be */



            return result;
        }
    }
}
