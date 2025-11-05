using System;
using System.Linq;
using System.Threading;
using Xunit;
using Tsonic.Node;

namespace Tsonic.Node.Tests;

public class DnsTests
{
    // ==================== Constants Tests ====================

    [Fact]
    public void Constants_ADDRCONFIG_IsCorrectValue()
    {
        Assert.Equal(0x0400, dns.ADDRCONFIG);
    }

    [Fact]
    public void Constants_V4MAPPED_IsCorrectValue()
    {
        Assert.Equal(0x0800, dns.V4MAPPED);
    }

    [Fact]
    public void Constants_ALL_IsCorrectValue()
    {
        Assert.Equal(dns.V4MAPPED | dns.ADDRCONFIG, dns.ALL);
    }

    [Fact]
    public void ErrorCodes_AllDefined()
    {
        Assert.Equal("ENODATA", dns.NODATA);
        Assert.Equal("EFORMERR", dns.FORMERR);
        Assert.Equal("ESERVFAIL", dns.SERVFAIL);
        Assert.Equal("ENOTFOUND", dns.NOTFOUND);
        Assert.Equal("ENOTIMP", dns.NOTIMP);
        Assert.Equal("EREFUSED", dns.REFUSED);
        Assert.Equal("EBADQUERY", dns.BADQUERY);
        Assert.Equal("EBADNAME", dns.BADNAME);
        Assert.Equal("EBADFAMILY", dns.BADFAMILY);
        Assert.Equal("EBADRESP", dns.BADRESP);
        Assert.Equal("ECONNREFUSED", dns.CONNREFUSED);
        Assert.Equal("ETIMEOUT", dns.TIMEOUT);
        Assert.Equal("EOF", dns.EOF);
        Assert.Equal("EFILE", dns.FILE);
        Assert.Equal("ENOMEM", dns.NOMEM);
        Assert.Equal("EDESTRUCTION", dns.DESTRUCTION);
        Assert.Equal("EBADSTR", dns.BADSTR);
        Assert.Equal("EBADFLAGS", dns.BADFLAGS);
        Assert.Equal("ENONAME", dns.NONAME);
        Assert.Equal("EBADHINTS", dns.BADHINTS);
        Assert.Equal("ENOTINITIALIZED", dns.NOTINITIALIZED);
        Assert.Equal("ELOADIPHLPAPI", dns.LOADIPHLPAPI);
        Assert.Equal("EADDRGETNETWORKPARAMS", dns.ADDRGETNETWORKPARAMS);
        Assert.Equal("ECANCELLED", dns.CANCELLED);
    }

    // ==================== lookup Tests ====================

    [Fact]
    public void Lookup_SimpleDomain_ReturnsAddress()
    {
        var resetEvent = new ManualResetEventSlim(false);
        string? address = null;
        int family = 0;
        Exception? error = null;

        dns.lookup("localhost", (err, addr, fam) =>
        {
            error = err;
            address = addr;
            family = fam;
            resetEvent.Set();
        });

        var signaled = resetEvent.Wait(5000);
        Assert.True(signaled, "Callback was not called within timeout");
        Assert.Null(error);
        Assert.NotNull(address);
        Assert.True(family == 4 || family == 6);
    }

