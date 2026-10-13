
import { MessageBubble } from "./MessageBubble";

export type Message = {
  id: string;
  senderId: string;
  senderName: string;
  content: string;
  sentAt: string;
  isRead?: boolean;
};

type MessageListProps = {
  messages: Message[];
  currentUserId: string;
};

function formatTime(sentAt: string) {
  return new Date(sentAt).toLocaleTimeString("vi-VN", {
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  });
}

export function MessageList({
  messages,
  currentUserId,
}: MessageListProps) {
  return (
    <div className="flex min-h-0 flex-1 flex-col gap-1 overflow-y-auto bg-[#fffefa] px-4 py-5 sm:px-5">
      {messages.length === 0 ? (
        <div className="flex flex-1 items-center justify-center text-sm text-[#8a8379]">
          Chưa có tin nhắn. Hãy bắt đầu trò chuyện nhé!
        </div>
      ) : (
        messages.map((message, index) => {
          const isMine = message.senderId === currentUserId;
          const nextMessage = messages[index + 1];

          const isLastInGroup =
            !nextMessage ||
            nextMessage.senderId !== message.senderId;

          return (
            <div
              key={message.id}
              className={isLastInGroup ? "mb-1" : ""}
            >
              {!isMine &&
                (index === 0 ||
                  messages[index - 1].senderId !==
                    message.senderId) && (
                  <p className="mb-1 ml-1 text-xs font-medium text-[#6f6b65]">
                    {message.senderName}
                  </p>
                )}

              <MessageBubble
                content={message.content}
                time={formatTime(message.sentAt)}
                isMine={isMine}
                isRead={message.isRead}
                showMetadata={isLastInGroup}
              />
            </div>
          );
        })
      )}
    </div>
  );
}
