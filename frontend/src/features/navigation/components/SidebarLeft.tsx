import {
  MessageSquare,
  Network,
  FileText,
  Sticker,
  Settings,
  LogOut,
} from "lucide-react";
import { cn } from "@/lib/utils";
import { PixelBlockIcon } from "@/components/PixelBlockIcon";
import type { NavTab } from "../types";

type NavItem = {
  id: NavTab;
  label: string;
  icon: typeof MessageSquare;
  badge?: number;
};

const topNavItems: NavItem[] = [
  { id: "messages", label: "Tin nhắn", icon: MessageSquare, badge: 3 },
  { id: "connect", label: "Kết nối", icon: Network },
  { id: "posts", label: "Bài viết", icon: FileText },
  { id: "stickers", label: "Thư viện Sticker", icon: Sticker },
];

interface SidebarLeftProps {
  activeNav: NavTab;
  onSelectNav: (tab: NavTab) => void;
  onLogout?: () => void;
}

export function SidebarLeft({ activeNav, onSelectNav, onLogout }: SidebarLeftProps) {
  return (
    <aside className="relative flex h-full w-[72px] shrink-0 flex-col items-center justify-between border-r border-[#0047b3] bg-[#0052cc] py-4 text-white shadow-md select-none z-20">
      {/* Top: App Logo & Navigation */}
      <div className="flex flex-col items-center gap-5 w-full">
        {/* App Logo (BlockChat) */}
        <div
          title="BlockChat"
          className="relative cursor-pointer group flex items-center justify-center"
        >
          <PixelBlockIcon
            type="block_chat"
            className="size-11 rounded-2xl ring-2 ring-white/30 shadow-sm transition-transform duration-200 group-hover:scale-105"
          />
        </div>

        {/* Small divider */}
        <div className="h-px w-8 bg-white/20" />

        {/* Navigation list */}
        <nav className="flex flex-col items-center gap-2.5 w-full px-2">
          {topNavItems.map((item) => {
            const Icon = item.icon;
            const isActive = activeNav === item.id;
            return (
              <button
                key={item.id}
                type="button"
                onClick={() => onSelectNav(item.id)}
                title={item.label}
                className={cn(
                  "group relative flex size-11 items-center justify-center rounded-2xl transition-all duration-150 cursor-pointer",
                  isActive
                    ? "bg-white/20 text-white font-bold"
                    : "text-blue-100 hover:bg-white/10 hover:text-white",
                )}
              >
                {/* Active indicator bar */}
                {isActive && (
                  <span className="absolute left-0 top-1/2 -translate-y-1/2 h-5 w-1 rounded-r-md bg-white" />
                )}

                <Icon className={cn("size-5.5 transition-transform duration-150 group-hover:scale-105", isActive ? "text-white" : "text-blue-100")} />

                {item.badge && (
                  <span className="absolute -top-1 -right-1 flex min-w-4.5 h-4.5 px-1 items-center justify-center rounded-full bg-rose-500 border border-white text-[10px] font-bold text-white shadow-sm">
                    {item.badge}
                  </span>
                )}
              </button>
            );
          })}
        </nav>
      </div>

      {/* Bottom: User Avatar, Settings & Logout Actions */}
      <div className="flex flex-col items-center gap-3 w-full px-2">
        {/* User Avatar */}
        <div
          title="Tài khoản cá nhân"
          className="relative cursor-pointer group flex items-center justify-center"
        >
          <PixelBlockIcon
            type="user_khoa"
            className="size-10 rounded-2xl ring-2 ring-white/40 shadow-sm transition-transform duration-200 group-hover:scale-105"
          />
          <span className="absolute -bottom-0.5 -right-0.5 size-3 rounded-full border-2 border-[#0052cc] bg-emerald-400" />
        </div>

        {/* Small divider */}
        <div className="h-px w-8 bg-white/20" />

        {/* Settings button */}
        <button
          type="button"
          onClick={() => onSelectNav("settings")}
          title="Cài đặt"
          className={cn(
            "group relative flex size-11 items-center justify-center rounded-2xl transition-all duration-150 cursor-pointer",
            activeNav === "settings"
              ? "bg-white/20 text-white font-bold"
              : "text-blue-100 hover:bg-white/10 hover:text-white",
          )}
        >
          {activeNav === "settings" && (
            <span className="absolute left-0 top-1/2 -translate-y-1/2 h-5 w-1 rounded-r-md bg-white" />
          )}
          <Settings className="size-5.5 transition-transform duration-150 group-hover:scale-110 group-hover:rotate-45" />
        </button>

        {/* Logout button */}
        {onLogout && (
          <button
            type="button"
            onClick={onLogout}
            title="Đăng xuất"
            className="group relative flex size-11 items-center justify-center rounded-2xl text-blue-100/70 hover:bg-rose-500/20 hover:text-rose-200 transition-all duration-150 cursor-pointer"
          >
            <LogOut className="size-5 transition-transform duration-150 group-hover:scale-110" />
          </button>
        )}
      </div>
    </aside>
  );
}
