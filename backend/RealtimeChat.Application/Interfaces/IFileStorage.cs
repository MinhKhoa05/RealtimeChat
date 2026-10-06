namespace RealtimeChat.Application.Interfaces;

public interface IFileStorage
{
    Task<string> SaveAsync(Stream content, string contentType, CancellationToken ct);
    Task<Stream> OpenAsync(string storageKey, CancellationToken ct);
    Task DeleteAsync(string storageKey, CancellationToken ct);
}