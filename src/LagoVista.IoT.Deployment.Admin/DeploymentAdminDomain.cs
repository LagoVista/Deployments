using LagoVista.Core.Attributes;
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin
{
    public class DeploymentAdminDomain
    {
        public const string DeploymentAdmin = "Deployment Admin";

        [DomainDescription(DeploymentAdmin)]
        public static DomainDescription DeploymentAdminDescription
        {
            get
            {
                return new DomainDescription()
                {
                    Description = "A set of classes that contains meta data for managing IoT Deployments.",
                    DomainType = DomainDescription.DomainTypes.BusinessObject,
                    Name = "Deployment Admin",
                    CurrentVersion = new Core.Models.VersionInfo()
                    {
                        Major = 0,
                        Minor = 8,
                        Build = 001,
                        DateStamp = new DateTime(2016, 12, 20),
                        Revision = 1,
                        ReleaseNotes = "Initial unstable preview"
                    }
                };
            }
        }
    }
}
