using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class Sticker : BaseEntity
{
    public long CollectionId { get; private set; }
    public StickerCollection Collection { get; private set; } = null!;

    public string Label { get; private set; } = null!;

    public long MediaId { get; private set; }
    public Media Media { get; private set; } = null!;

    private Sticker() { }

    public static Sticker Create(long collectionId, string label, long mediaId)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            throw new DomainException("Lable is required.");
        }

        return new Sticker
        {
            CollectionId = collectionId,
            Label = label.Trim(),
            MediaId = mediaId,
        };
    }
}