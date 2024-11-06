using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.IO.Compression;
using LagoVista.IoT.DeviceMessaging.Models.Cot;
using LagoVista.Core.Validation;
using LagoVista.IoT.Logging.Loggers;
using LagoVista.Core;

namespace LagoVista.IoT.Deployment.Admin.Services.NotificationClients
{
    public class TakClient : ITakClient
    {
        private const string CoTStreamsKey = "cot_streams";
        private const string ConnectionStringKey = "connectString0";
        
        private readonly TcpClient _client = new TcpClient();
        private SslStream _sslStream;

        private Thread _eventListenerThread;
        private readonly IAdminLogger _adminLogger;

        private CancellationToken _cancellationToken;

        private bool _running = false;

        private readonly CertificateCollectionProvider _certCollectionProvider;       

        X509Certificate _customRootCert;
        X509CertificateCollection _certCollection;
        Preferences _preferences;
        Manifest _manifest;

        private string _host;
        private int _port;
        private bool _ssl;

        private readonly XmlReaderSettings fragmentXmlReaderSettings = new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment };

        public event EventHandler TakServerConnected;
        public event EventHandler TakServerDisconnected;
        public event EventHandler<Event> MessageReceived;

        public bool IgnoreCertificateErrors { get; set; }

        public TakClient(IAdminLogger adminLogger)
        {
            _certCollectionProvider = new CertificateCollectionProvider();
            _adminLogger = adminLogger ?? throw new ArgumentNullException(nameof(adminLogger));
        }

        public Task<InvokeResult> InitAsync(Stream stream)
        {
            using (var package = new ZipArchive(stream))
            {
                var manifestResult = GetManifest(package);
                if (!manifestResult.Successful)
                    return Task.FromResult(manifestResult.ToInvokeResult());

                _manifest = manifestResult.Result;

                var preferenceResult = GetPrefernces(package, _manifest);
                if (!preferenceResult.Successful)
                    return Task.FromResult(preferenceResult.ToInvokeResult());

                var certResult = _certCollectionProvider.GetCollection(preferenceResult.Result, package);
                if (!certResult.Successful)
                    return Task.FromResult(certResult.ToInvokeResult());

                _certCollection = certResult.Result;

                return Task.FromResult(InvokeResult.Success);
            }
        }

        public async Task<InvokeResult> ConnectAsync()
        {
            _adminLogger.Trace($"[TakClient__ConnectAsync] Connecting to: {_host} on port: {_port}");

            try
            {
                await _client.ConnectAsync(_host, _port);
                _sslStream = new SslStream(_client.GetStream(), false, new RemoteCertificateValidationCallback(CertificateValidationCallback));

                await _sslStream.AuthenticateAsClientAsync(_host, _certCollection, false);

                TakServerConnected?.Invoke(this, null);

                return InvokeResult.Success;
            }
            catch (Exception ex)
            {
                _adminLogger.AddException("[TakClient__ConnectAsync]", ex, _host.ToKVP("host"));
                return InvokeResult.FromException("[TakClient__ConnectAsync]", ex);
            }
        }

        public Task<InvokeResult> ConnectAsync(byte[] customCert)
        {
            _customRootCert = new X509Certificate(customCert);
            return ConnectAsync();
        }

        public Task<InvokeResult> GetIsConnectedAsync()
        {
            if (_client == null)
                return Task.FromResult(InvokeResult.FromError("Client is null, could not send."));

            if (!_client.Connected)
                return Task.FromResult(InvokeResult.FromError("Client is not connected, could not send."));

            if (_sslStream == null)
                return Task.FromResult(InvokeResult.FromError("SSL Stream is null could not send."));


            return Task.FromResult(InvokeResult.Success);
        }

        public Task<InvokeResult> DisconnectAsync()
        {
            return Task.FromResult(InvokeResult.Success);
        }

        private async void ListenerThread()
        {
            while (_client.Connected && _running)
            {
                var buffer = new byte[_client.ReceiveBufferSize];
                var bytesRead = await _sslStream!.ReadAsync(buffer, _cancellationToken);
                if (bytesRead > 0)
                {
                    Console.WriteLine($"{DateTime.Now} [TakClient__ListenAsync] Read message with buffer: {bytesRead}");
                    Console.WriteLine($"==========================================================");
                    Console.WriteLine($"{DateTime.Now} MESSAGE START");
                    Console.WriteLine(Encoding.UTF8.GetString(buffer));
                    foreach (Match match in Regex.Matches(Encoding.UTF8.GetString(buffer), @"<event(.|\s)*\/event>").Cast<Match>())
                    {
                        try
                        {
                            using var memStream = new MemoryStream(Encoding.UTF8.GetBytes(match.Value));
                            using var reader = XmlReader.Create(memStream, fragmentXmlReaderSettings);
                            while (reader.Read() && !reader.EOF)
                            {
                                if (reader.NodeType == XmlNodeType.Element && reader.Name == "event")
                                    MessageReceived?.Invoke(this, Event.Parse(reader.ReadOuterXml()));
                            }
                        }
                        catch (Exception)
                        {
                            //TODO: ILogger
                        }
                    }

                    Console.WriteLine($"{DateTime.Now} MESSAGE END");
                    Console.WriteLine($"==========================================================");
                }
            }
        }

        /// <summary>
        /// Start the connection via SSL stream to the TAK Server and respond to received CoT events
        /// </summary>
        /// <returns></returns>
        public async Task<InvokeResult> ListenAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;

