using FluentAssertions;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;
using RealtimeChat.Domain.Factories;

namespace RealtimeChat.UnitTests.Domain;

public class MessageTests
{
    private const long DefaultConversationId = 1L;
    private const long DefaultOtherConversationId = 2L;
    private const long DefaultSenderId = 10L;
    private const long DefaultOtherSenderId = 20L;
    private const long DefaultMentionUserId = 30L;
    private const long DefaultUnauthorizedUserId = 99L;

    private static Message CreateDefault(
        long conversationId = DefaultConversationId,
        long senderId = DefaultSenderId,
        string content = "Hello")
        => Message.CreateText(conversationId, senderId, content);

    [Fact]
    public void CreateText_WithValidInput_CreatesTextMessage()
    {
        var message = CreateDefault();

        message.ConversationId.Should().Be(DefaultConversationId);
        message.SenderId.Should().Be(DefaultSenderId);
        message.Type.Should().Be(MessageType.Text);
        message.Content.Should().Be("Hello");
        message.MediaId.Should().BeNull();
        message.CallId.Should().BeNull();
        message.StickerId.Should().BeNull();
        message.ReplyToMessageId.Should().BeNull();
        message.RecalledAt.Should().BeNull();
        message.Mentions.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void CreateText_WhenContentIsNullOrWhiteSpace_ThrowsDomainException(string? content)
    {
        var act = () => CreateDefault(content: content!);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CreateText_WithMentionTokens_AddsDistinctMentionsPresentInContent()
    {
        var content = "Hello <@30> <@30>";
        var mentionUserIds = new List<long> { 30, 30, 20 };

        var message = Message.CreateText(
            DefaultConversationId, DefaultSenderId, content, mentionUserIds);

        message.Mentions.Should().ContainSingle(x => x.UserId == DefaultMentionUserId);
    }

    [Fact]
    public void CreateText_WithoutMentionTokens_DoesNotAddMentions()
    {
        var message = Message.CreateText(
            DefaultConversationId, DefaultSenderId, "Hello", [DefaultMentionUserId]);

        message.Mentions.Should().BeEmpty();
    }

    [Fact]
    public void CreateText_WithReplyToMessage_SetsReply()
    {
        var originalMessage = CreateDefault();

        var message = Message.CreateText(
            DefaultConversationId, DefaultOtherSenderId, "Reply",
            replyToMessage: originalMessage);

        message.ReplyToMessage.Should().BeSameAs(originalMessage);
        message.ReplyToMessageId.Should().Be(originalMessage.Id);
    }

    [Fact]
    public void CreateText_WhenReplyIsInAnotherConversation_ThrowsDomainException()
    {
        var originalMessage = CreateDefault(conversationId: DefaultOtherConversationId);

        var act = () => Message.CreateText(
            DefaultConversationId, DefaultSenderId, "Reply",
            replyToMessage: originalMessage);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CreateText_WhenReplyingToSystemMessage_ThrowsDomainException()
    {
        var systemMessage = SystemMessageFactory.CreateMemberJoined(DefaultConversationId, DefaultSenderId);

        var act = () => Message.CreateText(
            DefaultConversationId, DefaultOtherSenderId, "Reply",
            replyToMessage: systemMessage);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CreateMedia_WithValidInput_CreatesMediaMessage()
    {
        var media = Media.Create("image.png", "image/png", 1024, ".png");

        var message = Message.CreateMedia(DefaultConversationId, DefaultSenderId, media);

        message.ConversationId.Should().Be(DefaultConversationId);
        message.SenderId.Should().Be(DefaultSenderId);
        message.Type.Should().Be(MessageType.Media);
        message.MediaId.Should().Be(media.Id);
        message.Media.Should().BeSameAs(media);
        message.Content.Should().BeNull();
    }

    [Fact]
    public void CreateCall_WithValidInput_CreatesCallMessage()
    {
        var call = Call.Create(DefaultSenderId, DefaultOtherSenderId, CallType.Audio);

        var message = Message.CreateCall(DefaultConversationId, call);

        message.ConversationId.Should().Be(DefaultConversationId);
        message.SenderId.Should().Be(call.CallerId);
        message.Type.Should().Be(MessageType.Call);
        message.CallId.Should().Be(call.Id);
        message.Call.Should().BeSameAs(call);
        message.Content.Should().BeNull();
    }

    [Fact]
    public void Recall_WhenActorIsSender_RecallsTextMessageAndClearsMentions()
    {
        var message = Message.CreateText(
            DefaultConversationId,
            DefaultSenderId,
            $"Hello {Message.MentionToken(DefaultMentionUserId)}",
            [DefaultMentionUserId]);
        var now = message.CreatedAt.AddMinutes(5);

        var result = message.Recall(DefaultSenderId, now);

        result.Should().BeTrue();
        message.Content.Should().BeNull();
        message.RecalledAt.Should().Be(now);
        message.Mentions.Should().BeEmpty();
    }

    [Fact]
    public void Recall_WhenMessageIsAtExactRecallWindowBoundary_RecallsMessage()
    {
        var message = CreateDefault();
        var exactBoundary = message.CreatedAt.AddMinutes(15);

        var result = message.Recall(DefaultSenderId, exactBoundary);

        result.Should().BeTrue();
        message.RecalledAt.Should().Be(exactBoundary);
    }

    [Fact]
    public void Recall_WhenMessageIsMedia_RecallsMessage()
    {
        var media = Media.Create("image.png", "image/png", 1024, ".png");
        var mediaMessage = Message.CreateMedia(DefaultConversationId, DefaultSenderId, media);
        var now = mediaMessage.CreatedAt.AddMinutes(1);

        var result = mediaMessage.Recall(DefaultSenderId, now);

        result.Should().BeTrue();
        mediaMessage.RecalledAt.Should().Be(now);
    }

    [Fact]
    public void Recall_WhenActorIsNotSender_ThrowsForbiddenDomainException()
    {
        var message = CreateDefault();

        var act = () => message.Recall(DefaultUnauthorizedUserId, message.CreatedAt.AddMinutes(1));

        act.Should().Throw<ForbiddenDomainException>();
    }

    [Fact]
    public void Recall_WhenAlreadyRecalled_ReturnsFalseAndPreservesRecalledAt()
    {
        var message = CreateDefault();
        var firstRecallTime = message.CreatedAt.AddMinutes(1);
        message.Recall(DefaultSenderId, firstRecallTime);

        var result = message.Recall(DefaultSenderId, firstRecallTime.AddMinutes(2));

        result.Should().BeFalse();
        message.RecalledAt.Should().Be(firstRecallTime);
    }

    [Fact]
    public void Recall_WhenOutsideRecallWindow_ThrowsDomainException()
    {
        var message = CreateDefault();
        var outsideWindow = message.CreatedAt.AddMinutes(15).AddTicks(1);

        var act = () => message.Recall(DefaultSenderId, outsideWindow);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Recall_WhenMessageIsCall_ThrowsDomainException()
    {
        var call = Call.Create(DefaultSenderId, DefaultOtherSenderId, CallType.Audio);
        var message = Message.CreateCall(DefaultConversationId, call);

        var act = () => message.Recall(DefaultSenderId, message.CreatedAt.AddMinutes(1));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Recall_WhenMessageIsSystem_ThrowsForbiddenDomainException()
    {
        var systemMessage = SystemMessageFactory.CreateMemberJoined(DefaultConversationId, DefaultSenderId);

        var act = () => systemMessage.Recall(DefaultSenderId, systemMessage.CreatedAt.AddMinutes(1));

        act.Should().Throw<ForbiddenDomainException>();
    }

    [Fact]
    public void MentionToken_WithUserId_ReturnsExpectedToken()
    {
        Message.MentionToken(DefaultMentionUserId).Should().Be("<@30>");
    }
}
