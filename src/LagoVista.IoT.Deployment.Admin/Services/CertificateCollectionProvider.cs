using LagoVista.Core.Validation;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace LagoVista.IoT.Deployment.Admin.Services
{
    internal class CertificateCollectionProvider
    {
        private const string PrefsKey = "com.atakmap.app_preferences";
        private const string CertLocationKey = "certificateLocation";
        private const string CertPasswordKey = "clientPassword";

        public InvokeResult<X509CertificateCollection> GetCollection(Preferences manifest, ZipArchive package)
        {
            var pref = manifest.Preference.First(p => p.Name == PrefsKey);
            var certFileName = Path.GetFileName(pref.Entry.First(e => e.Key == CertLocationKey).Text);

            using var certStream = new MemoryStream();
            var certEntry = package.Entries.First(e => e.Name.Contains(certFileName));
            certEntry.Open().CopyToAsync(certStream);

            var cert = new X509Certificate(certStream.ToArray(), pref.Entry.FirstOrDefault(e => e.Key == CertPasswordKey)?.Text);


            return InvokeResult<X509CertificateCollection>.Create(new X509CertificateCollection() { cert });
        }
    }
}
