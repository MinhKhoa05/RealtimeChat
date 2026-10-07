using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class StickerCollection : BaseEntity
{
    public long OwnerId { get; private set; }
    public User Owner { get; private set; } = null!;

    public string Label { get; private set; } = null!;
    public Guid SharedKey { get; private set; }

    public StickerCollectionVisibility Visibility { get; private set; }

    public ICollection<Sticker> Stickers { get; private set; } = new List<Sticker>();

    private StickerCollection() { }

    public static StickerCollection Create(long ownerId, string label)
    {
        ValidateLabel(label);

        return new StickerCollection
        {
            OwnerId = ownerId,
            Label = label.Trim(),
            SharedKey = Guid.NewGuid(),
            Visibility = StickerCollectionVisibility.Private,
        };
    }

    public void Publish()
    {
        Visibility = StickerCollectionVisibility.Public;
    }

    public void Unpublish()
    {
        Visibility = StickerCollectionVisibility.Private;
    }

    public void RegenerateSharedKey()
    {
        SharedKey = Guid.NewGuid();
    }

    public void UpdateLable(string newLabel)
    {
        ValidateLabel(newLabel);

        Label = newLabel.Trim();
    }

    public bool IsPublic()
    {
        return Visibility == StickerCollectionVisibility.Public;
    }

    private static void ValidateLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            throw new DomainException("Label is required.");
        }
    }
}