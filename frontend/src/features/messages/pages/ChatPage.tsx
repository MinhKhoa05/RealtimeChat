
import { useState } from "react";
import { MoreHorizontal, Phone, Video } from "lucide-react";

import {
  MessageList,
  type Message,
} from "../components/MessageList";
import { MessageInput } from "../components/MessageInput";

const currentUserId = "user-1";

const initialMessages: Message[] = [
  {
    id: "1",
    senderId: "user-2",
    senderName: "Alice Nguyen",
    content: "Hey Khoa! 👋",
    sentAt: "2026-10-13T14:20:00",
  },
  {
    id: "2",
    senderId: "user-2",
    senderName: "Alice Nguyen",
    content: "Em xem giao diện mới chưa?",
    sentAt: "2026-10-13T14:20:30",
  },
  {
    id: "3",
    senderId: currentUserId,
    senderName: "Khoa",
    content: "Rồi em, nhìn khá ổn đó!",
    sentAt: "2026-10-13T14:21:00",
    isRead: true,
  },
  {
    id: "4",
    senderId: currentUserId,
    senderName: "Khoa",
    content: "Anh đang tích hợp các component lại.",
    sentAt: "2026-10-13T14:21:30",
    isRead: true,
  },
  {
    id: "5",
    senderId: "user-2",
    senderName: "Alice Nguyen",
    content: "Oke, khi nào xong báo em nhé 😊",
    sentAt: "2026-10-13T14:22:00",
  },
];

export default function ChatPage() {
  const [messages, setMessages] = useState(initialMessages);

  const handleSend = (content: string) => {
    const newMessage: Message = {
      id: crypto.randomUUID(),
      senderId: currentUserId,
      senderName: "Khoa",
      content,
      sentAt: new Date().toISOString(),
      isRead: false,
    };

    setMessages((previous) => [...previous, newMessage]);
  };

  return (
    <main className="flex h-dvh w-full items-center justify-center bg-[#f7f5f1] p-2 sm:p-4">
      <section className="flex h-full w-full max-w-5xl flex-col overflow-hidden rounded-2xl border border-[#e7e2da] bg-[#fffdf9] shadow-sm">
        <header className="flex h-17.5 shrink-0 items-center justify-between border-b border-[#e7e2da] px-4 sm:px-5">
          <div className="flex min-w-0 items-center gap-3">
            <div className="relative flex size-11 shrink-0 items-center justify-center rounded-full bg-[#f2e8d7] text-xl">
              👩🏻
              <span className="absolute bottom-0 right-0 size-3 rounded-full border-2 border-[#fffdf9] bg-emerald-500" />
            </div>

            <div className="min-w-0">
              <h1 className="truncate text-base font-semibold text-[#171717]">
                Alice Nguyen
              </h1>
              <p className="mt-0.5 text-xs text-emerald-600">
                ● Online
              </p>
            </div>
          </div>

          <div className="flex shrink-0 items-center gap-1.5">
            <button
              type="button"
              aria-label="Voice call"
              className="grid size-10 place-items-center rounded-xl text-[#6f6b65] transition hover:bg-[#f2e8d7]"
            >
              <Phone size={19} />
            </button>

            <button
              type="button"
              aria-label="Video call"
              className="grid size-10 place-items-center rounded-xl text-[#6f6b65] transition hover:bg-[#f2e8d7]"
            >
              <Video size={20} />
            </button>

            <button
              type="button"
              aria-label="More options"
              className="grid size-10 place-items-center rounded-xl text-[#6f6b65] transition hover:bg-[#f2e8d7]"
            >
              <MoreHorizontal size={21} />
            </button>
          </div>
        </header>

        <MessageList
          messages={messages}
          currentUserId={currentUserId}
        />

        <MessageInput onSend={handleSend} />
      </section>
    </main>
  );
}
