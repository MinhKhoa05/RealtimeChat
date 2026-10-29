import { useState } from "react";
import { Search, UserPlus, Users, Filter } from "lucide-react";
import { cn } from "@/lib/utils";
import type { ChatItem, ChatFilter } from "../data/mockData";
import { PixelBlockIcon } from "./PixelBlockIcon";

type ChatListProps = {
  chats: ChatItem[];
  selectedChatId: string;
  onSelectChat: (chat: ChatItem) => void;
};

export function ChatList({ chats, selectedChatId, onSelectChat }: ChatListProps) {
  const [filter, setFilter] = useState<ChatFilter>("all");
  const [searchQuery, setSearchQuery] = useState("");

  const filteredChats = chats.filter((chat) => {
    if (searchQuery.trim() && !chat.name.toLowerCase().includes(searchQuery.toLowerCase())) {
      return false;
    }
    if (filter === "groups") return chat.type === "group";
    if (filter === "friends") return chat.type === "friend";
    return true;
  });

  return (
    <section className="flex h-full w-[310px] shrink-0 flex-col border-r border-[#dbe6f5] bg-[#ffffff] select-none">
      {/* Top Search & Actions */}
      <div className="flex items-center gap-2 p-3.5 border-b border-[#f1f5f9]">
        <div className="relative flex flex-1 items-center rounded-xl bg-[#f0f4f9] px-3 py-2 transition-all duration-150 focus-within:bg-white focus-within:ring-2 focus-within:ring-[#0052cc]/20 focus-within:border-[#0052cc] border border-transparent">
          <Search className="size-4 shrink-0 text-slate-400" />
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="Tìm kiếm cuộc trò chuyện..."
            className="ml-2 w-full bg-transparent text-xs text-slate-800 placeholder:text-slate-400 outline-none"
          />
        </div>
        <button
          type="button"
          aria-label="Thêm bạn bè"
          className="grid size-8.5 shrink-0 place-items-center rounded-xl text-slate-600 hover:bg-[#eef3fb] hover:text-[#0052cc] transition duration-150"
          title="Thêm bạn mới"
        >
          <UserPlus className="size-4.5" />
        </button>
        <button
          type="button"
          aria-label="Tạo nhóm"
          className="grid size-8.5 shrink-0 place-items-center rounded-xl text-slate-600 hover:bg-[#eef3fb] hover:text-[#0052cc] transition duration-150"
          title="Tạo nhóm chat"
        >
          <Users className="size-4.5" />
        </button>
      </div>

      {/* Filter Tabs Bar (Tất cả, Nhóm, Cá nhân) */}
      <div className="flex items-center justify-between px-3.5 py-2.5 border-b border-[#f1f5f9] text-xs font-semibold">
        <div className="flex items-center gap-1.5">
          <button
            type="button"
            onClick={() => setFilter("all")}
            className={cn(
              "rounded-lg px-2.5 py-1 transition-all duration-150",
              filter === "all"
                ? "bg-[#e5efff] text-[#0052cc] font-bold"
                : "text-slate-500 hover:bg-slate-100 hover:text-slate-800",
            )}
          >
            Tất cả
          </button>
          <button
            type="button"
            onClick={() => setFilter("groups")}
            className={cn(
              "rounded-lg px-2.5 py-1 transition-all duration-150",
              filter === "groups"
                ? "bg-[#e5efff] text-[#0052cc] font-bold"
                : "text-slate-500 hover:bg-slate-100 hover:text-slate-800",
            )}
          >
            Nhóm
          </button>
          <button
            type="button"
            onClick={() => setFilter("friends")}
            className={cn(
              "rounded-lg px-2.5 py-1 transition-all duration-150",
              filter === "friends"
                ? "bg-[#e5efff] text-[#0052cc] font-bold"
                : "text-slate-500 hover:bg-slate-100 hover:text-slate-800",
            )}
          >
            Cá nhân
          </button>
        </div>

        <button
          type="button"
          className="flex items-center gap-1 text-[11px] font-medium text-slate-400 hover:text-[#0052cc] transition"
          title="Bộ lọc nâng cao"
        >
          <Filter className="size-3.5" />
          <span>Lọc</span>
        </button>
      </div>

      {/* Conversations List */}
      <div className="flex-1 overflow-y-auto px-2 py-1 space-y-1">
        {filteredChats.map((chat) => {
          const isSelected = chat.id === selectedChatId;
          return (
            <div
              key={chat.id}
              onClick={() => onSelectChat(chat)}
              className={cn(
                "group relative flex cursor-pointer items-center gap-3 rounded-xl px-3 py-2.5 transition-all duration-150 select-none",
                isSelected
                  ? "bg-[#e8f1fd] text-slate-900"
                  : "hover:bg-slate-50 text-slate-700",
              )}
            >
              {/* Selected indicator notch on the left */}
              {isSelected && (
                <div className="absolute left-0 top-2 bottom-2 w-1 rounded-r-md bg-[#0052cc]" />
              )}

              {/* Avatar */}
              <div className="relative shrink-0">
                <PixelBlockIcon
                  type={chat.avatarType}
                  className="size-11 rounded-xl shadow-2xs"
                />
                {chat.online && (
                  <span className="absolute -bottom-0.5 -right-0.5 size-3 rounded-full border-2 border-white bg-emerald-500" />
                )}
              </div>

              {/* Text Info */}
              <div className="min-w-0 flex-1">
                <div className="flex items-center justify-between">
                  <h3
                    className={cn(
                      "truncate text-sm font-semibold tracking-tight",
                      isSelected ? "text-slate-950 font-bold" : "text-slate-800",
                    )}
                  >
                    {chat.name}
                  </h3>
                  <span className="text-[11px] font-medium text-slate-400">
                    {chat.time}
                  </span>
                </div>
                <div className="mt-1 flex items-center justify-between">
                  <p className="truncate text-xs text-slate-500">
                    {chat.lastMessage}
                  </p>
                  {chat.unreadCount && (
                    <span className="flex size-4.5 shrink-0 items-center justify-center rounded-full bg-rose-500 text-[10px] font-bold text-white shadow-2xs">
                      {chat.unreadCount}
                    </span>
                  )}
                </div>
              </div>
            </div>
          );
        })}
      </div>
    </section>
  );
}
