using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Admin.Rest.Controllers
{
	public class HostRuntimeController
	{


		[HttpGet("/api/deployment/host/full")]
		public Task<InvokeResult<DeploymentHost>> GetDeploymentHost()
		{
			throw new NotImplementedException();
		}

	}
}