    [Fact]
    public void Lookup_WithIPv4Family_ReturnsIPv4Address()
    {
        var resetEvent = new ManualResetEventSlim(false);
        string? address = null;
        int family = 0;

        dns.lookup("localhost", 4, (err, addr, fam) =>
        {
            address = addr;
            family = fam;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(address);
        Assert.Equal(4, family);
    }

    [Fact]
    public void Lookup_WithIPv6Family_ReturnsIPv6Address()
    {
        var resetEvent = new ManualResetEventSlim(false);
        string? address = null;
        int family = 0;

        dns.lookup("localhost", 6, (err, addr, fam) =>
        {
            address = addr;
            family = fam;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        // May not have IPv6 support on all systems
        Assert.True(family == 0 || family == 6);
    }

    [Fact]
    public void Lookup_WithOptionsAll_ReturnsAddressArray()
    {
        var resetEvent = new ManualResetEventSlim(false);
        LookupAddress[]? addresses = null;

        dns.lookup("localhost", new LookupOptions { all = true }, (err, addrs) =>
        {
            addresses = addrs;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(addresses);
        Assert.True(addresses.Length > 0);
        Assert.All(addresses, addr =>
        {
            Assert.NotEmpty(addr.address);
            Assert.True(addr.family == 4 || addr.family == 6);
        });
    }

    [Fact]
    public void Lookup_WithIPv4FirstOrder_SortsCorrectly()
    {
        var resetEvent = new ManualResetEventSlim(false);
        LookupAddress[]? addresses = null;

        dns.lookup("localhost", new LookupOptions { all = true, order = "ipv4first" }, (err, addrs) =>
        {
            addresses = addrs;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(addresses);

        // Check that IPv4 addresses come before IPv6
        var ipv4Index = Array.FindIndex(addresses, a => a.family == 4);
        var ipv6Index = Array.FindIndex(addresses, a => a.family == 6);

        if (ipv4Index >= 0 && ipv6Index >= 0)
        {
            Assert.True(ipv4Index < ipv6Index);
        }
    }

    [Fact]
    public void Lookup_InvalidHostname_ReturnsError()
    {
        var resetEvent = new ManualResetEventSlim(false);
        Exception? error = null;

        dns.lookup("this-hostname-definitely-does-not-exist-12345.invalid", (err, addr, fam) =>
        {
            error = err;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(error);
    }

    [Fact]
    public void Lookup_WithOptionsFamily_WorksAsExpected()
    {
        var resetEvent = new ManualResetEventSlim(false);
        int family = 0;

        dns.lookup("localhost", new LookupOptions { family = 4 }, (err, addr, fam) =>
        {
            family = fam;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.Equal(4, family);
    }

    [Fact]
    public void Lookup_WithStringFamilyIPv4_WorksAsExpected()
    {
        var resetEvent = new ManualResetEventSlim(false);
        int family = 0;

        dns.lookup("localhost", new LookupOptions { family = "IPv4" }, (err, addr, fam) =>
        {
            family = fam;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.Equal(4, family);
    }

    // ==================== lookupService Tests ====================

    [Fact]
    public void LookupService_ValidIPAndPort_ReturnsHostnameAndService()
    {
        var resetEvent = new ManualResetEventSlim(false);
        string? hostname = null;
        string? service = null;
        Exception? error = null;

        dns.lookupService("127.0.0.1", 22, (err, host, svc) =>
        {
            error = err;
            hostname = host;
            service = svc;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.Null(error);
        Assert.NotNull(hostname);
        Assert.NotNull(service);
    }

    [Fact]
    public void LookupService_InvalidIP_ReturnsError()
    {
        var resetEvent = new ManualResetEventSlim(false);
        Exception? error = null;

        dns.lookupService("invalid-ip", 22, (err, host, svc) =>
        {
            error = err;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(error);
    }

    [Fact]
    public void LookupService_InvalidPort_ReturnsError()
    {
        var resetEvent = new ManualResetEventSlim(false);
        Exception? error = null;

        dns.lookupService("127.0.0.1", 99999, (err, host, svc) =>
        {
            error = err;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(error);
    }

    // ==================== resolve Tests ====================

    [Fact]
    public void Resolve_SimpleDomain_ReturnsAddresses()
    {
        var resetEvent = new ManualResetEventSlim(false);
        string[]? addresses = null;

        dns.resolve("localhost", (err, addrs) =>
        {
            addresses = addrs;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(addresses);
        Assert.True(addresses.Length > 0);
    }

    [Fact]
    public void Resolve_WithARecordType_ReturnsIPv4Addresses()
    {
        var resetEvent = new ManualResetEventSlim(false);
        object? result = null;

        dns.resolve("localhost", "A", (err, res) =>
        {
            result = res;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(result);
        Assert.IsType<string[]>(result);
    }

    [Fact]
    public void Resolve_WithAAAARecordType_ReturnsIPv6Addresses()
    {
        var resetEvent = new ManualResetEventSlim(false);
        object? result = null;

        dns.resolve("localhost", "AAAA", (err, res) =>
        {
            result = res;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(result);
        Assert.IsType<string[]>(result);
    }

    [Fact]
    public void Resolve_WithMXRecordType_ReturnsMxRecords()
    {
        var resetEvent = new ManualResetEventSlim(false);
        object? result = null;

        dns.resolve("localhost", "MX", (err, res) =>
        {
            result = res;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(result);
        Assert.IsType<MxRecord[]>(result);
    }

    [Fact]
    public void Resolve_WithTXTRecordType_ReturnsTxtRecords()
    {
        var resetEvent = new ManualResetEventSlim(false);
        object? result = null;

        dns.resolve("localhost", "TXT", (err, res) =>
        {
            result = res;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(result);
        Assert.IsType<string[][]>(result);
    }

    [Fact]
    public void Resolve_WithInvalidRecordType_ReturnsError()
    {
        var resetEvent = new ManualResetEventSlim(false);
        Exception? error = null;

        dns.resolve("localhost", "INVALID", (err, res) =>
        {
            error = err;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(error);
    }

    // ==================== resolve4 Tests ====================

    [Fact]
    public void Resolve4_ValidDomain_ReturnsIPv4Addresses()
    {
        var resetEvent = new ManualResetEventSlim(false);
        string[]? addresses = null;

        dns.resolve4("localhost", (err, addrs) =>
        {
            addresses = addrs;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(addresses);
    }

    [Fact]
    public void Resolve4_WithTtlOption_ReturnsRecordsWithTtl()
    {
        var resetEvent = new ManualResetEventSlim(false);
        object? result = null;

        dns.resolve4("localhost", new ResolveOptions { ttl = true }, (err, res) =>
        {
            result = res;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(result);
        Assert.IsType<RecordWithTtl[]>(result);
    }

    [Fact]
    public void Resolve4_WithoutTtlOption_ReturnsStringArray()
    {
        var resetEvent = new ManualResetEventSlim(false);
        object? result = null;

        dns.resolve4("localhost", new ResolveOptions { ttl = false }, (err, res) =>
        {
            result = res;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(result);
        Assert.IsType<string[]>(result);
    }

    // ==================== resolve6 Tests ====================

    [Fact]
    public void Resolve6_ValidDomain_ReturnsIPv6Addresses()
    {
        var resetEvent = new ManualResetEventSlim(false);
        string[]? addresses = null;

        dns.resolve6("localhost", (err, addrs) =>
        {
            addresses = addrs;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(addresses);
    }

    [Fact]
    public void Resolve6_WithTtlOption_ReturnsRecordsWithTtl()
    {
        var resetEvent = new ManualResetEventSlim(false);
        object? result = null;

        dns.resolve6("localhost", new ResolveOptions { ttl = true }, (err, res) =>
        {
            result = res;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(result);
        Assert.IsType<RecordWithTtl[]>(result);
    }

    // ==================== Specific resolve* Tests ====================

    [Fact]
    public void ResolveCname_ValidDomain_ReturnsCnames()
    {
        var resetEvent = new ManualResetEventSlim(false);
        string[]? cnames = null;

        dns.resolveCname("localhost", (err, names) =>
        {
            cnames = names;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(cnames);
    }

    [Fact]
    public void ResolveCaa_ValidDomain_ReturnsCaaRecords()
    {
        var resetEvent = new ManualResetEventSlim(false);
        CaaRecord[]? records = null;

        dns.resolveCaa("localhost", (err, recs) =>
        {
            records = recs;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(records);
    }

    [Fact]
    public void ResolveMx_ValidDomain_ReturnsMxRecords()
    {
        var resetEvent = new ManualResetEventSlim(false);
        MxRecord[]? records = null;

        dns.resolveMx("localhost", (err, recs) =>
        {
            records = recs;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(records);
    }

    [Fact]
    public void ResolveNaptr_ValidDomain_ReturnsNaptrRecords()
    {
        var resetEvent = new ManualResetEventSlim(false);
        NaptrRecord[]? records = null;

        dns.resolveNaptr("localhost", (err, recs) =>
        {
            records = recs;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(records);
    }

    [Fact]
    public void ResolveNs_ValidDomain_ReturnsNameServers()
    {
        var resetEvent = new ManualResetEventSlim(false);
        string[]? nameservers = null;

        dns.resolveNs("localhost", (err, ns) =>
        {
            nameservers = ns;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(nameservers);
    }

    [Fact]
    public void ResolvePtr_ValidDomain_ReturnsPtrRecords()
    {
        var resetEvent = new ManualResetEventSlim(false);
        string[]? records = null;

        dns.resolvePtr("localhost", (err, recs) =>
        {
            records = recs;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(records);
    }

    [Fact]
    public void ResolveSoa_ValidDomain_ReturnsSoaRecord()
    {
        var resetEvent = new ManualResetEventSlim(false);
        SoaRecord? record = null;

        dns.resolveSoa("localhost", (err, rec) =>
        {
            record = rec;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(record);
    }

    [Fact]
    public void ResolveSrv_ValidDomain_ReturnsSrvRecords()
    {
        var resetEvent = new ManualResetEventSlim(false);
        SrvRecord[]? records = null;

        dns.resolveSrv("localhost", (err, recs) =>
        {
            records = recs;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(records);
    }

    [Fact]
    public void ResolveTlsa_ValidDomain_ReturnsTlsaRecords()
    {
        var resetEvent = new ManualResetEventSlim(false);
        TlsaRecord[]? records = null;

        dns.resolveTlsa("localhost", (err, recs) =>
        {
            records = recs;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(records);
    }

    [Fact]
    public void ResolveTxt_ValidDomain_ReturnsTxtRecords()
    {
        var resetEvent = new ManualResetEventSlim(false);
        string[][]? records = null;

        dns.resolveTxt("localhost", (err, recs) =>
        {
            records = recs;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(records);
    }

    [Fact]
    public void ResolveAny_ValidDomain_ReturnsAnyRecords()
    {
        var resetEvent = new ManualResetEventSlim(false);
        object[]? records = null;

        dns.resolveAny("localhost", (err, recs) =>
        {
            records = recs;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(records);
    }

    // ==================== reverse Tests ====================

    [Fact]
    public void Reverse_ValidIPv4_ReturnsHostnames()
    {
        var resetEvent = new ManualResetEventSlim(false);
        string[]? hostnames = null;
        Exception? error = null;

        dns.reverse("127.0.0.1", (err, hosts) =>
        {
            error = err;
            hostnames = hosts;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.Null(error);
        Assert.NotNull(hostnames);
        Assert.True(hostnames.Length > 0);
    }

    [Fact]
    public void Reverse_ValidIPv6_ReturnsHostnames()
    {
        var resetEvent = new ManualResetEventSlim(false);
        string[]? hostnames = null;
        Exception? error = null;

        dns.reverse("::1", (err, hosts) =>
        {
            error = err;
            hostnames = hosts;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.Null(error);
        Assert.NotNull(hostnames);
    }

    [Fact]
    public void Reverse_InvalidIP_ReturnsError()
    {
        var resetEvent = new ManualResetEventSlim(false);
        Exception? error = null;

        dns.reverse("invalid-ip", (err, hosts) =>
        {
            error = err;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(error);
    }

    // ==================== Configuration Tests ====================

    [Fact]
    public void GetDefaultResultOrder_ReturnsVerbatim()
    {
        var order = dns.getDefaultResultOrder();
        Assert.Equal("verbatim", order);
    }

    [Fact]
    public void SetDefaultResultOrder_IPv4First_UpdatesOrder()
    {
        dns.setDefaultResultOrder("ipv4first");
        var order = dns.getDefaultResultOrder();
        Assert.Equal("ipv4first", order);

        // Reset to default
        dns.setDefaultResultOrder("verbatim");
    }

    [Fact]
    public void SetDefaultResultOrder_IPv6First_UpdatesOrder()
    {
        dns.setDefaultResultOrder("ipv6first");
        var order = dns.getDefaultResultOrder();
        Assert.Equal("ipv6first", order);

        // Reset to default
        dns.setDefaultResultOrder("verbatim");
    }

    [Fact]
    public void SetDefaultResultOrder_Verbatim_UpdatesOrder()
    {
        dns.setDefaultResultOrder("verbatim");
        var order = dns.getDefaultResultOrder();
        Assert.Equal("verbatim", order);
    }

    [Fact]
    public void SetDefaultResultOrder_InvalidValue_ThrowsError()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            dns.setDefaultResultOrder("invalid");
        });
    }

    [Fact]
    public void SetServers_ValidServers_DoesNotThrow()
    {
        var exception = Record.Exception(() =>
        {
            dns.setServers(new[] { "8.8.8.8", "8.8.4.4" });
        });
        Assert.Null(exception);
    }

    [Fact]
    public void GetServers_ReturnsServerArray()
    {
        var servers = dns.getServers();
        Assert.NotNull(servers);
        Assert.IsType<string[]>(servers);
    }

    // ==================== Resolver Class Tests ====================

    [Fact]
    public void Resolver_Constructor_CreatesInstance()
    {
        var resolver = new Resolver();
        Assert.NotNull(resolver);
    }

    [Fact]
    public void Resolver_ConstructorWithOptions_CreatesInstance()
    {
        var options = new ResolverOptions
        {
            timeout = 5000,
            tries = 3
        };
        var resolver = new Resolver(options);
        Assert.NotNull(resolver);
    }

    [Fact]
    public void Resolver_Cancel_DoesNotThrow()
    {
        var resolver = new Resolver();
        var exception = Record.Exception(() =>
        {
            resolver.cancel();
        });
        Assert.Null(exception);
    }

    [Fact]
    public void Resolver_Cancel_SubsequentCallsReturnCancelledError()
    {
        var resolver = new Resolver();
        resolver.cancel();

        var resetEvent = new ManualResetEventSlim(false);
        Exception? error = null;

        resolver.resolve4("localhost", (err, addrs) =>
        {
            error = err;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(error);
        Assert.Contains("ECANCELLED", error.Message);
    }

    [Fact]
    public void Resolver_Resolve4_WorksLikeStaticMethod()
    {
        var resolver = new Resolver();
        var resetEvent = new ManualResetEventSlim(false);
        string[]? addresses = null;

        resolver.resolve4("localhost", (err, addrs) =>
        {
            addresses = addrs;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(addresses);
    }

    [Fact]
    public void Resolver_Resolve6_WorksLikeStaticMethod()
    {
        var resolver = new Resolver();
        var resetEvent = new ManualResetEventSlim(false);
        string[]? addresses = null;

        resolver.resolve6("localhost", (err, addrs) =>
        {
            addresses = addrs;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(addresses);
    }

    [Fact]
    public void Resolver_ResolveMx_WorksLikeStaticMethod()
    {
        var resolver = new Resolver();
        var resetEvent = new ManualResetEventSlim(false);
        MxRecord[]? records = null;

        resolver.resolveMx("localhost", (err, recs) =>
        {
            records = recs;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(records);
    }

    [Fact]
    public void Resolver_Reverse_WorksLikeStaticMethod()
    {
        var resolver = new Resolver();
        var resetEvent = new ManualResetEventSlim(false);
        string[]? hostnames = null;

        resolver.reverse("127.0.0.1", (err, hosts) =>
        {
            hostnames = hosts;
            resetEvent.Set();
        });

        resetEvent.Wait(5000);
        Assert.NotNull(hostnames);
    }

    [Fact]
    public void Resolver_SetLocalAddress_DoesNotThrow()
    {
        var resolver = new Resolver();
        var exception = Record.Exception(() =>
        {
            resolver.setLocalAddress("0.0.0.0", "::0");
        });
        Assert.Null(exception);
    }

    [Fact]
    public void Resolver_GetServers_ReturnsArray()
    {
        var resolver = new Resolver();
        var servers = resolver.getServers();
        Assert.NotNull(servers);
        Assert.IsType<string[]>(servers);
    }

    [Fact]
    public void Resolver_SetServers_DoesNotThrow()
    {
        var resolver = new Resolver();
        var exception = Record.Exception(() =>
        {
            resolver.setServers(new[] { "8.8.8.8" });
        });
        Assert.Null(exception);
    }

    // ==================== Options Classes Tests ====================

    [Fact]
    public void LookupOptions_AllProperties_CanBeSet()
    {
        var options = new LookupOptions
        {
            family = 4,
            hints = dns.ADDRCONFIG,
            all = true,
            order = "ipv4first",
            verbatim = false
        };

        Assert.Equal(4, options.family);
        Assert.Equal(dns.ADDRCONFIG, options.hints);
        Assert.True(options.all);
        Assert.Equal("ipv4first", options.order);
        Assert.False(options.verbatim);
    }

    [Fact]
    public void LookupAddress_AllProperties_CanBeSet()
    {
        var address = new LookupAddress
        {
            address = "127.0.0.1",
            family = 4
        };

        Assert.Equal("127.0.0.1", address.address);
        Assert.Equal(4, address.family);
    }

    [Fact]
    public void ResolveOptions_TtlProperty_CanBeSet()
    {
        var options = new ResolveOptions { ttl = true };
        Assert.True(options.ttl);
    }

    [Fact]
    public void ResolverOptions_AllProperties_CanBeSet()
    {
        var options = new ResolverOptions
        {
            timeout = 5000,
            tries = 3,
            maxTimeout = 10000
        };

        Assert.Equal(5000, options.timeout);
        Assert.Equal(3, options.tries);
        Assert.Equal(10000, options.maxTimeout);
    }

    // ==================== Record Classes Tests ====================

    [Fact]
    public void RecordWithTtl_AllProperties_CanBeSet()
    {
        var record = new RecordWithTtl
        {
            address = "127.0.0.1",
            ttl = 300
        };

        Assert.Equal("127.0.0.1", record.address);
        Assert.Equal(300, record.ttl);
    }

    [Fact]
    public void AnyARecord_HasCorrectType()
    {
        var record = new AnyARecord();
        Assert.Equal("A", record.type);
    }

    [Fact]
    public void AnyAaaaRecord_HasCorrectType()
    {
        var record = new AnyAaaaRecord();
        Assert.Equal("AAAA", record.type);
    }

    [Fact]
    public void CaaRecord_AllProperties_CanBeSet()
    {
        var record = new CaaRecord
        {
            critical = 0,
            issue = "example.com",
            issuewild = "*.example.com",
            iodef = "mailto:security@example.com",
            contactemail = "security@example.com",
            contactphone = "+1-555-0100"
        };

        Assert.Equal(0, record.critical);
        Assert.Equal("example.com", record.issue);
        Assert.Equal("*.example.com", record.issuewild);
        Assert.Equal("mailto:security@example.com", record.iodef);
        Assert.Equal("security@example.com", record.contactemail);
        Assert.Equal("+1-555-0100", record.contactphone);
    }

    [Fact]
    public void AnyCaaRecord_HasCorrectType()
    {
        var record = new AnyCaaRecord();
        Assert.Equal("CAA", record.type);
    }

    [Fact]
    public void MxRecord_AllProperties_CanBeSet()
    {
        var record = new MxRecord
        {
            priority = 10,
            exchange = "mail.example.com"
        };

        Assert.Equal(10, record.priority);
        Assert.Equal("mail.example.com", record.exchange);
    }

    [Fact]
    public void AnyMxRecord_HasCorrectType()
    {
        var record = new AnyMxRecord();
        Assert.Equal("MX", record.type);
    }

    [Fact]
    public void NaptrRecord_AllProperties_CanBeSet()
    {
        var record = new NaptrRecord
        {
            flags = "s",
            service = "SIP+D2U",
            regexp = "",
            replacement = "_sip._udp.example.com",
            order = 30,
            preference = 100
        };

        Assert.Equal("s", record.flags);
        Assert.Equal("SIP+D2U", record.service);
        Assert.Equal("", record.regexp);
        Assert.Equal("_sip._udp.example.com", record.replacement);
        Assert.Equal(30, record.order);
        Assert.Equal(100, record.preference);
    }

    [Fact]
    public void AnyNaptrRecord_HasCorrectType()
    {
        var record = new AnyNaptrRecord();
        Assert.Equal("NAPTR", record.type);
    }

    [Fact]
    public void SoaRecord_AllProperties_CanBeSet()
    {
        var record = new SoaRecord
        {
            nsname = "ns.example.com",
            hostmaster = "root.example.com",
            serial = 2013101809,
            refresh = 10000,
            retry = 2400,
            expire = 604800,
            minttl = 3600
        };

        Assert.Equal("ns.example.com", record.nsname);
        Assert.Equal("root.example.com", record.hostmaster);
        Assert.Equal(2013101809, record.serial);
        Assert.Equal(10000, record.refresh);
        Assert.Equal(2400, record.retry);
        Assert.Equal(604800, record.expire);
        Assert.Equal(3600, record.minttl);
    }

    [Fact]
    public void AnySoaRecord_HasCorrectType()
    {
        var record = new AnySoaRecord();
        Assert.Equal("SOA", record.type);
    }

    [Fact]
    public void SrvRecord_AllProperties_CanBeSet()
    {
        var record = new SrvRecord
        {
            priority = 10,
            weight = 5,
            port = 21223,
            name = "service.example.com"
        };

        Assert.Equal(10, record.priority);
        Assert.Equal(5, record.weight);
        Assert.Equal(21223, record.port);
        Assert.Equal("service.example.com", record.name);
    }

    [Fact]
    public void AnySrvRecord_HasCorrectType()
    {
        var record = new AnySrvRecord();
        Assert.Equal("SRV", record.type);
    }

    [Fact]
    public void TlsaRecord_AllProperties_CanBeSet()
    {
        var record = new TlsaRecord
        {
            certUsage = 3,
            selector = 1,
            match = 1,
            data = new byte[] { 1, 2, 3, 4 }
        };

        Assert.Equal(3, record.certUsage);
        Assert.Equal(1, record.selector);
        Assert.Equal(1, record.match);
        Assert.Equal(new byte[] { 1, 2, 3, 4 }, record.data);
    }

    [Fact]
    public void AnyTlsaRecord_HasCorrectType()
    {
        var record = new AnyTlsaRecord();
        Assert.Equal("TLSA", record.type);
    }

    [Fact]
    public void AnyTxtRecord_AllProperties_CanBeSet()
    {
        var record = new AnyTxtRecord
        {
            entries = new[] { "v=spf1 include:_spf.example.com ~all" }
        };

        Assert.Equal("TXT", record.type);
        Assert.Single(record.entries);
        Assert.Equal("v=spf1 include:_spf.example.com ~all", record.entries[0]);
    }

    [Fact]
    public void AnyNsRecord_AllProperties_CanBeSet()
    {
        var record = new AnyNsRecord
        {
            value = "ns1.example.com"
        };

        Assert.Equal("NS", record.type);
        Assert.Equal("ns1.example.com", record.value);
    }

    [Fact]
    public void AnyPtrRecord_AllProperties_CanBeSet()
    {
        var record = new AnyPtrRecord
        {
            value = "example.com"
        };

        Assert.Equal("PTR", record.type);
        Assert.Equal("example.com", record.value);
    }

    [Fact]
    public void AnyCnameRecord_AllProperties_CanBeSet()
    {
        var record = new AnyCnameRecord
        {
            value = "www.example.com"
        };

        Assert.Equal("CNAME", record.type);
        Assert.Equal("www.example.com", record.value);
    }
}
