import { useState, useMemo } from "react";
import {
  Users,
  UserCheck,
  UserPlus,
  ShieldBan,
  Search,
  ArrowUpDown,
  Filter,
  MoreHorizontal,
  MessageSquare,
  Phone,
} from "lucide-react";
import { cn } from "@/lib/utils";
import { PixelBlockIcon } from "@/components/PixelBlockIcon";
import {
  sampleFriendsList,
  sampleFriendGroups,
  sampleFriendRequests,
  sampleBlockedList,
  type FriendItem,
  type GroupItem,
} from "../data/friendsData";

export type FriendsSubTab = "friends_list" | "groups_list" | "friend_requests" | "blocked_list";

interface FriendsPageProps {
  initialSubTab?: FriendsSubTab;
  onStartChatWithFriend?: (friend: FriendItem) => void;
  onOpenGroupChat?: (group: GroupItem) => void;
}

export function FriendsView({ initialSubTab = "friends_list", onStartChatWithFriend, onOpenGroupChat }: FriendsPageProps) {
  const [activeSubTab, setActiveSubTab] = useState<FriendsSubTab>(initialSubTab);
  
  // Search & Filter state for Friends
  const [searchFriend, setSearchFriend] = useState("");
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");
  const [filterOnline, setFilterOnline] = useState<"all" | "online">("all");

  // Search & Filter state for Groups
  const [searchGroup, setSearchGroup] = useState("");
  const [groupSort, setGroupSort] = useState<"active" | "name" | "members">("active");

  // Filter and sort friends
  const filteredFriends = useMemo(() => {
    let result = sampleFriendsList.filter((f) =>
      f.name.toLowerCase().includes(searchFriend.toLowerCase()),
    );
    if (filterOnline === "online") {
      result = result.filter((f) => f.online);
    }
    result.sort((a, b) =>
      sortOrder === "asc"
        ? a.name.localeCompare(b.name, "vi")
        : b.name.localeCompare(a.name, "vi"),
    );
    return result;
  }, [searchFriend, sortOrder, filterOnline]);

  // Group friends by initial letter
  const groupedFriends = useMemo(() => {
    const groups: { [key: string]: FriendItem[] } = {};
    for (const friend of filteredFriends) {
      const letter = friend.group || friend.name.charAt(0).toUpperCase();
      if (!groups[letter]) {
        groups[letter] = [];
      }
      groups[letter].push(friend);
    }
    return groups;
  }, [filteredFriends]);

  // Filter and sort groups
  const filteredGroups = useMemo(() => {
    const result = sampleFriendGroups.filter((g) =>
      g.name.toLowerCase().includes(searchGroup.toLowerCase()),
    );
    if (groupSort === "name") {
      result.sort((a, b) => a.name.localeCompare(b.name, "vi"));
    } else if (groupSort === "members") {
      result.sort((a, b) => b.memberCount - a.memberCount);
    }
    return result;
  }, [searchGroup, groupSort]);

  return (
    <div className="flex h-full flex-1 overflow-hidden bg-[#ffffff] select-none">
      {/* Sub-Sidebar (Cột 2): Danh mục bạn bè */}
      <aside className="flex h-full w-[310px] shrink-0 flex-col border-r border-[#dbe6f5] bg-white">
        {/* Search & Action bar */}
        <div className="flex items-center gap-2 p-3.5 border-b border-[#f1f5f9]">
          <div className="relative flex flex-1 items-center rounded-xl bg-[#f0f4f9] px-3 py-2 transition-all duration-150 focus-within:bg-white focus-within:ring-2 focus-within:ring-[#0052cc]/20 focus-within:border-[#0052cc] border border-transparent">
            <Search className="size-4 shrink-0 text-slate-400" />
            <input
              type="text"
              placeholder="Tìm kiếm"
              className="ml-2 w-full bg-transparent text-xs text-slate-800 placeholder:text-slate-400 outline-none"
            />
          </div>
          <button
            type="button"
            className="grid size-8.5 shrink-0 place-items-center rounded-xl text-slate-600 hover:bg-[#eef3fb] hover:text-[#0052cc] transition"
            title="Thêm bạn mới"
          >
            <UserPlus className="size-4.5" />
          </button>
        </div>

        {/* Categories navigation list (Danh sách bạn bè, Danh sách nhóm, Lời mời kết bạn, Đã chặn) */}
        <div className="p-2 space-y-1">
          <button
            type="button"
            onClick={() => setActiveSubTab("friends_list")}
            className={cn(
              "flex w-full items-center gap-3 rounded-xl px-3.5 py-3 text-xs font-semibold transition-all duration-150 text-left",
              activeSubTab === "friends_list"
                ? "bg-[#e8f1fd] text-[#0052cc]"
                : "text-slate-700 hover:bg-slate-50",
            )}
          >
            <Users className="size-4.5 text-[#0052cc]" />
            <span>Danh sách bạn bè</span>
          </button>

          <button
            type="button"
            onClick={() => setActiveSubTab("groups_list")}
            className={cn(
              "flex w-full items-center gap-3 rounded-xl px-3.5 py-3 text-xs font-semibold transition-all duration-150 text-left",
              activeSubTab === "groups_list"
                ? "bg-[#e8f1fd] text-[#0052cc]"
                : "text-slate-700 hover:bg-slate-50",
            )}
          >
            <Users className="size-4.5 text-[#0052cc]" />
            <span>Danh sách nhóm chat</span>
          </button>

          <button
            type="button"
            onClick={() => setActiveSubTab("friend_requests")}
            className={cn(
              "flex w-full items-center justify-between rounded-xl px-3.5 py-3 text-xs font-semibold transition-all duration-150 text-left",
              activeSubTab === "friend_requests"
                ? "bg-[#e8f1fd] text-[#0052cc]"
                : "text-slate-700 hover:bg-slate-50",
            )}
          >
            <div className="flex items-center gap-3">
              <UserPlus className="size-4.5 text-[#0052cc]" />
              <span>Lời mời kết bạn</span>
            </div>
            <span className="flex size-4.5 items-center justify-center rounded-full bg-rose-500 text-[10px] font-bold text-white shadow-2xs">
              2
            </span>
          </button>

          <button
            type="button"
            onClick={() => setActiveSubTab("blocked_list")}
            className={cn(
              "flex w-full items-center gap-3 rounded-xl px-3.5 py-3 text-xs font-semibold transition-all duration-150 text-left",
              activeSubTab === "blocked_list"
                ? "bg-[#e8f1fd] text-[#0052cc]"
                : "text-slate-700 hover:bg-slate-50",
            )}
          >
            <ShieldBan className="size-4.5 text-slate-500" />
            <span>Đã chặn</span>
          </button>
        </div>
      </aside>

      {/* Main Content Area */}
      <main className="flex h-full flex-1 flex-col overflow-hidden bg-[#f4f7fb]">
        {/* Top Header */}
        <header className="flex h-16 shrink-0 items-center border-b border-[#dbe6f5] bg-white px-6 shadow-2xs">
          <div className="flex items-center gap-3">
            <div className="flex size-9 items-center justify-center rounded-xl bg-[#eef3fb] text-[#0052cc]">
              {activeSubTab === "friends_list" && <UserCheck className="size-5" />}
              {activeSubTab === "groups_list" && <Users className="size-5" />}
              {activeSubTab === "friend_requests" && <UserPlus className="size-5" />}
              {activeSubTab === "blocked_list" && <ShieldBan className="size-5" />}
            </div>
            <h1 className="text-base font-bold text-slate-900 tracking-tight">
              {activeSubTab === "friends_list" && "Danh sách bạn bè"}
              {activeSubTab === "groups_list" && "Danh sách nhóm chat"}
              {activeSubTab === "friend_requests" && "Lời mời kết bạn"}
              {activeSubTab === "blocked_list" && "Danh sách người dùng đã chặn"}
            </h1>
          </div>
        </header>

        {/* Tab Content: Danh sách bạn bè */}
        {activeSubTab === "friends_list" && (
          <div className="flex-1 overflow-y-auto p-6 space-y-4">
            {/* Header stats & filter controls */}
            <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
              <div className="text-sm font-bold text-slate-900">
                Bạn bè ({filteredFriends.length})
              </div>

              <div className="flex items-center gap-3 flex-wrap">
                {/* Search input in list */}
                <div className="relative flex w-64 items-center rounded-xl border border-slate-200 bg-white px-3 py-1.5 shadow-2xs focus-within:border-[#0052cc]">
                  <Search className="size-4 text-slate-400" />
                  <input
                    type="text"
                    value={searchFriend}
                    onChange={(e) => setSearchFriend(e.target.value)}
                    placeholder="Tìm bạn..."
                    className="ml-2 w-full bg-transparent text-xs text-slate-800 placeholder:text-slate-400 outline-none"
                  />
                </div>

                {/* Sort Order dropdown */}
                <div className="flex items-center gap-1.5 rounded-xl border border-slate-200 bg-white px-3 py-1.5 text-xs text-slate-700 shadow-2xs cursor-pointer hover:bg-slate-50">
                  <ArrowUpDown className="size-3.5 text-slate-500" />
                  <select
                    value={sortOrder}
                    onChange={(e) => setSortOrder(e.target.value as "asc" | "desc")}
                    className="bg-transparent text-xs text-slate-700 outline-none cursor-pointer"
                  >
                    <option value="asc">Tên (A-Z)</option>
                    <option value="desc">Tên (Z-A)</option>
                  </select>
                </div>

                {/* Filter online dropdown */}
                <div className="flex items-center gap-1.5 rounded-xl border border-slate-200 bg-white px-3 py-1.5 text-xs text-slate-700 shadow-2xs cursor-pointer hover:bg-slate-50">
                  <Filter className="size-3.5 text-slate-500" />
                  <select
                    value={filterOnline}
                    onChange={(e) => setFilterOnline(e.target.value as "all" | "online")}
                    className="bg-transparent text-xs text-slate-700 outline-none cursor-pointer"
                  >
                    <option value="all">Tất cả</option>
                    <option value="online">Đang online</option>
                  </select>
                </div>
              </div>
            </div>

            {/* Alphabetical grouped friend rows */}
            <div className="rounded-2xl border border-slate-200/90 bg-white p-5 shadow-xs divide-y divide-slate-100">
              {Object.keys(groupedFriends).length === 0 ? (
                <div className="py-12 text-center text-sm text-slate-400">
                  Không tìm thấy bạn bè nào phù hợp.
                </div>
              ) : (
                Object.keys(groupedFriends).map((letter) => (
                  <div key={letter} className="pt-3 pb-3 first:pt-0 last:pb-0">
                    {/* Letter badge */}
                    <div className="mb-2 text-xs font-bold text-slate-400 tracking-wider">
                      {letter}
                    </div>

                    {/* Friend list in this letter */}
                    <div className="space-y-1">
                      {groupedFriends[letter].map((friend) => (
                        <div
                          key={friend.id}
                          className="flex items-center justify-between rounded-xl p-2.5 hover:bg-[#f0f4f9] transition duration-150 group"
                        >
                          <div className="flex items-center gap-3 min-w-0">
                            {/* Avatar */}
                            <div className="relative shrink-0">
                              <PixelBlockIcon
                                type={friend.avatarType}
                                className="size-10 rounded-xl shadow-2xs"
                              />
                              {friend.online && (
                                <span className="absolute -bottom-0.5 -right-0.5 size-3 rounded-full border-2 border-white bg-emerald-500" />
                              )}
                            </div>

                            {/* Info */}
                            <div className="min-w-0">
                              <h3 className="truncate text-sm font-semibold text-slate-800 group-hover:text-[#0052cc] transition">
                                {friend.name}
                              </h3>
                              {friend.statusText && (
                                <p className="truncate text-xs text-slate-400">
                                  {friend.statusText}
                                </p>
                              )}
                            </div>
                          </div>

                          {/* Action icons on hover */}
                          <div className="flex items-center gap-1">
                            <button
                              type="button"
                              onClick={() => onStartChatWithFriend?.(friend)}
                              className="grid size-8 place-items-center rounded-lg text-slate-400 hover:bg-white hover:text-[#0052cc] transition shadow-2xs"
                              title="Nhắn tin"
                            >
                              <MessageSquare className="size-4" />
                            </button>
                            <button
                              type="button"
                              className="grid size-8 place-items-center rounded-lg text-slate-400 hover:bg-white hover:text-[#0052cc] transition shadow-2xs"
                              title="Gọi thoại"
                            >
                              <Phone className="size-4" />
                            </button>
                            <button
                              type="button"
                              className="grid size-8 place-items-center rounded-lg text-slate-400 hover:bg-white hover:text-slate-700 transition shadow-2xs"
                              title="Tùy chọn khác"
                            >
                              <MoreHorizontal className="size-4" />
                            </button>
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>
        )}

        {/* Tab Content: Danh sách nhóm (Giao diện list phẳng giống hệt danh sách bạn bè) */}
        {activeSubTab === "groups_list" && (
          <div className="flex-1 overflow-y-auto p-6 space-y-4">
            {/* Header stats & filter controls */}
            <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
              <div className="text-sm font-bold text-slate-900">
                Nhóm ({filteredGroups.length})
              </div>

              <div className="flex items-center gap-3 flex-wrap">
                {/* Search input in group list */}
                <div className="relative flex w-64 items-center rounded-xl border border-slate-200 bg-white px-3 py-1.5 shadow-2xs focus-within:border-[#0052cc]">
                  <Search className="size-4 text-slate-400" />
                  <input
                    type="text"
                    value={searchGroup}
                    onChange={(e) => setSearchGroup(e.target.value)}
                    placeholder="Tìm kiếm..."
                    className="ml-2 w-full bg-transparent text-xs text-slate-800 placeholder:text-slate-400 outline-none"
                  />
                </div>

                {/* Sort Order dropdown */}
                <div className="flex items-center gap-1.5 rounded-xl border border-slate-200 bg-white px-3 py-1.5 text-xs text-slate-700 shadow-2xs cursor-pointer hover:bg-slate-50">
                  <ArrowUpDown className="size-3.5 text-slate-500" />
                  <select
                    value={groupSort}
                    onChange={(e) => setGroupSort(e.target.value as "active" | "name" | "members")}
                    className="bg-transparent text-xs text-slate-700 outline-none cursor-pointer"
                  >
                    <option value="active">Hoạt động (mới → cũ)</option>
                    <option value="name">Tên (A-Z)</option>
                    <option value="members">Số thành viên</option>
                  </select>
                </div>

                {/* Filter dropdown */}
                <div className="flex items-center gap-1.5 rounded-xl border border-slate-200 bg-white px-3 py-1.5 text-xs text-slate-700 shadow-2xs cursor-pointer hover:bg-slate-50">
                  <Filter className="size-3.5 text-slate-500" />
                  <select className="bg-transparent text-xs text-slate-700 outline-none cursor-pointer">
                    <option value="all">Tất cả</option>
                    <option value="mine">Nhóm tôi quản lý</option>
                  </select>
                </div>
              </div>
            </div>

            {/* Group List Table Panel */}
            <div className="rounded-2xl border border-slate-200/90 bg-white p-2 shadow-xs divide-y divide-slate-100">
              {filteredGroups.length === 0 ? (
                <div className="py-12 text-center text-sm text-slate-400">
                  Không tìm thấy nhóm nào phù hợp.
                </div>
              ) : (
                filteredGroups.map((group) => (
                  <div
                    key={group.id}
                    className="flex items-center justify-between rounded-xl p-3 hover:bg-[#f0f4f9] transition duration-150 group cursor-pointer"
                    onClick={() => onOpenGroupChat?.(group)}
                  >
                    <div className="flex items-center gap-3.5 min-w-0">
                      {/* Avatar */}
                      <div className="relative shrink-0">
                        <PixelBlockIcon
                          type={group.avatarType}
                          className="size-11 rounded-xl shadow-2xs ring-1 ring-black/5"
                        />
                      </div>

                      {/* Info */}
                      <div className="min-w-0">
                        <h3 className="truncate text-sm font-semibold text-slate-800 group-hover:text-[#0052cc] transition">
                          {group.name}
                        </h3>
                        <p className="truncate text-xs text-slate-400 mt-0.5">
                          {group.memberCount} thành viên · Hoạt động {group.lastActive}
                        </p>
                      </div>
                    </div>

                    {/* Action icons on hover */}
                    <div className="flex items-center gap-1">
                      <button
                        type="button"
                        onClick={(e) => {
                          e.stopPropagation();
                          onOpenGroupChat?.(group);
                        }}
                        className="grid size-8.5 place-items-center rounded-lg text-slate-400 hover:bg-white hover:text-[#0052cc] transition shadow-2xs"
                        title="Vào trò chuyện"
                      >
                        <MessageSquare className="size-4" />
                      </button>
                      <button
                        type="button"
                        onClick={(e) => e.stopPropagation()}
                        className="grid size-8.5 place-items-center rounded-lg text-slate-400 hover:bg-white hover:text-slate-700 transition shadow-2xs"
                        title="Tùy chọn khác"
                      >
                        <MoreHorizontal className="size-4" />
                      </button>
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>
        )}

        {/* Tab Content: Lời mời kết bạn */}
        {activeSubTab === "friend_requests" && (
          <div className="flex-1 overflow-y-auto p-6 space-y-4">
            <div className="text-sm font-bold text-slate-900">
              Lời mời kết bạn ({sampleFriendRequests.length})
            </div>
            <div className="rounded-2xl border border-slate-200 bg-white p-3 shadow-xs divide-y divide-slate-100">
              {sampleFriendRequests.map((req) => (
                <div
                  key={req.id}
                  className="flex items-center justify-between p-3"
                >
                  <div className="flex items-center gap-3">
                    <PixelBlockIcon
                      type={req.avatarType}
                      className="size-11 rounded-xl shadow-2xs"
                    />
                    <div>
                      <h4 className="text-sm font-bold text-slate-900">{req.name}</h4>
                      <p className="text-xs text-slate-400">
                        {req.mutualFriends} bạn chung · {req.time}
                      </p>
                    </div>
                  </div>
                  <div className="flex items-center gap-2">
                    <button
                      type="button"
                      className="rounded-xl bg-[#0052cc] px-4 py-1.5 text-xs font-semibold text-white hover:bg-[#0043a8] transition shadow-xs"
                    >
                      Đồng ý
                    </button>
                    <button
                      type="button"
                      className="rounded-xl border border-slate-200 bg-white px-3.5 py-1.5 text-xs font-semibold text-slate-600 hover:bg-slate-50 transition"
                    >
                      Từ chối
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Tab Content: Đã chặn */}
        {activeSubTab === "blocked_list" && (
          <div className="flex-1 overflow-y-auto p-6 space-y-4">
            <div className="text-sm font-bold text-slate-900">
              Danh sách đã chặn ({sampleBlockedList.length})
            </div>
            <div className="rounded-2xl border border-slate-200 bg-white p-3 shadow-xs divide-y divide-slate-100">
              {sampleBlockedList.map((b) => (
                <div
                  key={b.id}
                  className="flex items-center justify-between p-3"
                >
                  <div className="flex items-center gap-3">
                    <PixelBlockIcon
                      type={b.avatarType}
                      className="size-10 rounded-xl shadow-2xs"
                    />
                    <div>
                      <h4 className="text-sm font-semibold text-slate-800">{b.name}</h4>
                      <p className="text-xs text-slate-400">
                        Đã chặn từ {b.blockedDate}
                      </p>
                    </div>
                  </div>
                  <button
                    type="button"
                    className="rounded-xl border border-slate-200 bg-white px-3.5 py-1.5 text-xs font-semibold text-slate-700 hover:bg-rose-50 hover:text-rose-600 hover:border-rose-200 transition"
                  >
                    Bỏ chặn
                  </button>
                </div>
              ))}
            </div>
          </div>
        )}
      </main>
    </div>
  );
}
