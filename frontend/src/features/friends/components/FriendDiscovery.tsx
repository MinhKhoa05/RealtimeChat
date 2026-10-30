import { useState, useMemo } from "react";
import {
  Search,
  UserPlus,
  Check,
  RefreshCw,
  Sparkles,
  Users,
  Compass,
} from "lucide-react";
import { cn } from "@/lib/utils";
import { PixelBlockIcon } from "@/components/PixelBlockIcon";
import {
  sampleSuggestedUsers,
  type SuggestedUser,
} from "../data/friendsData";

interface FriendDiscoveryProps {
  onAddFriendSuccess?: (user: SuggestedUser) => void;
}

export function FriendDiscovery({ onAddFriendSuccess }: FriendDiscoveryProps) {
  const [searchQuery, setSearchQuery] = useState("");
  const [usersList, setUsersList] = useState<SuggestedUser[]>(sampleSuggestedUsers);
  const [sentRequestIds, setSentRequestIds] = useState<Set<string>>(new Set());
  const [loadingIds, setLoadingIds] = useState<Set<string>>(new Set());
  const [isRefreshing, setIsRefreshing] = useState(false);

  // Helper to format contextual subtitle
  const getContextSubtitle = (user: SuggestedUser) => {
    const hasMutual = user.mutualFriendsCount && user.mutualFriendsCount > 0;
    const hasGroups = user.sharedGroupsCount && user.sharedGroupsCount > 0;

    if (hasMutual && hasGroups) {
      return `${user.mutualFriendsCount} bạn chung · ${user.sharedGroupsCount} nhóm chung`;
    }
    if (hasMutual) {
      return `${user.mutualFriendsCount} bạn chung`;
    }
    if (hasGroups) {
      return `${user.sharedGroupsCount} nhóm chung`;
    }
    if (user.isNewMember) {
      return "Thành viên mới";
    }
    return "Gợi ý dành cho bạn";
  };

  // Helper to get badge style / text for contextual reason
  const getContextBadge = (user: SuggestedUser) => {
    const hasMutual = user.mutualFriendsCount && user.mutualFriendsCount > 0;
    const hasGroups = user.sharedGroupsCount && user.sharedGroupsCount > 0;

    if (hasMutual && hasGroups) {
      return {
        label: "Tương đồng cao",
        className: "bg-indigo-50 text-indigo-700 border-indigo-200/80",
      };
    }
    if (hasMutual) {
      return {
        label: "Bạn chung",
        className: "bg-blue-50 text-blue-700 border-blue-200/80",
      };
    }
    if (hasGroups) {
      return {
        label: "Nhóm chung",
        className: "bg-emerald-50 text-emerald-700 border-emerald-200/80",
      };
    }
    if (user.isNewMember) {
      return {
        label: "Mới tham gia",
        className: "bg-amber-50 text-amber-700 border-amber-200/80",
      };
    }
    return {
      label: "Gợi ý",
      className: "bg-slate-50 text-slate-600 border-slate-200/80",
    };
  };

  // Filter users by search keyword (name or username)
  const filteredUsers = useMemo(() => {
    const query = searchQuery.trim().toLowerCase();
    if (!query) return usersList;

    return usersList.filter(
      (user) =>
        user.name.toLowerCase().includes(query) ||
        user.username.toLowerCase().includes(query)
    );
  }, [usersList, searchQuery]);

  // Handle Refresh
  const handleRefresh = () => {
    setIsRefreshing(true);
    // Simulate refresh latency while shuffling or reloading suggestions
    setTimeout(() => {
      // Shuffle slightly or reset list
      const shuffled = [...sampleSuggestedUsers].sort(() => Math.random() - 0.5);
      setUsersList(shuffled);
      setIsRefreshing(false);
    }, 600);
  };

  // Handle Sending friend request
  const handleSendRequest = (user: SuggestedUser) => {
    if (sentRequestIds.has(user.id) || loadingIds.has(user.id)) return;

    setLoadingIds((prev) => new Set(prev).add(user.id));

    setTimeout(() => {
      setLoadingIds((prev) => {
        const next = new Set(prev);
        next.delete(user.id);
        return next;
      });
      setSentRequestIds((prev) => new Set(prev).add(user.id));
      onAddFriendSuccess?.(user);
    }, 500);
  };

  return (
    <div className="flex-1 overflow-y-auto p-6 space-y-6">
      {/* Banner / Intro Card */}
      <div className="relative overflow-hidden rounded-2xl bg-gradient-to-r from-[#0052cc] via-[#0065ff] to-[#0284c7] p-6 text-white shadow-sm">
        <div className="relative z-10 max-w-xl">
          <div className="inline-flex items-center gap-1.5 rounded-full bg-white/20 px-3 py-1 text-xs font-semibold backdrop-blur-xs mb-3">
            <Sparkles className="size-3.5 text-amber-300" />
            <span>Gợi ý thông minh</span>
          </div>
          <h2 className="text-xl font-bold tracking-tight sm:text-2xl">
            Mở rộng mạng lưới bạn bè
          </h2>
          <p className="mt-1.5 text-xs sm:text-sm text-blue-100 leading-relaxed">
            Khám phá những người dùng có chung sở thích, bạn bè chung, cùng tham gia nhóm hoặc thành viên mới trong cộng đồng BlockChat.
          </p>
        </div>

        {/* Decorative background icons */}
        <div className="absolute -bottom-6 -right-4 opacity-15 pointer-events-none select-none">
          <Users className="size-48 text-white" />
        </div>
      </div>

      {/* Control Bar: Search + Refresh + Summary */}
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h3 className="text-sm font-bold text-slate-900">
            Người dùng gợi ý ({filteredUsers.length})
          </h3>
          <p className="text-xs text-slate-500">
            Kết hợp bạn chung, nhóm chung và thành viên mới nổi bật
          </p>
        </div>

        <div className="flex items-center gap-3">
          {/* Search bar */}
          <div className="relative flex w-full sm:w-72 items-center rounded-xl border border-slate-200 bg-white px-3 py-1.5 shadow-2xs focus-within:border-[#0052cc] focus-within:ring-2 focus-within:ring-[#0052cc]/10 transition">
            <Search className="size-4 shrink-0 text-slate-400" />
            <input
              type="text"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder="Tìm theo tên hoặc username..."
              className="ml-2 w-full bg-transparent text-xs text-slate-800 placeholder:text-slate-400 outline-none"
            />
            {searchQuery && (
              <button
                type="button"
                onClick={() => setSearchQuery("")}
                className="text-xs text-slate-400 hover:text-slate-600 px-1"
              >
                ✕
              </button>
            )}
          </div>

          {/* Refresh button */}
          <button
            type="button"
            onClick={handleRefresh}
            disabled={isRefreshing}
            title="Làm mới danh sách gợi ý"
            className="flex items-center gap-1.5 rounded-xl border border-slate-200 bg-white px-3 py-1.5 text-xs font-semibold text-slate-700 shadow-2xs hover:bg-slate-50 active:bg-slate-100 disabled:opacity-60 transition cursor-pointer shrink-0"
          >
            <RefreshCw
              className={cn("size-3.5 text-slate-600", isRefreshing && "animate-spin text-[#0052cc]")}
            />
            <span className="hidden sm:inline">Làm mới</span>
          </button>
        </div>
      </div>

      {/* Grid of Suggested Users */}
      {isRefreshing ? (
        /* Loading skeleton state */
        <div className="grid grid-cols-1 md:grid-cols-2 gap-3.5">
          {Array.from({ length: 6 }).map((_, i) => (
            <div
              key={`skel-${i}`}
              className="flex items-center justify-between rounded-2xl border border-slate-200/80 bg-white p-4 shadow-xs animate-pulse"
            >
              <div className="flex items-center gap-3.5">
                <div className="size-12 rounded-xl bg-slate-200 shrink-0" />
                <div className="space-y-2">
                  <div className="h-4 w-28 rounded-md bg-slate-200" />
                  <div className="h-3 w-20 rounded-md bg-slate-150" />
                  <div className="h-3 w-36 rounded-md bg-slate-100" />
                </div>
              </div>
              <div className="h-8 w-24 rounded-xl bg-slate-200" />
            </div>
          ))}
        </div>
      ) : filteredUsers.length === 0 ? (
        /* Empty state */
        <div className="rounded-2xl border border-dashed border-slate-200 bg-white/70 py-16 px-6 text-center">
          <div className="mx-auto flex size-14 items-center justify-center rounded-2xl bg-blue-50 text-[#0052cc] mb-3">
            <Compass className="size-7" />
          </div>
          <h4 className="text-base font-bold text-slate-900">
            Không tìm thấy người dùng phù hợp
          </h4>
          <p className="mt-1 text-xs text-slate-500 max-w-sm mx-auto">
            {searchQuery
              ? `Không có kết quả nào khớp với "${searchQuery}". Hãy thử tìm kiếm bằng từ khóa hoặc username khác.`
              : "Hiện tại chưa có gợi ý mới. Vui lòng bấm làm mới để tải thêm gợi ý."}
          </p>
          {searchQuery && (
            <button
              type="button"
              onClick={() => setSearchQuery("")}
              className="mt-4 inline-flex items-center gap-1.5 rounded-xl bg-[#eef3fb] px-4 py-2 text-xs font-semibold text-[#0052cc] hover:bg-[#e0ecfb] transition"
            >
              Xóa bộ lọc tìm kiếm
            </button>
          )}
        </div>
      ) : (
        /* List of suggestions */
        <div className="grid grid-cols-1 md:grid-cols-2 gap-3.5">
          {filteredUsers.map((user) => {
            const isSent = sentRequestIds.has(user.id);
            const isLoading = loadingIds.has(user.id);
            const badge = getContextBadge(user);
            const subtitle = getContextSubtitle(user);

            return (
              <div
                key={user.id}
                className="group relative flex flex-col justify-between rounded-2xl border border-slate-200/90 bg-white p-4 shadow-xs hover:border-[#0052cc]/30 hover:shadow-md transition-all duration-150"
              >
                <div className="flex items-start gap-3.5">
                  {/* User Avatar */}
                  <div className="relative shrink-0">
                    <PixelBlockIcon
                      type={user.avatarType}
                      className="size-12 rounded-xl shadow-2xs group-hover:scale-105 transition-transform duration-150"
                    />
                    {user.isNewMember && (
                      <span
                        title="Thành viên mới"
                        className="absolute -top-1 -right-1 flex size-4 items-center justify-center rounded-full bg-amber-400 text-[10px] font-bold text-white shadow-2xs ring-2 ring-white"
                      >
                        ★
                      </span>
                    )}
                  </div>

                  {/* Info */}
                  <div className="min-w-0 flex-1">
                    <div className="flex items-center gap-2 flex-wrap">
                      <h4 className="text-sm font-bold text-slate-900 truncate">
                        {user.name}
                      </h4>
                      <span
                        className={cn(
                          "rounded-md border px-1.5 py-0.5 text-[10px] font-medium leading-none",
                          badge.className
                        )}
                      >
                        {badge.label}
                      </span>
                    </div>

                    <p className="text-xs text-slate-400 font-mono mt-0.5">
                      @{user.username}
                    </p>

                    {/* Contextual Subtitle */}
                    <p className="mt-1.5 text-xs font-semibold text-[#0052cc] flex items-center gap-1">
                      <Sparkles className="size-3 shrink-0" />
                      <span className="truncate">{subtitle}</span>
                    </p>

                    {user.bio && (
                      <p className="mt-1 text-xs text-slate-500 line-clamp-1">
                        {user.bio}
                      </p>
                    )}
                  </div>
                </div>

                {/* Bottom action row */}
                <div className="mt-3.5 flex items-center justify-end border-t border-slate-100 pt-3">
                  <button
                    type="button"
                    disabled={isSent || isLoading}
                    onClick={() => handleSendRequest(user)}
                    className={cn(
                      "flex items-center gap-1.5 rounded-xl px-4 py-1.5 text-xs font-semibold transition-all shadow-xs cursor-pointer",
                      isSent
                        ? "bg-slate-100 text-slate-600 cursor-default"
                        : "bg-[#0052cc] text-white hover:bg-[#0043a8] active:scale-98"
                    )}
                  >
                    {isLoading ? (
                      <>
                        <RefreshCw className="size-3.5 animate-spin" />
                        <span>Đang gửi...</span>
                      </>
                    ) : isSent ? (
                      <>
                        <Check className="size-3.5 text-emerald-600" />
                        <span>Đã gửi lời mời</span>
                      </>
                    ) : (
                      <>
                        <UserPlus className="size-3.5" />
                        <span>Kết bạn</span>
                      </>
                    )}
                  </button>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
