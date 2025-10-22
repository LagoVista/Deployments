// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 02f7489f33fd64623b1554f1bd2bc87f9d159fa202ca82670dc88653869f5287
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System;
using System.Security.Cryptography;

namespace LagoVista.IoT.Deployment.Models
{
    public class InstanceAccount : IValidateable
    {
        public string InstanceId { get; set; }
        public string UserName { get; set; }
        public string AccessKey1 { get; set; }
        public string AccessKeyHash1 { get; set; }
        public string AccessKey2 { get; set; }
        public string AccessKeyHash2 { get; set; }

        public InstanceAccountDTO CreateDTO(string instanceId)
        {       
            var salt = System.Text.ASCIIEncoding.ASCII.GetBytes(UserName + instanceId);

            if (!String.IsNullOrEmpty(AccessKey1))
            {
                var bytes = new Rfc2898DeriveBytes(AccessKey1, salt);
                var buffer = bytes.GetBytes(24);
                AccessKeyHash1 = Convert.ToBase64String(buffer);
                AccessKey1 = String.Empty;
            }

            if (!String.IsNullOrEmpty(AccessKey2))
            {
                var bytes = new Rfc2898DeriveBytes(AccessKey2, salt);
                var buffer = bytes.GetBytes(24);
                AccessKeyHash2 = Convert.ToBase64String(buffer);
                AccessKey2 = String.Empty;
            }

            var dto = new InstanceAccountDTO()
            {
               
                PartitionKey = instanceId,
                RowKey = UserName,
                AccessKeyHash1 = AccessKeyHash1,
                AccessKeyHash2 = AccessKeyHash2
            };

            return dto;
        }
    }

    public class InstanceAccountDTO : TableStorageEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AccessKeyHash1 { get; set; }
        public string AccessKeyHash2 { get; set; }

        public InstanceAccount ToInstanceAccount()
        {
            return new InstanceAccount()
            {
                UserName = RowKey,
                InstanceId = PartitionKey,
                AccessKeyHash1 = AccessKeyHash1,
                AccessKeyHash2 = AccessKeyHash2
            };
        }
    }
}
