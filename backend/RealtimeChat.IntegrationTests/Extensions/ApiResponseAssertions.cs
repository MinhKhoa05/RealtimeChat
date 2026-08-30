using System.Net;
using FluentAssertions;
using RealtimeChat.BLL.Exceptions;

namespace RealtimeChat.IntegrationTests.Extensions;

public static class ApiResponseAssertions
{
    public static async Task<T> ShouldBeSuccess<T>(
        this HttpResponseMessage response,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        CancellationToken cancellationToken = default)
    {
        response.StatusCode.Should().Be(statusCode);
        var body = await response.ReadApiResponseAsync<T>(cancellationToken);
        body.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
        return body.Data!;
    }

    public static async Task ShouldBeSuccess(
        this HttpResponseMessage response,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        CancellationToken cancellationToken = default)
    {
        response.StatusCode.Should().Be(statusCode);
        var body = await response.ReadApiResponseAsync<object>(cancellationToken);
        body.Success.Should().BeTrue();
    }

    public static async Task ShouldHaveError(
        this HttpResponseMessage response,
        HttpStatusCode statusCode,
        ErrorCode errorCode,
        CancellationToken cancellationToken = default)
    {
        response.StatusCode.Should().Be(statusCode);
        var body = await response.ReadApiResponseAsync<object>(cancellationToken);
        body.Success.Should().BeFalse();
        body.ErrorCode.Should().Be(errorCode);
    }


    public static async Task ShouldHaveError(
        this HttpResponseMessage response,
        HttpStatusCode statusCode,
        CancellationToken cancellationToken = default)
    {
        response.StatusCode.Should().Be(statusCode);
        var body = await response.ReadApiResponseAsync<object>(cancellationToken);
        body.Success.Should().BeFalse();
    }
}
