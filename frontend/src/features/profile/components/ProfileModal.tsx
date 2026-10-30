import { useState, type FormEvent } from "react";
import { X, Camera, Edit2, Check } from "lucide-react";
import { PixelBlockIcon } from "@/components/PixelBlockIcon";
import type { UserProfile } from "../data/profileData";

interface ProfileModalProps {
  isOpen: boolean;
  onClose: () => void;
  profile: UserProfile;
  onUpdateProfile: (updated: UserProfile) => void;
}

export function ProfileModal({
  isOpen,
  onClose,
  profile,
  onUpdateProfile,
}: ProfileModalProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState<UserProfile>(profile);
  const [showToast, setShowToast] = useState(false);

  if (!isOpen) return null;

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    onUpdateProfile(formData);
    setIsEditing(false);
    setShowToast(true);
    setTimeout(() => setShowToast(false), 2000);
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/40 select-none animate-in fade-in duration-150">
      {/* Zalo-style popup card (380px) */}
      <div className="relative flex flex-col w-full max-w-[390px] overflow-hidden rounded-2xl bg-white shadow-2xl border border-slate-200">
        {/* Cover Photo */}
        <div className="relative h-32 w-full bg-[#0052cc] shrink-0">
          {/* Close button */}
          <button
            type="button"
            onClick={onClose}
            className="absolute top-3 right-3 grid size-7 place-items-center rounded-full bg-black/30 hover:bg-black/50 text-white transition cursor-pointer"
          >
            <X className="size-4" />
          </button>

          {/* Change cover icon */}
          <button
            type="button"
            title="Đổi ảnh bìa"
            className="absolute bottom-2.5 right-2.5 flex items-center gap-1 px-2.5 py-1 rounded-md bg-black/35 hover:bg-black/50 text-white text-[11px] font-medium backdrop-blur-xs transition cursor-pointer"
          >
            <Camera className="size-3" />
            <span>Đổi ảnh bìa</span>
          </button>
        </div>

        {/* Avatar & Name Header */}
        <div className="relative px-5 pt-0 pb-3 border-b border-slate-100">
          <div className="flex items-end justify-between -mt-10 mb-2">
            <div className="relative group">
              <PixelBlockIcon
                type={formData.avatarType}
                className="size-20 rounded-2xl ring-4 ring-white shadow-md bg-white shrink-0"
              />
              <button
                type="button"
                title="Đổi ảnh đại diện"
                className="absolute bottom-0 right-0 grid size-6 place-items-center rounded-full bg-white text-slate-700 shadow-md hover:bg-slate-50 transition cursor-pointer border border-slate-200"
              >
                <Camera className="size-3 text-slate-600" />
              </button>
            </div>

            {/* Pencil button or save */}
            {!isEditing && (
              <button
                type="button"
                onClick={() => setIsEditing(true)}
                className="flex items-center gap-1.5 px-3 py-1.5 rounded-lg border border-slate-200 text-xs font-semibold text-slate-700 hover:bg-slate-50 transition cursor-pointer"
              >
                <Edit2 className="size-3.5" />
                <span>Chỉnh sửa</span>
              </button>
            )}
          </div>

          <div>
            <h2 className="text-base font-bold text-slate-900 leading-tight">
              {formData.displayName}
            </h2>
            <p className="text-xs text-slate-400 mt-0.5">@{formData.username}</p>
          </div>
        </div>

        {/* Body: Form or Info Table */}
        <div className="p-5 overflow-y-auto">
          {isEditing ? (
            <form onSubmit={handleSubmit} className="space-y-3.5 text-xs">
              <div>
                <label className="block font-semibold text-slate-600 mb-1">
                  Tên hiển thị
                </label>
                <input
                  type="text"
                  value={formData.displayName}
                  onChange={(e) =>
                    setFormData({ ...formData, displayName: e.target.value })
                  }
                  className="w-full rounded-lg border border-slate-200 px-3 py-2 text-slate-800 outline-none focus:border-[#0052cc]"
                />
              </div>

              <div>
                <label className="block font-semibold text-slate-600 mb-1">
                  Giới tính
                </label>
                <div className="flex gap-4 pt-1">
                  {(["Nam", "Nữ", "Khác"] as const).map((gender) => (
                    <label key={gender} className="flex items-center gap-1.5 cursor-pointer">
                      <input
                        type="radio"
                        name="gender"
                        checked={formData.gender === gender}
                        onChange={() => setFormData({ ...formData, gender })}
                        className="accent-[#0052cc]"
                      />
                      <span className="text-slate-700">{gender}</span>
                    </label>
                  ))}
                </div>
              </div>

              <div>
                <label className="block font-semibold text-slate-600 mb-1">
                  Ngày sinh
                </label>
                <input
                  type="text"
                  value={formData.birthday}
                  onChange={(e) =>
                    setFormData({ ...formData, birthday: e.target.value })
                  }
                  placeholder="DD/MM/YYYY"
                  className="w-full rounded-lg border border-slate-200 px-3 py-2 text-slate-800 outline-none focus:border-[#0052cc]"
                />
              </div>

              <div>
                <label className="block font-semibold text-slate-600 mb-1">
                  Số điện thoại
                </label>
                <input
                  type="text"
                  value={formData.phone}
                  onChange={(e) =>
                    setFormData({ ...formData, phone: e.target.value })
                  }
                  className="w-full rounded-lg border border-slate-200 px-3 py-2 text-slate-800 outline-none focus:border-[#0052cc]"
                />
              </div>

              {/* Action buttons */}
              <div className="flex justify-end gap-2 pt-3 border-t border-slate-100">
                <button
                  type="button"
                  onClick={() => {
                    setFormData(profile);
                    setIsEditing(false);
                  }}
                  className="px-3.5 py-1.5 rounded-lg border border-slate-200 text-xs font-semibold text-slate-600 hover:bg-slate-50 transition cursor-pointer"
                >
                  Hủy
                </button>
                <button
                  type="submit"
                  className="px-4 py-1.5 rounded-lg bg-[#0052cc] hover:bg-[#0043a8] text-xs font-semibold text-white shadow-xs transition cursor-pointer"
                >
                  Cập nhật
                </button>
              </div>
            </form>
          ) : (
            <div className="space-y-3 text-xs">
              <h3 className="font-bold text-slate-800 mb-2">Thông tin cá nhân</h3>

              <div className="flex items-center justify-between py-1.5 border-b border-slate-50">
                <span className="text-slate-500">Giới tính</span>
                <span className="font-semibold text-slate-800">{formData.gender}</span>
              </div>

              <div className="flex items-center justify-between py-1.5 border-b border-slate-50">
                <span className="text-slate-500">Ngày sinh</span>
                <span className="font-semibold text-slate-800">{formData.birthday}</span>
              </div>

              <div className="flex items-center justify-between py-1.5 border-b border-slate-50">
                <span className="text-slate-500">Điện thoại</span>
                <span className="font-semibold text-slate-800">{formData.phone}</span>
              </div>

              <div className="flex items-center justify-between py-1.5">
                <span className="text-slate-500">Khu vực</span>
                <span className="font-semibold text-slate-800">{formData.location}</span>
              </div>
            </div>
          )}
        </div>

        {/* Toast Notification */}
        {showToast && (
          <div className="absolute bottom-3 left-1/2 -translate-x-1/2 flex items-center gap-1.5 rounded-lg bg-slate-900 px-3 py-1.5 text-xs font-semibold text-white shadow-lg animate-in fade-in duration-150">
            <Check className="size-3.5 text-emerald-400" />
            <span>Cập nhật thông tin thành công</span>
          </div>
        )}
      </div>
    </div>
  );
}
