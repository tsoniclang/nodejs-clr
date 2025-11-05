using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Xunit;
using Tsonic.Node;

#pragma warning disable SYSLIB0057 // Obsolete X509Certificate2 constructor

namespace Tsonic.Node.Tests;

public class TlsTests : IDisposable
{
    private const int TEST_PORT = 18300;
    private X509Certificate2? _serverCert;
    private X509Certificate2? _clientCert;

    public TlsTests()
    {
        // Generate self-signed certificates for testing
        _serverCert = GenerateSelfSignedCertificate("CN=localhost");
        _clientCert = GenerateSelfSignedCertificate("CN=testclient");
    }

    public void Dispose()
    {
        _serverCert?.Dispose();
        _clientCert?.Dispose();
    }

    private static X509Certificate2 GenerateSelfSignedCertificate(string subjectName)
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(subjectName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        // Add extensions
        request.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(false, false, 0, false));

        request.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment,
                false));

        request.CertificateExtensions.Add(
            new X509EnhancedKeyUsageExtension(
                new OidCollection {
                    new Oid("1.3.6.1.5.5.7.3.1"), // Server Authentication
                    new Oid("1.3.6.1.5.5.7.3.2")  // Client Authentication
                },
                false));

        // Generate certificate
        var certificate = request.CreateSelfSigned(
            DateTimeOffset.Now.AddDays(-1),
            DateTimeOffset.Now.AddYears(1));

        // Export and re-import to ensure we have the private key
        var pfxData = certificate.Export(X509ContentType.Pfx, "");
        return new X509Certificate2(pfxData, "", X509KeyStorageFlags.Exportable);
    }

    // ==================== Static tls Module Tests ====================

    [Fact]
    public void CreateServer_NoArgs_ReturnsServer()
    {
        var server = tls.createServer();
        Assert.NotNull(server);
        Assert.IsType<TLSServer>(server);
    }

    [Fact]
    public void CreateServer_WithListener_AttachesListener()
    {
        var server = tls.createServer((socket) => { });
        Assert.NotNull(server);
    }

    [Fact]
    public void CreateServer_WithOptions_ReturnsServer()
    {
        var options = new TlsOptions { cert = _serverCert };
        var server = tls.createServer(options);
        Assert.NotNull(server);
    }

    [Fact]
    public void CreateSecureContext_NoOptions_ReturnsContext()
    {
        var context = tls.createSecureContext();
        Assert.NotNull(context);
    }

    [Fact]
    public void CreateSecureContext_WithCert_LoadsCertificate()
    {
        var context = tls.createSecureContext(new SecureContextOptions
        {
            cert = _serverCert
        });
        Assert.NotNull(context);
        Assert.NotNull(context.Certificate);
    }

    [Fact]
    public void CheckServerIdentity_MatchingHostname_ReturnsNull()
    {
        var cert = new PeerCertificate
        {
            subject = new TLSCertificateInfo { CN = "example.com" }
        };
        var error = tls.checkServerIdentity("example.com", cert);
        Assert.Null(error);
    }

    [Fact]
    public void CheckServerIdentity_MismatchedHostname_ReturnsError()
    {
        var cert = new PeerCertificate
        {
            subject = new TLSCertificateInfo { CN = "example.com" }
        };
        var error = tls.checkServerIdentity("other.com", cert);
        Assert.NotNull(error);
    }

    [Fact]
    public void GetCiphers_ReturnsArray()
    {
        var ciphers = tls.getCiphers();
        Assert.NotNull(ciphers);
        Assert.NotEmpty(ciphers);
    }

    [Fact]
    public void GetCACertificates_ReturnsArray()
    {
        var certs = tls.getCACertificates();
        Assert.NotNull(certs);
        // May be empty depending on system
    }

    [Fact]
    public void Constants_HaveExpectedValues()
    {
        Assert.Equal(3, tls.CLIENT_RENEG_LIMIT);
        Assert.Equal(600, tls.CLIENT_RENEG_WINDOW);
        Assert.Equal("auto", tls.DEFAULT_ECDH_CURVE);
        Assert.Equal("TLSv1.3", tls.DEFAULT_MAX_VERSION);
        Assert.Equal("TLSv1.2", tls.DEFAULT_MIN_VERSION);
    }

    // ==================== SecureContext Tests ====================

    [Fact]
    public void SecureContext_Constructor_CreatesInstance()
    {
        var context = new SecureContext();
        Assert.NotNull(context);
    }

    [Fact]
    public void SecureContext_LoadCertificate_StoresCertificate()
    {
        var context = new SecureContext();
        context.LoadCertificate(_serverCert, null, null);
        Assert.NotNull(context.Certificate);
    }

    [Fact]
    public void SecureContext_SetProtocols_ConfiguresProtocols()
    {
        var context = new SecureContext();
        context.SetProtocols("TLSv1.2", "TLSv1.3");
        Assert.NotEqual(System.Security.Authentication.SslProtocols.None, context.Protocols);
    }

    // ==================== TLSSocket Tests ====================

    [Fact]
    public void TLSSocket_Constructor_CreatesInstance()
    {
        var baseSocket = new Socket();
        var tlsSocket = new TLSSocket(baseSocket);
        Assert.NotNull(tlsSocket);
    }

    [Fact]
    public void TLSSocket_Encrypted_AlwaysTrue()
    {
        var baseSocket = new Socket();
        var tlsSocket = new TLSSocket(baseSocket);
        Assert.True(tlsSocket.encrypted);
    }

    [Fact]
    public void TLSSocket_Authorized_InitiallyFalse()
    {
        var baseSocket = new Socket();
        var tlsSocket = new TLSSocket(baseSocket);
        Assert.False(tlsSocket.authorized);
    }

    [Fact]
    public void TLSSocket_GetCertificate_NoCert_ReturnsNull()
    {
        var baseSocket = new Socket();
        var tlsSocket = new TLSSocket(baseSocket);
        var cert = tlsSocket.getCertificate();
        Assert.Null(cert);
    }

    [Fact]
    public void TLSSocket_GetPeerCertificate_NoCert_ReturnsNull()
    {
        var baseSocket = new Socket();
        var tlsSocket = new TLSSocket(baseSocket);
        var cert = tlsSocket.getPeerCertificate();
        Assert.Null(cert);
    }

    [Fact]
    public void TLSSocket_GetCipher_ReturnsInfo()
    {
        var baseSocket = new Socket();
        var tlsSocket = new TLSSocket(baseSocket);
        var cipher = tlsSocket.getCipher();
        Assert.NotNull(cipher);
        Assert.NotNull(cipher.name);
    }

    [Fact]
    public void TLSSocket_GetProtocol_NoHandshake_ReturnsNull()
    {
        var baseSocket = new Socket();
        var tlsSocket = new TLSSocket(baseSocket);
        var protocol = tlsSocket.getProtocol();
        Assert.Null(protocol);
    }

    [Fact]
    public void TLSSocket_GetSharedSigalgs_ReturnsArray()
    {
        var baseSocket = new Socket();
        var tlsSocket = new TLSSocket(baseSocket);
        var sigalgs = tlsSocket.getSharedSigalgs();
        Assert.NotNull(sigalgs);
    }

    [Fact]
    public void TLSSocket_IsSessionReused_ReturnsFalse()
    {
        var baseSocket = new Socket();
        var tlsSocket = new TLSSocket(baseSocket);
        Assert.False(tlsSocket.isSessionReused());
    }

    [Fact]
    public void TLSSocket_Renegotiate_ReturnsfalseAndCallsCallback()
    {
        var baseSocket = new Socket();
        var tlsSocket = new TLSSocket(baseSocket);
        var resetEvent = new ManualResetEventSlim(false);
        Exception? error = null;

        var result = tlsSocket.renegotiate(new { }, (err) =>
        {
            error = err;
            resetEvent.Set();
        });

        Assert.False(result);
        resetEvent.Wait(1000);
        Assert.NotNull(error);
        Assert.IsType<NotSupportedException>(error);
    }

    [Fact]
    public void TLSSocket_SetMaxSendFragment_ReturnsFalse()
    {
        var baseSocket = new Socket();
        var tlsSocket = new TLSSocket(baseSocket);
        var result = tlsSocket.setMaxSendFragment(1024);
        Assert.False(result);
    }

    [Fact]
    public void TLSSocket_DisableRenegotiation_DoesNotThrow()
    {
        var baseSocket = new Socket();
        var tlsSocket = new TLSSocket(baseSocket);
        var exception = Record.Exception(() => tlsSocket.disableRenegotiation());
        Assert.Null(exception);
    }

    [Fact]
    public void TLSSocket_EnableTrace_DoesNotThrow()
    {
        var baseSocket = new Socket();
        var tlsSocket = new TLSSocket(baseSocket);
        var exception = Record.Exception(() => tlsSocket.enableTrace());
        Assert.Null(exception);
    }

    [Fact]
    public void TLSSocket_ExportKeyingMaterial_ThrowsNotSupported()
    {
        var baseSocket = new Socket();
        var tlsSocket = new TLSSocket(baseSocket);
        Assert.Throws<NotSupportedException>(() =>
            tlsSocket.exportKeyingMaterial(128, "label", new byte[0]));
    }

    // ==================== TLSServer Tests ====================

    [Fact]
    public void TLSServer_Constructor_CreatesInstance()
    {
        var server = new TLSServer();
        Assert.NotNull(server);
    }

    [Fact]
    public void TLSServer_ConstructorWithListener_AttachesListener()
    {
        var server = new TLSServer((socket) => { });
        Assert.NotNull(server);
    }

    [Fact]
    public void TLSServer_ConstructorWithOptions_CreatesInstance()
    {
        var options = new TlsOptions { cert = _serverCert };
        var server = new TLSServer(options, null);
        Assert.NotNull(server);
    }

    [Fact]
    public void TLSServer_GetTicketKeys_Returns48Bytes()
    {
        var server = new TLSServer();
        var keys = server.getTicketKeys();
        Assert.NotNull(keys);
        Assert.Equal(48, keys.Length);
    }

    [Fact]
    public void TLSServer_SetTicketKeys_AcceptsValidKeys()
    {
        var server = new TLSServer();
        var keys = new byte[48];
        new Random().NextBytes(keys);

        var exception = Record.Exception(() => server.setTicketKeys(keys));
        Assert.Null(exception);
    }

    [Fact]
    public void TLSServer_SetTicketKeys_InvalidLength_Throws()
    {
        var server = new TLSServer();
        var keys = new byte[32]; // Wrong length

        Assert.Throws<ArgumentException>(() => server.setTicketKeys(keys));
    }

    [Fact]
    public void TLSServer_SetSecureContext_AcceptsOptions()
    {
        var server = new TLSServer();
        var exception = Record.Exception(() =>
            server.setSecureContext(new SecureContextOptions { cert = _serverCert }));
        Assert.Null(exception);
    }

    [Fact]
    public void TLSServer_AddContext_DoesNotThrow()
    {
        var server = new TLSServer();
        var exception = Record.Exception(() =>
            server.addContext("example.com", new SecureContextOptions()));
        Assert.Null(exception);
    }

    // ==================== Options Classes Tests ====================

    [Fact]
    public void TLSCertificateInfo_AllProperties_CanBeSet()
    {
        var cert = new TLSCertificateInfo
        {
            C = "US",
            ST = "CA",
            L = "San Francisco",
            O = "Test Corp",
            OU = "Test Unit",
            CN = "test.example.com"
        };

        Assert.Equal("US", cert.C);
        Assert.Equal("CA", cert.ST);
        Assert.Equal("San Francisco", cert.L);
        Assert.Equal("Test Corp", cert.O);
        Assert.Equal("Test Unit", cert.OU);
        Assert.Equal("test.example.com", cert.CN);
    }

    [Fact]
    public void PeerCertificate_AllProperties_CanBeSet()
    {
        var cert = new PeerCertificate
        {
            ca = true,
            raw = new byte[] { 1, 2, 3 },
            serialNumber = "ABC123",
            fingerprint = "SHA1",
            fingerprint256 = "SHA256",
            fingerprint512 = "SHA512",
            valid_from = "Jan 1 2024",
            valid_to = "Dec 31 2025"
        };

        Assert.True(cert.ca);
        Assert.NotNull(cert.raw);
        Assert.Equal("ABC123", cert.serialNumber);
    }

    [Fact]
    public void CipherNameAndProtocol_AllProperties_CanBeSet()
    {
        var cipher = new CipherNameAndProtocol
        {
            name = "AES256-GCM-SHA384",
            version = "TLSv1.3",
            standardName = "TLS_AES_256_GCM_SHA384"
        };

        Assert.Equal("AES256-GCM-SHA384", cipher.name);
        Assert.Equal("TLSv1.3", cipher.version);
    }

    [Fact]
    public void EphemeralKeyInfo_AllProperties_CanBeSet()
    {
        var info = new EphemeralKeyInfo
        {
            type = "ECDH",
            name = "prime256v1",
            size = 256
        };

        Assert.Equal("ECDH", info.type);
        Assert.Equal("prime256v1", info.name);
        Assert.Equal(256, info.size);
    }

    [Fact]
    public void SecureContextOptions_AllProperties_CanBeSet()
    {
        var opts = new SecureContextOptions
        {
            ca = "ca-cert",
            cert = "cert",
            key = "key",
            passphrase = "pass",
            ciphers = "HIGH",
            maxVersion = "TLSv1.3",
            minVersion = "TLSv1.2"
        };

        Assert.Equal("ca-cert", opts.ca);
        Assert.Equal("pass", opts.passphrase);
    }

    [Fact]
    public void TLSSocketOptions_AllProperties_CanBeSet()
    {
        var opts = new TLSSocketOptions
        {
            isServer = true,
            servername = "example.com",
            ca = "ca",
            cert = "cert",
            key = "key",
            passphrase = "pass"
        };

        Assert.True(opts.isServer);
        Assert.Equal("example.com", opts.servername);
    }

    [Fact]
    public void ConnectionOptions_AllProperties_CanBeSet()
    {
        var opts = new ConnectionOptions
        {
            host = "example.com",
            port = 443,
            servername = "example.com",
            timeout = 5000
        };

        Assert.Equal("example.com", opts.host);
        Assert.Equal(443, opts.port);
        Assert.Equal(5000, opts.timeout);
    }

    [Fact]
    public void TlsOptions_AllProperties_CanBeSet()
    {
        var opts = new TlsOptions
        {
            handshakeTimeout = 120000,
            sessionTimeout = 300,
            ca = "ca",
            cert = "cert",
            key = "key",
            passphrase = "pass"
        };

        Assert.Equal(120000, opts.handshakeTimeout);
        Assert.Equal(300, opts.sessionTimeout);
    }

    // Note: Integration tests for actual TLS connections would require
    // more complex setup with real server/client interactions.
    // Those are better suited for end-to-end testing scenarios.
}
