import { Phone, PhoneOff, Video } from "lucide-react";
import { PixelBlockIcon } from "@/components/PixelBlockIcon";
import type { ActiveCallState } from "../types";

interface IncomingCallBannerProps {
  callState: ActiveCallState;
  onAccept: () => void;
  onDecline: () => void;
}

export function IncomingCallBanner({
  callState,
  onAccept,
  onDecline,
}: IncomingCallBannerProps) {
  const isVideo = callState.type === "video";

  return (
    <div className="fixed top-6 right-6 z-50 flex w-96 max-w-[calc(100vw-3rem)] items-center justify-between rounded-3xl border border-slate-700/60 bg-gradient-to-r from-slate-900 via-slate-850 to-slate-900 p-4 text-white shadow-2xl backdrop-blur-xl animate-in slide-in-from-top-4 duration-300 ring-2 ring-emerald-500/20">
      <div className="flex items-center gap-3.5 min-w-0">
        {/* Caller Avatar with Ring Animation */}
        <div className="relative shrink-0">
          <div className="absolute -inset-1 rounded-2xl bg-emerald-500/30 blur-xs animate-pulse" />
          <PixelBlockIcon
            type={callState.caller.avatarType}
            className="size-12 rounded-xl ring-2 ring-white/20 shadow-md"
          />
          <div className="absolute -bottom-1 -right-1 rounded-full bg-emerald-500 p-1 text-white shadow-xs">
            {isVideo ? <Video className="size-3" /> : <Phone className="size-3" />}
          </div>
        </div>

        {/* Info */}
        <div className="min-w-0 flex-1">
          <h4 className="text-sm font-bold text-white truncate">
            {callState.caller.name}
          </h4>
          <p className="flex items-center gap-1.5 text-xs text-emerald-400 font-medium">
            <span className="size-2 rounded-full bg-emerald-400 animate-ping shrink-0" />
            <span>
              {isVideo ? "Cuộc gọi video đến..." : "Cuộc gọi thoại đến..."}
            </span>
          </p>
        </div>
      </div>

      {/* Accept & Decline Buttons */}
      <div className="flex items-center gap-2 shrink-0 ml-3">
        {/* Decline */}
        <button
          type="button"
          onClick={onDecline}
          title="Từ chối cuộc gọi"
          className="flex size-10 items-center justify-center rounded-xl bg-rose-600 text-white shadow-md shadow-rose-600/30 hover:bg-rose-500 active:scale-95 transition cursor-pointer"
        >
          <PhoneOff className="size-4.5" />
        </button>

        {/* Accept */}
        <button
          type="button"
          onClick={onAccept}
          title="Trả lời"
          className="flex size-10 items-center justify-center rounded-xl bg-emerald-500 text-white shadow-md shadow-emerald-500/30 hover:bg-emerald-400 active:scale-95 transition cursor-pointer animate-bounce"
        >
          {isVideo ? <Video className="size-4.5" /> : <Phone className="size-4.5" />}
        </button>
      </div>
    </div>
  );
}
