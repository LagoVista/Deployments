using LagoVista.IoT.Deployment.Models.Runtime;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace LagoVista.IoT.Deployment.Admin.Rest.Services
{
    internal class RuntimeAccessLeaseTokenIssuer
    {
        private readonly IConfiguration _configuration;

        public RuntimeAccessLeaseTokenIssuer(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public RuntimeAccessLease Create(string orgId, string instanceId, string hostId, IEnumerable<string> capabilities)
        {
            var endpoint = _configuration["RuntimeDataService:Endpoint"];
            if (String.IsNullOrWhiteSpace(endpoint))
                throw new InvalidOperationException("RuntimeDataService:Endpoint is not configured.");

            var signingKeyBase64 = _configuration["RuntimeDataService:SigningKey"];
            if (String.IsNullOrWhiteSpace(signingKeyBase64))
                throw new InvalidOperationException("RuntimeDataService:SigningKey is not configured.");

            byte[] signingKey;
            try
            {
                signingKey = Convert.FromBase64String(signingKeyBase64);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("RuntimeDataService:SigningKey must be a base64 encoded secret.", ex);
            }

            if (signingKey.Length < 32)
                throw new InvalidOperationException("RuntimeDataService:SigningKey must contain at least 256 bits of key material.");

            var leaseMinutes = 60;
            if (Int32.TryParse(_configuration["RuntimeDataService:LeaseMinutes"], out var configuredLeaseMinutes))
                leaseMinutes = configuredLeaseMinutes;

            if (leaseMinutes < 5 || leaseMinutes > 1440)
                throw new InvalidOperationException("RuntimeDataService:LeaseMinutes must be between 5 and 1440 minutes.");

            var issuedUtc = DateTime.UtcNow;
            var validThroughUtc = issuedUtc.AddMinutes(leaseMinutes);
            var grantedCapabilities = new List<string>(capabilities ?? RuntimeDataCapabilities.All);

            var payload = new
            {
                v = 1,
                jti = Guid.NewGuid().ToString("N"),
                org = orgId,
                instance = instanceId,
                host = hostId,
                iat = new DateTimeOffset(issuedUtc).ToUnixTimeSeconds(),
                exp = new DateTimeOffset(validThroughUtc).ToUnixTimeSeconds(),
                cap = grantedCapabilities
            };

            var payloadJson = JsonSerializer.Serialize(payload);
            var payloadSegment = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));
            var signedContent = $"v1.{payloadSegment}";

            byte[] signature;
            using (var hmac = new HMACSHA256(signingKey))
                signature = hmac.ComputeHash(Encoding.UTF8.GetBytes(signedContent));

            return new RuntimeAccessLease
            {
                Token = $"{signedContent}.{Base64UrlEncode(signature)}",
                Endpoint = endpoint.TrimEnd('/'),
                IssuedUtc = issuedUtc,
                ValidThroughUtc = validThroughUtc,
                Capabilities = grantedCapabilities
            };
        }

        private static string Base64UrlEncode(byte[] value)
        {
            return Convert.ToBase64String(value)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
