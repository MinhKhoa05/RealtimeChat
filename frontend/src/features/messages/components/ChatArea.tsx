import { useState } from "react";
import {
  Phone,
  Video,
  Search,
  FileText,
  Smile,
  Send,
  Paperclip,
  CheckCheck,
  Download,
  Image as ImageIcon,
  Sidebar,
  MoreVertical,
  Sticker,
  MapPin,
  Navigation,
} from "lucide-react";
import { cn } from "@/lib/utils";
import type { ChatMessage } from "../data/messagesData";
import type { ConversationItem } from "@/features/conversations/data/conversationsData";
import { PixelBlockIcon } from "@/components/PixelBlockIcon";

type ChatAreaProps = {
  activeConversation: ConversationItem;
  messages: ChatMessage[];
  onSendMessage: (text: string) => void;
  onToggleRightSidebar: () => void;
  showRightSidebar: boolean;
};

export function ChatArea({
  activeConversation,
  messages,
  onSendMessage,
  onToggleRightSidebar,
  showRightSidebar,
}: ChatAreaProps) {
  const [inputValue, setInputValue] = useState("");

  const handleSend = () => {
    if (!inputValue.trim()) return;
    onSendMessage(inputValue.trim());
    setInputValue("");
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  return (
    <main className="relative flex h-full flex-1 flex-col overflow-hidden bg-[#edf3fa]">
      {/* Top Header */}
      <header className="flex h-16 shrink-0 items-center justify-between border-b border-[#dbe6f5] bg-white px-5 shadow-2xs z-10">
        <div className="flex items-center gap-3">
          <div className="relative">
            <PixelBlockIcon
              type={activeConversation.avatarType}
              className="size-10 rounded-xl shadow-xs ring-1 ring-black/5"
            />
            {activeConversation.online && (
              <span className="absolute -bottom-0.5 -right-0.5 size-3 rounded-full border-2 border-white bg-emerald-500" />
            )}
          </div>
          <div>
            <h2 className="text-sm font-bold text-slate-900 tracking-tight">
              {activeConversation.name}
            </h2>
            <div className="flex items-center gap-1.5 text-xs text-slate-500">
              <span className="size-2 rounded-full bg-emerald-500" />
              <span>6 thành viên · Đang hoạt động</span>
            </div>
          </div>
        </div>

        {/* Action icons */}
        <div className="flex items-center gap-1 text-slate-600">
          <button
            type="button"
            className="grid size-9 place-items-center rounded-xl hover:bg-[#edf5ff] hover:text-[#0052cc] transition"
            title="Cuộc gọi thoại"
          >
            <Phone className="size-4.5" />
          </button>
          <button
            type="button"
            className="grid size-9 place-items-center rounded-xl hover:bg-[#edf5ff] hover:text-[#0052cc] transition"
            title="Cuộc gọi video"
          >
            <Video className="size-5" />
          </button>
          <button
            type="button"
            className="grid size-9 place-items-center rounded-xl hover:bg-[#edf5ff] hover:text-[#0052cc] transition"
            title="Tìm kiếm tin nhắn"
          >
            <Search className="size-4.5" />
          </button>
          <div className="mx-1 h-5 w-px bg-slate-200" />
          <button
            type="button"
            onClick={onToggleRightSidebar}
            className={cn(
              "grid size-9 place-items-center rounded-xl transition",
              showRightSidebar ? "bg-[#e5efff] text-[#0052cc]" : "hover:bg-slate-100",
            )}
            title="Thông tin hội thoại"
          >
            <Sidebar className="size-4.5" />
          </button>
        </div>
      </header>

      {/* Message Feed */}
      <div className="flex-1 overflow-y-auto p-5 space-y-4">
        {messages.map((msg) => {
          const isMine = msg.isMine;
          return (
            <div key={msg.id} className="flex flex-col">
              {/* Date divider pill */}
              {msg.dateDivider && (
                <div className="my-2.5 flex items-center justify-center">
                  <span className="rounded-full border border-slate-200/60 bg-white px-3.5 py-0.5 text-[11px] font-medium text-slate-500 shadow-2xs">
                    {msg.dateDivider}
                  </span>
                </div>
              )}

              {/* Message Item */}
              <div
                className={cn(
                  "flex items-start gap-2.5 group",
                  isMine ? "justify-end" : "justify-start",
                )}
              >
                {!isMine && (
                  <PixelBlockIcon
                    type={msg.avatarType}
                    className="size-8.5 rounded-xl shrink-0 shadow-2xs mt-0.5 ring-1 ring-black/5"
                  />
                )}

                <div
                  className={cn(
                    "flex flex-col max-w-[65%]",
                    isMine ? "items-end" : "items-start",
                  )}
                >
                  {!isMine && (
                    <span className="mb-1 ml-1 text-xs font-semibold text-slate-600">
                      {msg.senderName}
                    </span>
                  )}

                  {/* Message Bubble or File Card */}
                  {msg.attachment?.type === "file" ? (
                    <div className="flex items-center gap-3 rounded-2xl border border-slate-200/90 bg-white p-3 shadow-xs hover:shadow-sm transition">
                      <div className="flex size-10 shrink-0 items-center justify-center rounded-xl bg-[#0052cc] text-white shadow-2xs">
                        <FileText className="size-5 text-white" />
                      </div>
                      <div className="min-w-0 pr-4">
                        <p className="truncate text-xs font-bold text-slate-800">
                          {msg.attachment.name}
                        </p>
                        <p className="text-[11px] text-slate-400">
                          {msg.attachment.size}
                        </p>
                      </div>
                      <button
                        type="button"
                        className="ml-auto grid size-8 shrink-0 place-items-center rounded-lg text-slate-400 hover:bg-[#edf5ff] hover:text-[#0052cc] transition"
                        title="Tải về"
                      >
                        <Download className="size-4" />
                      </button>
                    </div>
                  ) : (
                    <div
                      className={cn(
                        "relative rounded-2xl px-4 py-2.5 text-sm leading-relaxed shadow-2xs",
                        isMine
                          ? "rounded-tr-xs bg-[#0052cc] text-white"
                          : "rounded-tl-xs bg-white text-slate-800 border border-slate-200/80",
                      )}
                    >
                      <p className="wrap-break-words">{msg.content}</p>
                    </div>
                  )}

                  {/* Timestamp & read receipts */}
                  <div
                    className={cn(
                      "mt-1 flex items-center gap-1.5 px-1 text-[10px] text-slate-400",
                      isMine ? "justify-end" : "justify-start",
                    )}
                  >
                    <span>{msg.time}</span>
                    {isMine && (
                      <CheckCheck className="size-3 text-[#0052cc]" />
                    )}
                  </div>
                </div>

                {isMine && (
                  <PixelBlockIcon
                    type="user_khoa"
                    className="size-8.5 rounded-xl shrink-0 shadow-2xs mt-0.5 ring-1 ring-black/5"
                  />
                )}
              </div>
            </div>
          );
        })}
      </div>

      {/* Input Action Bar */}
      <div className="border-t border-[#dbe6f5] bg-white p-3 z-10">
        {/* Quick Toolbar above input */}
        <div className="flex items-center gap-1 pb-2 text-slate-500">
          <button
            type="button"
            className="grid size-7.5 place-items-center rounded-lg hover:bg-[#edf5ff] hover:text-[#0052cc] transition"
            title="Biểu tượng cảm xúc (Emoji)"
          >
            <Smile className="size-4.5" />
          </button>
          <button
            type="button"
            className="grid size-7.5 place-items-center rounded-lg hover:bg-[#edf5ff] hover:text-[#0052cc] transition"
            title="Nhãn dán (Sticker)"
          >
            <Sticker className="size-4.5" />
          </button>
          <button
            type="button"
            className="grid size-7.5 place-items-center rounded-lg hover:bg-[#edf5ff] hover:text-[#0052cc] transition"
            title="Gửi hình ảnh"
          >
            <ImageIcon className="size-4.5" />
          </button>
          <button
            type="button"
            className="grid size-7.5 place-items-center rounded-lg hover:bg-[#edf5ff] hover:text-[#0052cc] transition"
            title="Đính kèm tập tin"
          >
            <Paperclip className="size-4.5" />
          </button>
          <button
            type="button"
            className="grid size-7.5 place-items-center rounded-lg hover:bg-[#edf5ff] hover:text-[#0052cc] transition"
            title="Chia sẻ vị trí"
          >
            <MapPin className="size-4.5" />
          </button>
          <button
            type="button"
            className="grid size-7.5 place-items-center rounded-lg hover:bg-[#edf5ff] hover:text-[#0052cc] transition"
            title="Hành trình / Lộ trình di chuyển"
          >
            <Navigation className="size-4.5" />
          </button>
          <div className="mx-1 h-4 w-px bg-slate-200" />
          <button
            type="button"
            className="grid size-7.5 place-items-center rounded-lg hover:bg-[#edf5ff] hover:text-[#0052cc] transition"
            title="Thêm tùy chọn khác"
          >
            <MoreVertical className="size-4.5" />
          </button>
        </div>

        {/* Input box and Solid Blue Send button */}
        <div className="flex items-center gap-2.5">
          <div className="flex flex-1 items-center rounded-xl bg-[#f0f4f9] px-3.5 py-2 border border-transparent focus-within:border-[#0052cc] focus-within:bg-white focus-within:ring-2 focus-within:ring-[#0052cc]/20 transition duration-150">
            <input
              type="text"
              value={inputValue}
              onChange={(e) => setInputValue(e.target.value)}
              onKeyDown={handleKeyDown}
              placeholder={`Nhập tin nhắn tới ${activeConversation.name}...`}
              className="w-full bg-transparent text-sm text-slate-800 placeholder:text-slate-400 outline-none"
            />
          </div>

          <button
            type="button"
            onClick={handleSend}
            disabled={!inputValue.trim()}
            aria-label="Gửi tin nhắn"
            className={cn(
              "flex items-center gap-1.5 rounded-xl px-4 py-2 text-xs font-semibold text-white transition-all duration-150 active:scale-95 cursor-pointer shadow-xs",
              inputValue.trim()
                ? "bg-[#0052cc] hover:bg-[#0043a8]"
                : "bg-slate-200 text-slate-400 cursor-not-allowed shadow-none",
            )}
          >
            <span>Gửi</span>
            <Send className="size-3.5" />
          </button>
        </div>
      </div>
    </main>
  );
}
