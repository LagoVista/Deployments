using LagoVista.IoT.Deployment.Admin.Models;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public static class RequestSigningService
    {     
        public static string GetAuthHeaderValue(DeploymentHost host, String instanceId, string requestId, string orgId, string userId, string method, String resource, String date)
        {
            var canonicalizedString = $"{method}\n";
            canonicalizedString += $"{requestId}\n";
            canonicalizedString += $"{instanceId}\n";
            canonicalizedString += $"{orgId}\n";
            canonicalizedString += $"{userId}\n";
            canonicalizedString += $"{date}\n";
            canonicalizedString += resource;

            var encData = Encoding.UTF8.GetBytes(canonicalizedString);

            var hmac = new HMac(new Sha256Digest());
            hmac.Init(new KeyParameter(Encoding.UTF8.GetBytes(host.HostAccessKey1)));

            var result = new byte[hmac.GetMacSize()];
            hmac.BlockUpdate(encData, 0, encData.Length);
            hmac.DoFinal(result, 0);

            var md5Sting = new StringBuilder(result.Length);
            for (var i = 0; i < result.Length; i++)
            {
                md5Sting.Append(result[i].ToString("X2"));
            }

            return $"{instanceId}:{md5Sting.ToString()}";
        }

    }
}
