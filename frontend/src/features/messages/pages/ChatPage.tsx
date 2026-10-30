import { useState } from "react";
import { SidebarLeft } from "@/features/navigation/components/SidebarLeft";
import type { NavTab } from "@/features/navigation/types";
import { ConversationList } from "@/features/conversations/components/ConversationList";
import {
  sampleConversations,
  type ConversationItem,
} from "@/features/conversations/data/conversationsData";
import { ChatArea } from "@/features/messages/components/ChatArea";
import {
  sampleChatMessages,
  type ChatMessage,
} from "@/features/messages/data/messagesData";
import { ConversationDetails } from "@/features/details/components/ConversationDetails";
import { FriendsView } from "@/features/friends/components/FriendsView";
import type { FriendItem, GroupItem } from "@/features/friends/data/friendsData";
import { PostsView } from "@/features/posts/components/PostsView";
import { StickersView } from "@/features/stickers/components/StickersView";
import { ProfileModal } from "@/features/profile/components/ProfileModal";
import {
  initialUserProfile,
  type UserProfile,
} from "@/features/profile/data/profileData";

interface ChatPageProps {
  onLogout?: () => void;
}

export default function ChatPage({ onLogout }: ChatPageProps) {
  const [activeNav, setActiveNav] = useState<NavTab>("messages");
  const [conversations, setConversations] = useState<ConversationItem[]>(sampleConversations);
  const [selectedConversationId, setSelectedConversationId] = useState<string>("team-project");
  const [messages, setMessages] = useState<ChatMessage[]>(sampleChatMessages);
  const [showRightSidebar, setShowRightSidebar] = useState<boolean>(true);

  // Profile modal state
  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const [userProfile, setUserProfile] = useState<UserProfile>(initialUserProfile);

  const activeConversation =
    conversations.find((c) => c.id === selectedConversationId) || conversations[0];

  const handleSelectConversation = (conversation: ConversationItem) => {
    setSelectedConversationId(conversation.id);
  };

  const handleSendMessage = (text: string) => {
    const now = new Date();
    const timeStr = `${String(now.getHours()).padStart(2, "0")}:${String(
      now.getMinutes(),
    ).padStart(2, "0")}`;

    const newMsg: ChatMessage = {
      id: `msg-${Date.now()}`,
      senderId: "khoa",
      senderName: userProfile.displayName,
      avatarType: userProfile.avatarType,
      content: text,
      time: timeStr,
      isMine: true,
      isRead: false,
    };

    setMessages((prev) => [...prev, newMsg]);

    setConversations((prev) =>
      prev.map((c) =>
        c.id === selectedConversationId
          ? {
              ...c,
              lastMessage: text,
              time: timeStr,
            }
          : c,
      ),
    );
  };

  // Start chat with a friend from Friends View
  const handleStartChatWithFriend = (friend: FriendItem) => {
    const existing = conversations.find(
      (c) => c.name.toLowerCase() === friend.name.toLowerCase(),
    );
    if (existing) {
      setSelectedConversationId(existing.id);
    } else {
      const newConversation: ConversationItem = {
        id: `chat-${friend.id}`,
        name: friend.name,
        avatarType: friend.avatarType,
        lastMessage: "Bắt đầu cuộc trò chuyện mới",
        time: "Vừa xong",
        type: "friend",
        online: friend.online,
      };
      setConversations((prev) => [newConversation, ...prev]);
      setSelectedConversationId(newConversation.id);
    }
    setActiveNav("messages");
  };

  // Open group chat from Group List
  const handleOpenGroupChat = (group: GroupItem) => {
    const existing = conversations.find(
      (c) => c.name.toLowerCase() === group.name.toLowerCase(),
    );
    if (existing) {
      setSelectedConversationId(existing.id);
    } else {
      const newConversation: ConversationItem = {
        id: `chat-${group.id}`,
        name: group.name,
        avatarType: group.avatarType,
        lastMessage: "Chào mừng bạn đến với nhóm",
        time: "Vừa xong",
        type: "group",
      };
      setConversations((prev) => [newConversation, ...prev]);
      setSelectedConversationId(newConversation.id);
    }
    setActiveNav("messages");
  };

  return (
    <div className="flex h-screen w-screen overflow-hidden bg-slate-100 select-none">
      {/* Column 1: Navigation Sidebar (72px) */}
      <SidebarLeft
        activeNav={activeNav}
        onSelectNav={setActiveNav}
        onOpenProfile={() => setIsProfileOpen(true)}
        onLogout={onLogout}
      />

      {/* Dynamic Content: When "connect", "posts", or "stickers" is selected */}
      {activeNav === "connect" ? (
        <FriendsView
          key="connect-view"
          initialSubTab="friends_list"
          onStartChatWithFriend={handleStartChatWithFriend}
          onOpenGroupChat={handleOpenGroupChat}
        />
      ) : activeNav === "posts" ? (
        <PostsView key="posts-view" />
      ) : activeNav === "stickers" ? (
        <StickersView key="stickers-view" />
      ) : (
        /* Regular Messaging Columns */
        <>
          {/* Column 2: Conversations List (310px) */}
          <ConversationList
            conversations={conversations}
            selectedConversationId={selectedConversationId}
            onSelectConversation={handleSelectConversation}
          />

          {/* Column 3: Chat Area (Flex 1) */}
          <ChatArea
            activeConversation={activeConversation}
            messages={messages}
            onSendMessage={handleSendMessage}
            onToggleRightSidebar={() => setShowRightSidebar(!showRightSidebar)}
            showRightSidebar={showRightSidebar}
          />

          {/* Column 4: Conversation Details (310px) */}
          {showRightSidebar && <ConversationDetails />}
        </>
      )}

      {/* Simple Zalo-style Profile Modal */}
      <ProfileModal
        isOpen={isProfileOpen}
        onClose={() => setIsProfileOpen(false)}
        profile={userProfile}
        onUpdateProfile={(updated) => setUserProfile(updated)}
      />
    </div>
  );
}
