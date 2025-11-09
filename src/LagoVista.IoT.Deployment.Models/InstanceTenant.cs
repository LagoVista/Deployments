// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 3954f0b67ee0e8e76c38fb759cca7e71eb976f84d7552bc139e73f9fe143f502
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Models
{
    public class InstanceTenant
    {
        public EntityHeader Instance { get; set; }
        public EntityHeader Organization { get; set; }

        public string PasswordSalt { get; set; }
        public string PasswordEncryptionKey { get; set; }

        public List<TenantAccount> Accounts { get; set; }

        public string TenantKey
        {
            get { return $"{Organization.Key}_{Instance.Key}"; }
        }
    }

    public class TenantAccount
    {
        public string UserName { get; set; }
        public string HashedPasswords { get; set; }
    }
}
