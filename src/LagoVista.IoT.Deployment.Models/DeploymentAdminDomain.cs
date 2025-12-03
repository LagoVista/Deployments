// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: df2938dff69674fc42e3d93b59216a9fd6b8fc7677cdc47e825e1694f172f0fe
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin
{
    [DomainDescriptor]
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
                    Description = "Metadata and tools for configuring and managing IoT deployments, runtime hosts, solutions, integrations, and related resources.",
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