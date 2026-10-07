using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class Sticker : BaseEntity
{
    public long CollectionId { get; private set; }
    public StickerCollection Collection { get; private set; } = null!;

    public string Label { get; private set; } = null!;
    public Guid PublicId { get; private set; }

    public long MediaId { get; private set; }
    public Media Media { get; private set; } = null!;

    
    private Sticker() { }

    public static Sticker Create(long collectionId, string label, Media media)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            throw new DomainException("Lable is required.");
        }

        return new Sticker
        {
            CollectionId = collectionId,
            Label = label.Trim(),
            PublicId = Guid.NewGuid(),
            MediaId = media.Id,
            Media = media
        };
    }

    public void UpdateLabel(string newLabel)
    {
        if (string.IsNullOrWhiteSpace(newLabel))
        {
            throw new DomainException("Lable is required.");
        }

        Label = newLabel.Trim();
    }
}