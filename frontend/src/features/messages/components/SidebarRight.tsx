import { useState } from "react";
import {
  Search,
  Bell,
  Settings,
  UserPlus,
  FileText,
  Download,
  ChevronRight,
  Pin,
  Trash2,
} from "lucide-react";
import {
  teamMembers,
  sampleFiles,
} from "../data/mockData";
import { PixelBlockIcon } from "./PixelBlockIcon";

export function SidebarRight() {
  const [notificationsOn, setNotificationsOn] = useState(true);

  return (
    <aside className="flex h-full w-[310px] shrink-0 flex-col overflow-y-auto border-l border-[#dbe6f5] bg-white text-slate-800 select-none">
      {/* Header */}
      <div className="flex h-16 shrink-0 items-center justify-between border-b border-[#f1f5f9] px-5 font-bold text-sm text-slate-900 tracking-tight">
        <span>Thông tin hội thoại</span>
      </div>

      <div className="p-4 space-y-5">
        {/* Top Group Avatar & Info */}
        <div className="flex flex-col items-center text-center">
          <PixelBlockIcon
            type="grass_block"
            className="size-16 rounded-2xl shadow-sm border border-[#0052cc]/20"
          />
          <h2 className="mt-3 text-base font-bold text-slate-900 tracking-tight">
            Team Project
          </h2>
          <p className="text-xs text-slate-500 mt-0.5">
            6 thành viên · Nhóm học tập & phát triển
          </p>
        </div>

        {/* 4 Icon Action Buttons */}
        <div className="grid grid-cols-4 gap-2 pt-1 pb-1">
          <button
            type="button"
            className="group flex flex-col items-center gap-1.5 rounded-xl p-2 hover:bg-[#edf5ff] transition"
          >
            <div className="grid size-9.5 place-items-center rounded-full bg-[#f1f5f9] text-slate-600 group-hover:bg-[#0052cc] group-hover:text-white transition duration-150">
              <UserPlus className="size-4" />
            </div>
            <span className="text-[11px] text-slate-600 group-hover:text-[#0052cc] font-medium">Thêm</span>
          </button>
          <button
            type="button"
            className="group flex flex-col items-center gap-1.5 rounded-xl p-2 hover:bg-[#edf5ff] transition"
          >
            <div className="grid size-9.5 place-items-center rounded-full bg-[#f1f5f9] text-slate-600 group-hover:bg-[#0052cc] group-hover:text-white transition duration-150">
              <Search className="size-4" />
            </div>
            <span className="text-[11px] text-slate-600 group-hover:text-[#0052cc] font-medium">Tìm kiếm</span>
          </button>
          <button
            type="button"
            onClick={() => setNotificationsOn(!notificationsOn)}
            className="group flex flex-col items-center gap-1.5 rounded-xl p-2 hover:bg-[#edf5ff] transition"
          >
            <div className="grid size-9.5 place-items-center rounded-full bg-[#f1f5f9] text-slate-600 group-hover:bg-[#0052cc] group-hover:text-white transition duration-150">
              <Bell className="size-4" />
            </div>
            <span className="text-[11px] text-slate-600 group-hover:text-[#0052cc] font-medium">
              {notificationsOn ? "Bật chuông" : "Tắt chuông"}
            </span>
          </button>
          <button
            type="button"
            className="group flex flex-col items-center gap-1.5 rounded-xl p-2 hover:bg-[#edf5ff] transition"
          >
            <div className="grid size-9.5 place-items-center rounded-full bg-[#f1f5f9] text-slate-600 group-hover:bg-[#0052cc] group-hover:text-white transition duration-150">
              <Settings className="size-4" />
            </div>
            <span className="text-[11px] text-slate-600 group-hover:text-[#0052cc] font-medium">Quản lý</span>
          </button>
        </div>

        {/* Thành viên nhóm (6) */}
        <div className="border-t border-slate-100 pt-4">
          <div className="flex items-center justify-between pb-2.5 text-xs font-bold text-slate-900">
            <span>Thành viên nhóm (6)</span>
            <button
              type="button"
              className="flex items-center text-[11px] font-semibold text-[#0052cc] hover:underline"
            >
              Xem tất cả <ChevronRight className="size-3" />
            </button>
          </div>
          <div className="flex items-center gap-2 overflow-x-auto pb-1">
            {teamMembers.map((member) => (
              <div key={member.id} className="flex flex-col items-center gap-1 shrink-0">
                <PixelBlockIcon
                  type={member.avatarType}
                  className="size-9 rounded-xl shadow-2xs ring-1 ring-black/5"
                />
                <span className="text-[10px] text-slate-600 font-medium truncate max-w-10">
                  {member.name}
                </span>
              </div>
            ))}
          </div>
        </div>

        {/* Ảnh / Video */}
        <div className="border-t border-slate-100 pt-4">
          <div className="flex items-center justify-between pb-2 text-xs font-bold text-slate-900">
            <span>Ảnh / Video</span>
            <button
              type="button"
              className="flex items-center text-[11px] font-semibold text-[#0052cc] hover:underline"
            >
              Xem tất cả <ChevronRight className="size-3" />
            </button>
          </div>
          <div className="grid grid-cols-4 gap-1.5">
            <div className="aspect-square rounded-xl bg-blue-600 flex items-center justify-center text-white text-xs font-semibold shadow-2xs hover:opacity-90 transition cursor-pointer">
              🌄
            </div>
            <div className="aspect-square rounded-xl bg-sky-500 flex items-center justify-center text-white text-xs font-semibold shadow-2xs hover:opacity-90 transition cursor-pointer">
              🌲
            </div>
            <div className="aspect-square rounded-xl bg-amber-500 flex items-center justify-center text-white text-xs font-semibold shadow-2xs hover:opacity-90 transition cursor-pointer">
              🌇
            </div>
            <div className="aspect-square rounded-xl bg-slate-100 flex items-center justify-center text-slate-500 text-xs font-semibold hover:bg-slate-200 transition cursor-pointer">
              +8
            </div>
          </div>
        </div>

        {/* File & Tài liệu */}
        <div className="border-t border-slate-100 pt-4">
          <div className="flex items-center justify-between pb-2 text-xs font-bold text-slate-900">
            <span>File & Tài liệu</span>
            <button
              type="button"
              className="flex items-center text-[11px] font-semibold text-[#0052cc] hover:underline"
            >
              Xem tất cả <ChevronRight className="size-3" />
            </button>
          </div>
          <div className="space-y-1.5">
            {sampleFiles.map((file) => (
              <div
                key={file.id}
                className="flex items-center justify-between rounded-xl bg-[#f8fafc] p-2 hover:bg-[#edf5ff] transition cursor-pointer border border-slate-100"
              >
                <div className="flex items-center gap-2.5 min-w-0">
                  <div className="flex size-8 shrink-0 items-center justify-center rounded-lg bg-[#0052cc] text-white shadow-2xs">
                    <FileText className="size-4 text-white" />
                  </div>
                  <div className="min-w-0">
                    <p className="truncate text-xs font-semibold text-slate-800">
                      {file.name}
                    </p>
                    <p className="text-[10px] text-slate-400">{file.size}</p>
                  </div>
                </div>
                <button
                  type="button"
                  className="grid size-7 place-items-center rounded-lg text-slate-400 hover:bg-white hover:text-[#0052cc] transition shadow-2xs"
                  title="Tải về"
                >
                  <Download className="size-3.5" />
                </button>
              </div>
            ))}
          </div>
        </div>

        {/* Thiết lập bảo mật & Tùy chọn khác */}
        <div className="border-t border-slate-100 pt-4 space-y-2">
          <div className="flex items-center justify-between py-1 text-xs text-slate-700 hover:text-slate-900 cursor-pointer">
            <div className="flex items-center gap-2">
              <Pin className="size-4 text-slate-400" />
              <span>Ghim hội thoại</span>
            </div>
            <input type="checkbox" className="size-3.5 rounded accent-[#0052cc]" />
          </div>

          <div className="flex items-center justify-between py-1 text-xs text-rose-600 hover:text-rose-700 cursor-pointer">
            <div className="flex items-center gap-2">
              <Trash2 className="size-4 text-rose-500" />
              <span>Xóa lịch sử trò chuyện</span>
            </div>
          </div>
        </div>
      </div>
    </aside>
  );
}
