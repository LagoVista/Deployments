// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: dc1ce4bb818f38006914f76d034f048508c47cc13a9b86b09575b5d5972c1a9f
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.IoT.Deployment.Admin.Models;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    public static class RequestSigningService
    {
        public static string GetAuthHeaderValue(DeploymentHost host, string requestId, string orgId, string userId, string method, String resource, String date)
        {
            if (host == null) throw new InvalidOperationException("Missing Host."); 
            if (String.IsNullOrEmpty(host.HostAccessKey1)) throw new InvalidOperationException("Missing Host Access Key.");
            
            if (String.IsNullOrEmpty(method)) return "INVALID - MISSING HTTP METHOD";
            if (String.IsNullOrEmpty(requestId)) return "INVALID - MISSING REQUEST ID";
            if (String.IsNullOrEmpty(orgId)) return "INVALID - MISSGING ORG ID";
            if (String.IsNullOrEmpty(userId)) return "INVALID - MISSING USER ID";
            if (String.IsNullOrEmpty(date)) return "INVALID - MISSING DATE STAMP";

            var canonicalizedString = $"{method}\n";
            canonicalizedString += $"{requestId}\n";
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

            var headerValue = $"{requestId}:{md5Sting.ToString()}";
            return headerValue;
        }
    }
}
