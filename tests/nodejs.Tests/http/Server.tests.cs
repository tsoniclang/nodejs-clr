using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using nodejs.Http;

namespace nodejs.Tests;

public class HttpServerTests
{
    [Fact]
    public async Task Server_BasicRequest_ReturnsResponse()
    {
        // Arrange
        var port = 18080;
        var receivedRequest = false;
        string? receivedMethod = null;
        string? receivedUrl = null;

        var server = http.createServer((req, res) =>
        {
            receivedRequest = true;
            receivedMethod = req.method;
            receivedUrl = req.url;

            res.writeHead(200, new System.Collections.Generic.Dictionary<string, string>
            {
                { "Content-Type", "text/plain" }
            });

            res.end("Hello World").Wait();
        });

        server.listen(port, (Action?)null);

        try
        {
            // Act - Make HTTP request to server
            using var client = new HttpClient();
            var response = await client.GetAsync($"http://localhost:{port}/test");
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(receivedRequest);
            Assert.Equal("GET", receivedMethod);
            Assert.Equal("/test", receivedUrl);
            Assert.Equal("Hello World", body);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
        finally
        {
            // Cleanup
            server.close();
            await Task.Delay(500); // Give server time to close
        }
    }

    [Fact]
    public async Task Server_CustomHeaders_ReturnsCorrectHeaders()
    {
        // Arrange
        var port = 18081;

        var server = http.createServer((req, res) =>
        {
            res.writeHead(200, new System.Collections.Generic.Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "X-Custom-Header", "test-value" }
            });

            res.end("{\"status\":\"ok\"}").Wait();
        });

        server.listen(port, (Action?)null);

        try
        {
            // Act
            using var client = new HttpClient();
            var response = await client.GetAsync($"http://localhost:{port}/");

            // Assert
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
            Assert.True(response.Headers.Contains("X-Custom-Header"));
            Assert.Equal("test-value", response.Headers.GetValues("X-Custom-Header").First());
        }
        finally
        {
            // Cleanup
            server.close();
            await Task.Delay(500);
        }
    }

    [Fact]
    public async Task Server_RequestHeaders_AreAccessible()
    {
        // Arrange
        var port = 18082;
        string? receivedUserAgent = null;

        var server = http.createServer((req, res) =>
        {
            receivedUserAgent = req.headers.GetValueOrDefault("user-agent");
            res.end("OK").Wait();
        });

        server.listen(port, (Action?)null);

        try
        {
            // Act
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "TestAgent/1.0");
            await client.GetAsync($"http://localhost:{port}/");

            // Assert
            Assert.NotNull(receivedUserAgent);
            Assert.Contains("TestAgent/1.0", receivedUserAgent);
        }
        finally
        {
            // Cleanup
            server.close();
            await Task.Delay(500);
        }
    }

    [Fact]
    public void Server_Listen_SetsListeningProperty()
    {
        // Arrange
        var port = 18083;
        var server = http.createServer((req, res) => res.end("OK").Wait());

        // Assert before
        Assert.False(server.listening);

        // Act
        server.listen(port, (Action?)null);

        try
        {
            // Assert after
            Assert.True(server.listening);
        }
        finally
        {
            // Cleanup
            server.close();
        }
    }

    [Fact]
    public async Task Server_Close_StopsAcceptingConnections()
    {
        // Arrange
        var port = 18084;
        var server = http.createServer((req, res) => res.end("OK").Wait());

        server.listen(port, (Action?)null);

        // Act - Close server
        server.close();
        await Task.Delay(500); // Give server time to close

        // Assert - Connection should fail
        using var client = new HttpClient();
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            await client.GetAsync($"http://localhost:{port}/");
        });
    }
}
