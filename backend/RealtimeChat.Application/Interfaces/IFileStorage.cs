namespace RealtimeChat.Application.Interfaces;

public interface IFileStorage
{
    Task SaveAsync(Stream content, string storageKey, CancellationToken ct);
    Task DeleteAsync(string storageKey, CancellationToken ct);
}