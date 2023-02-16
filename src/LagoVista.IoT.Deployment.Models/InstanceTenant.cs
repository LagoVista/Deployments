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
