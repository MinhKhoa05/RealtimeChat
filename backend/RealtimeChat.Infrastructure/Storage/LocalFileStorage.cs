using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Infrastructure.Storage;

public class LocalFileStorage : IFileStorage
{
    private readonly string _rootPath;

    public LocalFileStorage(string rootPath)
    {
        _rootPath = rootPath;
        Directory.CreateDirectory(_rootPath);
    }

    public async Task SaveAsync(Stream content, string storageKey, CancellationToken ct)
    {
        var filePath = Path.Combine(_rootPath, storageKey);

        await using var stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);

        await content.CopyToAsync(stream, ct);
    }

    public Task DeleteAsync(string storageKey, CancellationToken ct)
    {
        var filePath = Path.Combine(_rootPath, storageKey);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}