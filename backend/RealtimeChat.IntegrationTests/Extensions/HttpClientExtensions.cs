using System.Net.Http.Json;
using System.Text.Json;
using RealtimeChat.Api;

namespace RealtimeChat.IntegrationTests.Extensions;

public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static Task<HttpResponseMessage> PostJsonAsync<T>(
        this HttpClient client,
        string requestUri,
        T request,
        CancellationToken cancellationToken = default)
    {
        return client.PostAsJsonAsync(requestUri, request, JsonOptions, cancellationToken);
    }

    public static Task<HttpResponseMessage> PutJsonAsync<T>(
        this HttpClient client,
        string requestUri,
        T request,
        CancellationToken cancellationToken = default)
    {
        return client.PutAsJsonAsync(requestUri, request, JsonOptions, cancellationToken);
    }

    public static async Task<HttpResponseMessage> PostMultipartAsync(
        this HttpClient client,
        string requestUri,
        IReadOnlyDictionary<string, string> fields,
        HttpContent fileContent,
        string fileFieldName = "imageFile",
        string fileName = "image.png",
        CancellationToken cancellationToken = default)
    {
        using var content = CreateMultipartContent(fields, fileContent, fileFieldName, fileName);
        return await client.PostAsync(requestUri, content, cancellationToken);
    }

    public static async Task<HttpResponseMessage> PutMultipartAsync(
        this HttpClient client,
        string requestUri,
        IReadOnlyDictionary<string, string> fields,
        HttpContent fileContent,
        string fileFieldName = "imageFile",
        string fileName = "image.png",
        CancellationToken cancellationToken = default)
    {
        using var content = CreateMultipartContent(fields, fileContent, fileFieldName, fileName);
        return await client.PutAsync(requestUri, content, cancellationToken);
    }

    public static async Task<ApiResponse> ReadApiResponseAsync(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return await response.Content.ReadFromJsonAsync<ApiResponse>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("Response body was empty.");
    }

    public static async Task<ApiResponse<T>> ReadApiResponseAsync<T>(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        return await response.Content.ReadFromJsonAsync<ApiResponse<T>>(JsonOptions, cancellationToken)
            ?? throw new InvalidOperationException("Response body was empty.");
    }

    public static IReadOnlyList<string> GetSetCookieHeaders(this HttpResponseMessage response)
    {
        return response.Headers.TryGetValues("Set-Cookie", out var values)
            ? values.ToList()
            : [];
    }

    public static string? ExtractCookieValue(this IEnumerable<string> setCookieHeaders, string cookieName)
    {
        var cookie = setCookieHeaders.FirstOrDefault(header =>
            header.StartsWith($"{cookieName}=", StringComparison.OrdinalIgnoreCase));

        if (cookie is null)
        {
            return null;
        }

        var value = cookie.Split(';', 2)[0];
        var index = value.IndexOf('=');

        return index >= 0 ? value[(index + 1)..] : null;
    }

    public static void ReplaceCookieHeader(this HttpClient client, params string[] cookies)
    {
        client.DefaultRequestHeaders.Remove("Cookie");

        if (cookies.Length > 0)
        {
            client.DefaultRequestHeaders.Add("Cookie", string.Join("; ", cookies));
        }
    }

    private static MultipartFormDataContent CreateMultipartContent(
        IReadOnlyDictionary<string, string> fields,
        HttpContent fileContent,
        string fileFieldName,
        string fileName)
    {
        var content = new MultipartFormDataContent();

        foreach (var field in fields)
        {
            content.Add(new StringContent(field.Value), field.Key);
        }

        content.Add(fileContent, fileFieldName, fileName);
        return content;
    }
}
