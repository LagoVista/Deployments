// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 5ff9e24d6827cb09527a0cc8d509134ff40bf98c490406ca290427c11b163404
// IndexVersion: 0
// --- END CODE INDEX META ---
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
