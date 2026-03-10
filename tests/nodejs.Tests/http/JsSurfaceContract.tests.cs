using System;
using System.Collections.Generic;
using Xunit;

namespace nodejs.Tests;

public class HttpJsSurfaceContractTests
{
    [Fact]
    public void Http_IntegerBackedMembers_CompileAgainstExactNumericContracts()
    {
        Action compile = () =>
        {
            nodejs.Http.Server server = null!;
            nodejs.Http.ServerResponse response = null!;
            nodejs.Http.IncomingMessage incoming = null!;

            int timeout = server.timeout;
            int headersTimeout = server.headersTimeout;
            int requestTimeout = server.requestTimeout;
            int keepAliveTimeout = server.keepAliveTimeout;
            int statusCode = response.statusCode;
            int? incomingStatusCode = incoming.statusCode;

            server.timeout = timeout;
            server.headersTimeout = headersTimeout;
            server.requestTimeout = requestTimeout;
            server.keepAliveTimeout = keepAliveTimeout;
            response.statusCode = statusCode;

            _ = incomingStatusCode;
        };

        Assert.NotNull(compile);
    }

    [Fact]
    public void Http_IntegerBackedMethods_CompileAgainstExactNumericContracts()
    {
        Func<nodejs.Http.Server, nodejs.Http.Server> listen = (server) => server.listen(80, "127.0.0.1", 128, () => { });
        Func<nodejs.Http.Server, nodejs.Http.Server> serverSetTimeout = (server) => server.setTimeout(1000, () => { });
        Func<nodejs.Http.IncomingMessage, nodejs.Http.IncomingMessage> incomingSetTimeout = (incoming) => incoming.setTimeout(1000, () => { });
        Func<nodejs.Http.ServerResponse, nodejs.Http.ServerResponse> responseSetTimeout = (response) => response.setTimeout(1000, () => { });
        Func<nodejs.Http.ServerResponse, nodejs.Http.ServerResponse> writeHead = (response) =>
            response.writeHead(204, "No Content", new Dictionary<string, string>());

        Assert.NotNull(listen);
        Assert.NotNull(serverSetTimeout);
        Assert.NotNull(incomingSetTimeout);
        Assert.NotNull(responseSetTimeout);
        Assert.NotNull(writeHead);
    }
}