            var connectResult = await ConnectAsync();
            if (connectResult.Successful)
            {
                if (_client.Connected)
                    Console.WriteLine($"[TakClient__ListenAsync] Connected;");
                else
                {
                    Console.WriteLine($"[TakClient__ListenAsync] Did not connect;");
                    return InvokeResult.FromError($"Could not connect to TAK server ");
                }

                _running = true;

                _eventListenerThread = new Thread(ListenerThread);
                _eventListenerThread.Start();

                return InvokeResult.Success;
            }
            else
                return connectResult;
        }

        /// <summary>
        /// Read CoT Events from server in buffer
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Event>> ReadEventsAsync(CancellationToken cancellationToken = default)
        {
            var events = new List<Event>();

            if (!_client.Connected)
                return events;

            var buffer = new byte[_client.ReceiveBufferSize];
            await _sslStream!.ReadAsync(buffer, cancellationToken);

            foreach (Match match in Regex.Matches(Encoding.UTF8.GetString(buffer), @"<event(.|\s)*\/event>").Cast<Match>())
            {
                try
                {
                    using var memStream = new MemoryStream(Encoding.UTF8.GetBytes(match.Value));
                    using var reader = XmlReader.Create(memStream, fragmentXmlReaderSettings);
                    while (reader.Read() && !reader.EOF)
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "event")
                            events.Add(Event.Parse(reader.ReadOuterXml()));
                    }
                }
                catch (Exception)
                {
                    //TODO: ILogger
                }
            }
            return events;
        }

        public async Task<InvokeResult> SendAsync(Message message, CancellationToken cancellationToken = default)
        {
            var xml = message.ToXmlString();
            return await SendAsync(xml, cancellationToken);
        }

        public async Task<InvokeResult> SendAsync(string message, CancellationToken cancellationToken = default)
        {
            var canSend = await GetIsConnectedAsync();

            if (!canSend.Successful)
            {
                return canSend;
            }


            var buffer = Encoding.ASCII.GetBytes(message);
            Console.WriteLine($"[TakClient__SendAsync_RawXML] Message Content:\n{message}");

            try
            {
                await _sslStream.WriteAsync(buffer, cancellationToken);

                return InvokeResult.Success;
            }
            catch (Exception ex)
            {
                _adminLogger.AddException("[TAKClient__SendAsync]", ex);
                return InvokeResult.FromException("[TAKClient__Send]", ex);
            }
        }

        private bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (IgnoreCertificateErrors)
                return true;

            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            return _customRootCert.GetPublicKeyString() ==
                chain.ChainElements[chain.ChainElements.Count - 1].Certificate.GetPublicKeyString();
        }

        private InvokeResult<Manifest> GetManifest(ZipArchive package)
        {
            var manifestEntry = package.Entries.FirstOrDefault(e => e.Name == "MANIFEST.xml");
            if (manifestEntry == null)
                return InvokeResult<Manifest>.FromError("Zip File does not contain file MANIFEST.xml, this typeically should be in a folder MANIFEST/MANIFEST.xml");

            using (var manifestFile = manifestEntry.Open())
            using (var xmlStream = new StreamReader(manifestFile))
            {

                var serializer = new XmlSerializer(typeof(Manifest));

                return InvokeResult<Manifest>.Create((Manifest)serializer.Deserialize(xmlStream));

            }
        }

        private InvokeResult<Preferences> GetPrefernces(ZipArchive package, Manifest manifest)
        {
            if (manifest.Contents == null)
                return InvokeResult<Preferences>.FromError("Contents node not found in manifest.");

            if (manifest.Contents.Items == null)
                return InvokeResult<Preferences>.FromError("Items node not found in Content in manifest.");

            var profileEntry = manifest.Contents.Items.FirstOrDefault(itm => itm.ZipEntry.ToLower().EndsWith("pref"));
            if (profileEntry == null)
                return InvokeResult<Preferences>.FromError("could not find file that ends with .pref that should include preferences to connect to the TAK server.");

            var prefEntry = package.Entries.FirstOrDefault(e => e.FullName == profileEntry.ZipEntry);
            if (prefEntry == null)
                return InvokeResult<Preferences>.FromError($"could not find file {profileEntry.ZipEntry} specified in MANIFEST.xml within the ZIP file.");

            using (var prefStream = prefEntry.Open())
            {
                using (var xmlStream = new StreamReader(prefStream))
                {
                    var serializer = new XmlSerializer(typeof(Preferences));

                    var preferences = serializer.Deserialize(xmlStream) as Preferences;

                    var connectionStream = preferences.Preference.FirstOrDefault(p => p.Name == CoTStreamsKey);
                    if (connectionStream == null)
                        return InvokeResult<Preferences>.FromError($"Could not find {CoTStreamsKey} in preferences file in the prefernce section.");

                    var entry = connectionStream.Entry;
                    if (entry == null)
                        return InvokeResult<Preferences>.FromError($"Could not find entry in {CoTStreamsKey} in preferences file to identify the connection string.");

                    var connectionString = entry.First(e => e.Key == ConnectionStringKey);
                    if (connectionString == null)
                        return InvokeResult<Preferences>.FromError($"Could not find entry in {ConnectionStringKey} in entry node of preferences file to identify the connection string.");

                    if (string.IsNullOrEmpty(connectionString.Text))
                        return InvokeResult<Preferences>.FromError($"value is null for entry in {ConnectionStringKey} in entry node of preferences file to identify the connection string.");


                    var connectionParams = connectionString.Text.Split(':');
                    if (connectionParams.Length < 2)
                        return InvokeResult<Preferences>.FromError($"At minimum the connection string should contain two parts separated by a : such as mytaksever.com:port.");

                    _host = connectionParams[0];
                    _port = int.Parse(connectionParams[1]);

                    if (connectionParams.Length > 2)
                    {
                        _ssl = connectionParams[2].ToLower() == "ssl";
                    }

                    return InvokeResult<Preferences>.Create(preferences);
                }
            }
        }
    }
}
