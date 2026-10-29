import { useState } from "react";
import { SidebarLeft } from "../components/SidebarLeft";
import { ChatList } from "../components/ChatList";
import { ChatArea } from "../components/ChatArea";
import { SidebarRight } from "../components/SidebarRight";
import {
  sampleChats,
  sampleChatMessages,
} from "../data/mockData";
import type {
  ChatItem,
  ChatMessage,
} from "../data/mockData";

export default function ChatPage() {
  const [chats, setChats] = useState<ChatItem[]>(sampleChats);
  const [selectedChatId, setSelectedChatId] = useState<string>("team-project");
  const [messages, setMessages] = useState<ChatMessage[]>(sampleChatMessages);
  const [showRightSidebar, setShowRightSidebar] = useState<boolean>(true);

  const activeChat =
    chats.find((c) => c.id === selectedChatId) || chats[0];

  const handleSelectChat = (chat: ChatItem) => {
    setSelectedChatId(chat.id);
  };

  const handleSendMessage = (text: string) => {
    const now = new Date();
    const timeStr = `${String(now.getHours()).padStart(2, "0")}:${String(
      now.getMinutes(),
    ).padStart(2, "0")}`;

    const newMsg: ChatMessage = {
      id: `msg-${Date.now()}`,
      senderId: "khoa",
      senderName: "Khoa",
      avatarType: "user_khoa",
      content: text,
      time: timeStr,
      isMine: true,
      isRead: false,
    };

    setMessages((prev) => [...prev, newMsg]);

    setChats((prev) =>
      prev.map((c) =>
        c.id === selectedChatId
          ? {
              ...c,
              lastMessage: text,
              time: timeStr,
            }
          : c,
      ),
    );
  };

  return (
    <div className="flex h-screen w-screen overflow-hidden bg-slate-100 select-none">
      {/* Column 1: Zalo-style Left Slim Navigation Bar (68px) */}
      <SidebarLeft />

      {/* Column 2: Conversations & Channels List (310px) */}
      <ChatList
        chats={chats}
        selectedChatId={selectedChatId}
        onSelectChat={handleSelectChat}
      />

      {/* Column 3: Main Chat Stream (Flex 1) */}
      <ChatArea
        activeChat={activeChat}
        messages={messages}
        onSendMessage={handleSendMessage}
        onToggleRightSidebar={() => setShowRightSidebar(!showRightSidebar)}
        showRightSidebar={showRightSidebar}
      />

      {/* Column 4: Right Conversation Details Sidebar (300px) */}
      {showRightSidebar && <SidebarRight />}
    </div>
  );
}
