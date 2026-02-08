using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace BookstoreApi.Tests;

public class ApplicationStartupTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApplicationStartupTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Application_Starts_And_Responds()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/books");

        // No controllers registered yet, so 404 is expected.
        // The key assertion is that the app starts without throwing.
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
