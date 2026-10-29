import { useState } from "react";
import {
  Sticker,
  Plus,
  Bookmark,
  Trash2,
  Search,
  Check,
  Copy,
} from "lucide-react";
import { cn } from "@/lib/utils";
import {
  sampleCollections,
  type StickerCollection,
  type StickerItem,
} from "../data/stickersData";

type StickerSubTab = "all" | "my_collections" | "saved";

export function StickersView() {
  const [activeSubTab, setActiveSubTab] = useState<StickerSubTab>("all");
  const [collections, setCollections] = useState<StickerCollection[]>(sampleCollections);
  const [searchQuery, setSearchQuery] = useState("");
  const [selectedCollection, setSelectedCollection] = useState<StickerCollection>(collections[0]);
  const [copiedKey, setCopiedKey] = useState<string | null>(null);

  // Modal / Form state for Creating Collection
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [newColName, setNewColName] = useState("");
  const [newColDesc, setNewColDesc] = useState("");
  const [newColEmoji, setNewColEmoji] = useState("✨");

  // Form state for Adding Sticker to selected Collection
  const [newStickerName, setNewStickerName] = useState("");
  const [newStickerEmoji, setNewStickerEmoji] = useState("😎");

  // Filter collections by tab & search
  const filteredCollections = collections.filter((col) => {
    if (searchQuery.trim() && !col.name.toLowerCase().includes(searchQuery.toLowerCase())) {
      return false;
    }
    if (activeSubTab === "my_collections") return col.isMyCollection;
    if (activeSubTab === "saved") return col.isSaved;
    return true;
  });

  // Create Collection
  const handleCreateCollection = () => {
    if (!newColName.trim()) return;
    const newCol: StickerCollection = {
      id: `col-${Date.now()}`,
      name: newColName.trim(),
      description: newColDesc.trim() || "Bộ sưu tập nhãn dán tự tạo",
      coverEmoji: newColEmoji,
      isMyCollection: true,
      isSaved: true,
      sharedKey: crypto.randomUUID(),
      stickersCount: 0,
      stickers: [],
    };
    setCollections([newCol, ...collections]);
    setSelectedCollection(newCol);
    setShowCreateModal(false);
    setNewColName("");
    setNewColDesc("");
  };

  // Delete Collection
  const handleDeleteCollection = (id: string) => {
    const updated = collections.filter((c) => c.id !== id);
    setCollections(updated);
    if (selectedCollection?.id === id && updated.length > 0) {
      setSelectedCollection(updated[0]);
    }
  };

  // Toggle Save Collection
  const handleToggleSave = (colId: string) => {
    setCollections((prev) =>
      prev.map((c) => (c.id === colId ? { ...c, isSaved: !c.isSaved } : c)),
    );
    if (selectedCollection.id === colId) {
      setSelectedCollection((prev) => ({ ...prev, isSaved: !prev.isSaved }));
    }
  };

  // Add Sticker to current collection
  const handleAddSticker = () => {
    if (!newStickerName.trim()) return;
    const newSticker: StickerItem = {
      id: `s-${Date.now()}`,
      name: newStickerName.trim(),
      emoji: newStickerEmoji,
    };
    const updatedStickers = [...selectedCollection.stickers, newSticker];
    const updatedCollection = {
      ...selectedCollection,
      stickers: updatedStickers,
      stickersCount: updatedStickers.length,
    };

    setSelectedCollection(updatedCollection);
    setCollections((prev) =>
      prev.map((c) => (c.id === selectedCollection.id ? updatedCollection : c)),
    );
    setNewStickerName("");
  };

  // Delete Sticker
  const handleDeleteSticker = (stickerId: string) => {
    const updatedStickers = selectedCollection.stickers.filter((s) => s.id !== stickerId);
    const updatedCollection = {
      ...selectedCollection,
      stickers: updatedStickers,
      stickersCount: updatedStickers.length,
    };

    setSelectedCollection(updatedCollection);
    setCollections((prev) =>
      prev.map((c) => (c.id === selectedCollection.id ? updatedCollection : c)),
    );
  };

  // Copy shared key
  const handleCopyKey = (key: string) => {
    navigator.clipboard.writeText(key);
    setCopiedKey(key);
    setTimeout(() => setCopiedKey(null), 2000);
  };

  return (
    <div className="flex h-full flex-1 overflow-hidden bg-[#ffffff] select-none">
      {/* Sub-Sidebar (Cột 2): Phân loại & Danh sách bộ Sticker */}
      <aside className="flex h-full w-[310px] shrink-0 flex-col border-r border-[#dbe6f5] bg-white">
        {/* Header & Create Button */}
        <div className="flex items-center justify-between p-3.5 border-b border-[#f1f5f9]">
          <div className="flex items-center gap-2">
            <div className="flex size-9 items-center justify-center rounded-xl bg-[#eef3fb] text-[#0052cc]">
              <Sticker className="size-5" />
            </div>
            <h2 className="text-sm font-bold text-slate-900">Thư viện Sticker</h2>
          </div>
          <button
            type="button"
            onClick={() => setShowCreateModal(true)}
            className="grid size-8 place-items-center rounded-xl bg-[#0052cc] text-white hover:bg-[#0043a8] transition shadow-2xs"
            title="Tạo bộ sưu tập mới"
          >
            <Plus className="size-4.5" />
          </button>
        </div>

        {/* Categories Tab */}
        <div className="p-2 space-y-1 border-b border-[#f1f5f9]">
          <button
            type="button"
            onClick={() => setActiveSubTab("all")}
            className={cn(
              "flex w-full items-center justify-between rounded-xl px-3.5 py-2.5 text-xs font-semibold transition text-left",
              activeSubTab === "all"
                ? "bg-[#e8f1fd] text-[#0052cc]"
                : "text-slate-700 hover:bg-slate-50",
            )}
          >
            <span>Khám phá bộ sưu tập</span>
            <span className="text-[11px] text-slate-400 font-normal">({collections.length})</span>
          </button>
          <button
            type="button"
            onClick={() => setActiveSubTab("my_collections")}
            className={cn(
              "flex w-full items-center justify-between rounded-xl px-3.5 py-2.5 text-xs font-semibold transition text-left",
              activeSubTab === "my_collections"
                ? "bg-[#e8f1fd] text-[#0052cc]"
                : "text-slate-700 hover:bg-slate-50",
            )}
          >
            <span>Bộ sticker của tôi</span>
            <span className="text-[11px] text-slate-400 font-normal">
              ({collections.filter((c) => c.isMyCollection).length})
            </span>
          </button>
          <button
            type="button"
            onClick={() => setActiveSubTab("saved")}
            className={cn(
              "flex w-full items-center justify-between rounded-xl px-3.5 py-2.5 text-xs font-semibold transition text-left",
              activeSubTab === "saved"
                ? "bg-[#e8f1fd] text-[#0052cc]"
                : "text-slate-700 hover:bg-slate-50",
            )}
          >
            <span>Đã lưu vào chat</span>
            <span className="text-[11px] text-slate-400 font-normal">
              ({collections.filter((c) => c.isSaved).length})
            </span>
          </button>
        </div>

        {/* Search */}
        <div className="p-3 border-b border-[#f1f5f9]">
          <div className="relative flex items-center rounded-xl bg-[#f0f4f9] px-3 py-1.5 focus-within:bg-white focus-within:border-[#0052cc] border border-transparent">
            <Search className="size-3.5 text-slate-400" />
            <input
              type="text"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder="Tìm bộ sưu tập..."
              className="ml-2 w-full bg-transparent text-xs text-slate-800 placeholder:text-slate-400 outline-none"
            />
          </div>
        </div>

        {/* Collections List */}
        <div className="flex-1 overflow-y-auto p-2 space-y-1">
          {filteredCollections.map((col) => {
            const isSelected = selectedCollection?.id === col.id;
            return (
              <div
                key={col.id}
                onClick={() => setSelectedCollection(col)}
                className={cn(
                  "flex items-center justify-between rounded-xl p-2.5 transition cursor-pointer group",
                  isSelected
                    ? "bg-[#e8f1fd] text-[#0052cc] border border-[#bfdbfe]"
                    : "hover:bg-slate-50 text-slate-800",
                )}
              >
                <div className="flex items-center gap-3 min-w-0">
                  <div className="grid size-10 place-items-center rounded-xl bg-slate-100 text-xl shadow-2xs shrink-0">
                    {col.coverEmoji}
                  </div>
                  <div className="min-w-0">
                    <h3 className="truncate text-xs font-bold">{col.name}</h3>
                    <p className="truncate text-[11px] text-slate-400">
                      {col.stickersCount} nhãn dán
                    </p>
                  </div>
                </div>

                {col.isMyCollection && (
                  <span className="rounded-md bg-blue-100 px-1.5 py-0.5 text-[9px] font-bold text-[#0052cc]">
                    Tạo bởi tôi
                  </span>
                )}
              </div>
            );
          })}
        </div>
      </aside>

      {/* Main Content Area: Collection Detail & CRUD Stickers */}
      <main className="flex h-full flex-1 flex-col overflow-hidden bg-[#f4f7fb]">
        {/* Top Header of Selected Collection */}
        <header className="flex h-16 shrink-0 items-center justify-between border-b border-[#dbe6f5] bg-white px-6 shadow-2xs">
          <div className="flex items-center gap-3 min-w-0">
            <span className="text-2xl">{selectedCollection.coverEmoji}</span>
            <div className="min-w-0">
              <h1 className="truncate text-base font-bold text-slate-900 tracking-tight">
                {selectedCollection.name}
              </h1>
              <p className="truncate text-xs text-slate-400">
                {selectedCollection.description}
              </p>
            </div>
          </div>

          {/* Action buttons */}
          <div className="flex items-center gap-2">
            {/* Copy Shared Key */}
            <button
              type="button"
              onClick={() => handleCopyKey(selectedCollection.sharedKey)}
              className="flex items-center gap-1.5 rounded-xl border border-slate-200 bg-white px-3 py-1.5 text-xs font-semibold text-slate-700 hover:bg-slate-50 transition shadow-2xs"
              title="Copy mã chia sẻ bộ sưu tập"
            >
              {copiedKey === selectedCollection.sharedKey ? (
                <>
                  <Check className="size-3.5 text-emerald-600" />
                  <span className="text-emerald-600">Đã chép mã</span>
                </>
              ) : (
                <>
                  <Copy className="size-3.5 text-slate-500" />
                  <span>Chia sẻ key</span>
                </>
              )}
            </button>

            {/* Save to chat button */}
            <button
              type="button"
              onClick={() => handleToggleSave(selectedCollection.id)}
              className={cn(
                "flex items-center gap-1.5 rounded-xl px-3 py-1.5 text-xs font-semibold transition shadow-xs",
                selectedCollection.isSaved
                  ? "bg-slate-100 text-slate-700 hover:bg-slate-200"
                  : "bg-[#0052cc] text-white hover:bg-[#0043a8]",
              )}
            >
              <Bookmark className="size-3.5" />
              <span>{selectedCollection.isSaved ? "Bỏ lưu" : "Lưu vào chat"}</span>
            </button>

            {/* Delete (if my collection) */}
            {selectedCollection.isMyCollection && (
              <button
                type="button"
                onClick={() => handleDeleteCollection(selectedCollection.id)}
                className="grid size-8 place-items-center rounded-xl text-slate-400 hover:bg-rose-50 hover:text-rose-600 transition"
                title="Xóa bộ sưu tập"
              >
                <Trash2 className="size-4" />
              </button>
            )}
          </div>
        </header>

        {/* Content Body: Stickers Grid & Add Form */}
        <div className="flex-1 overflow-y-auto p-6 space-y-6">
          {/* Add Sticker Bar (if my collection) */}
          {selectedCollection.isMyCollection && (
            <div className="rounded-2xl border border-slate-200/90 bg-white p-4 shadow-xs">
              <h3 className="text-xs font-bold text-slate-900 mb-2">Thêm sticker mới vào bộ này</h3>
              <div className="flex items-center gap-3 flex-wrap">
                <input
                  type="text"
                  value={newStickerEmoji}
                  onChange={(e) => setNewStickerEmoji(e.target.value)}
                  placeholder="Emoji"
                  className="w-20 rounded-xl bg-[#f8fafc] px-3 py-2 text-center text-lg outline-none border border-slate-200 focus:border-[#0052cc]"
                />
                <input
                  type="text"
                  value={newStickerName}
                  onChange={(e) => setNewStickerName(e.target.value)}
                  onKeyDown={(e) => e.key === "Enter" && handleAddSticker()}
                  placeholder="Tên sticker (vd: Cười mỉm, Quẩy...)"
                  className="flex-1 min-w-[200px] rounded-xl bg-[#f8fafc] px-3.5 py-2 text-xs text-slate-800 placeholder:text-slate-400 outline-none border border-slate-200 focus:border-[#0052cc]"
                />
                <button
                  type="button"
                  onClick={handleAddSticker}
                  disabled={!newStickerName.trim()}
                  className="flex items-center gap-1.5 rounded-xl bg-[#0052cc] px-4 py-2 text-xs font-semibold text-white hover:bg-[#0043a8] transition shadow-xs disabled:opacity-50"
                >
                  <Plus className="size-4" />
                  <span>Thêm sticker</span>
                </button>
              </div>
            </div>
          )}

          {/* Stickers Grid */}
          <div className="rounded-2xl border border-slate-200/90 bg-white p-6 shadow-xs">
            <div className="flex items-center justify-between pb-4 border-b border-slate-100">
              <span className="text-xs font-bold text-slate-900">
                Danh sách sticker ({selectedCollection.stickers.length})
              </span>
              <span className="text-[11px] text-slate-400">
                Key: {selectedCollection.sharedKey.slice(0, 13)}...
              </span>
            </div>

            {selectedCollection.stickers.length === 0 ? (
              <div className="py-12 text-center text-xs text-slate-400">
                Chưa có sticker nào trong bộ sưu tập này.
              </div>
            ) : (
              <div className="grid grid-cols-3 sm:grid-cols-4 md:grid-cols-6 gap-4 pt-4">
                {selectedCollection.stickers.map((stk) => (
                  <div
                    key={stk.id}
                    className="relative group flex flex-col items-center justify-center rounded-2xl border border-slate-100 bg-[#f8fafc] p-4 hover:border-slate-300 hover:bg-white hover:shadow-xs transition"
                  >
                    <div className="text-4xl py-2">{stk.emoji}</div>
                    <span className="text-xs font-medium text-slate-700 truncate max-w-full mt-1">
                      {stk.name}
                    </span>

                    {/* Delete sticker button (if my collection) */}
                    {selectedCollection.isMyCollection && (
                      <button
                        type="button"
                        onClick={() => handleDeleteSticker(stk.id)}
                        className="absolute top-1.5 right-1.5 opacity-0 group-hover:opacity-100 grid size-6 place-items-center rounded-lg bg-white border border-slate-200 text-rose-500 hover:bg-rose-50 transition shadow-2xs"
                        title="Xóa sticker này"
                      >
                        <Trash2 className="size-3" />
                      </button>
                    )}
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </main>

      {/* Modal Tạo Bộ Sưu Tập Mới */}
      {showCreateModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-2xs p-4">
          <div className="w-full max-w-md rounded-2xl bg-white p-6 shadow-xl border border-slate-200 space-y-4">
            <div className="flex items-center justify-between">
              <h3 className="text-base font-bold text-slate-900">Tạo bộ sưu tập Sticker</h3>
              <button
                type="button"
                onClick={() => setShowCreateModal(false)}
                className="text-slate-400 hover:text-slate-700"
              >
                ✕
              </button>
            </div>

            <div className="space-y-3">
              <div>
                <label className="block text-xs font-semibold text-slate-700 mb-1">
                  Biểu tượng đại diện (Cover Emoji)
                </label>
                <input
                  type="text"
                  value={newColEmoji}
                  onChange={(e) => setNewColEmoji(e.target.value)}
                  className="w-20 rounded-xl bg-[#f8fafc] px-3 py-2 text-center text-xl border border-slate-200 focus:border-[#0052cc] outline-none"
                />
              </div>

              <div>
                <label className="block text-xs font-semibold text-slate-700 mb-1">
                  Tên bộ sưu tập
                </label>
                <input
                  type="text"
                  value={newColName}
                  onChange={(e) => setNewColName(e.target.value)}
                  placeholder="Ví dụ: Minecraft Pixel Emotes"
                  className="w-full rounded-xl bg-[#f8fafc] px-3.5 py-2 text-xs text-slate-800 placeholder:text-slate-400 border border-slate-200 focus:border-[#0052cc] outline-none"
                />
              </div>

              <div>
                <label className="block text-xs font-semibold text-slate-700 mb-1">
                  Mô tả bộ sưu tập
                </label>
                <textarea
                  value={newColDesc}
                  onChange={(e) => setNewColDesc(e.target.value)}
                  placeholder="Mô tả ngắn gọn về bộ nhãn dán..."
                  rows={2}
                  className="w-full resize-none rounded-xl bg-[#f8fafc] px-3.5 py-2 text-xs text-slate-800 placeholder:text-slate-400 border border-slate-200 focus:border-[#0052cc] outline-none"
                />
              </div>
            </div>

            <div className="flex items-center justify-end gap-2 pt-2">
              <button
                type="button"
                onClick={() => setShowCreateModal(false)}
                className="rounded-xl border border-slate-200 px-4 py-2 text-xs font-semibold text-slate-600 hover:bg-slate-50 transition"
              >
                Hủy
              </button>
              <button
                type="button"
                onClick={handleCreateCollection}
                disabled={!newColName.trim()}
                className="rounded-xl bg-[#0052cc] px-4 py-2 text-xs font-semibold text-white hover:bg-[#0043a8] transition shadow-xs disabled:opacity-50"
              >
                Tạo mới
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
