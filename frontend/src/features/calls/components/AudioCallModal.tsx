import { useState, useEffect } from "react";
import {
  Mic,
  MicOff,
  Volume2,
  VolumeX,
  PhoneOff,
  Minimize2,
  Maximize2,
  Radio,
} from "lucide-react";
import { cn } from "@/lib/utils";
import { PixelBlockIcon } from "@/components/PixelBlockIcon";
import type { ActiveCallState } from "../types";

interface AudioCallModalProps {
  callState: ActiveCallState;
  onEndCall: () => void;
  onAcceptCall?: () => void;
}

export function AudioCallModal({
  callState,
  onEndCall,
}: AudioCallModalProps) {
  const [isMuted, setIsMuted] = useState(false);
  const [isSpeakerOn, setIsSpeakerOn] = useState(true);
  const [callDuration, setCallDuration] = useState(0);
  const [isMinimized, setIsMinimized] = useState(false);

  // Timer for active call
  useEffect(() => {
    if (callState.status !== "active") return;
    const interval = setInterval(() => {
      setCallDuration((prev) => prev + 1);
    }, 1000);
    return () => clearInterval(interval);
  }, [callState.status]);

  const formatDuration = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins.toString().padStart(2, "0")}:${secs.toString().padStart(2, "0")}`;
  };

  const statusText = () => {
    switch (callState.status) {
      case "outgoing_calling":
        return "Đang đổ chuông...";
      case "outgoing_connecting":
        return "Đang kết nối...";
      case "active":
        return formatDuration(callDuration);
      case "ended":
        return "Cuộc gọi đã kết thúc";
      default:
        return "Cuộc gọi thoại";
    }
  };

  if (isMinimized) {
    return (
      <div className="fixed bottom-6 right-6 z-50 flex items-center gap-3 rounded-2xl border border-slate-200 bg-white/95 p-3.5 shadow-2xl backdrop-blur-md transition-all animate-in slide-in-from-bottom-4">
        <div className="relative">
          <PixelBlockIcon
            type={callState.caller.avatarType}
            className="size-10 rounded-xl shadow-xs"
          />
          {callState.status === "active" && (
            <span className="absolute -bottom-0.5 -right-0.5 size-3 rounded-full border-2 border-white bg-emerald-500 animate-pulse" />
          )}
        </div>
        <div className="flex flex-col">
          <span className="text-xs font-bold text-slate-800 line-clamp-1 max-w-[120px]">
            {callState.caller.name}
          </span>
          <span className="text-[11px] font-mono text-emerald-600 font-semibold">
            {statusText()}
          </span>
        </div>
        <div className="flex items-center gap-1.5 ml-2">
          <button
            type="button"
            onClick={() => setIsMinimized(false)}
            title="Mở rộng"
            className="p-1.5 rounded-lg text-slate-500 hover:bg-slate-100 hover:text-slate-800 transition cursor-pointer"
          >
            <Maximize2 className="size-4" />
          </button>
          <button
            type="button"
            onClick={onEndCall}
            title="Kết thúc cuộc gọi"
            className="p-1.5 rounded-lg bg-rose-500 text-white hover:bg-rose-600 transition cursor-pointer"
          >
            <PhoneOff className="size-4" />
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/70 p-4 backdrop-blur-md animate-in fade-in duration-200">
      <div className="relative flex w-full max-w-sm flex-col items-center justify-between overflow-hidden rounded-3xl border border-slate-700/50 bg-gradient-to-b from-slate-900 via-slate-850 to-slate-950 p-6 text-white shadow-2xl sm:p-8">
        {/* Top Controls: Minimize */}
        <div className="flex w-full items-center justify-between text-slate-400">
          <div className="flex items-center gap-1.5 rounded-full bg-white/10 px-3 py-1 text-xs font-medium text-slate-300 backdrop-blur-xs">
            <Radio className="size-3 text-emerald-400 animate-pulse" />
            <span>BlockChat Audio</span>
          </div>
          <button
            type="button"
            onClick={() => setIsMinimized(true)}
            title="Thu nhỏ cửa sổ"
            className="rounded-xl p-2 text-slate-400 hover:bg-white/10 hover:text-white transition cursor-pointer"
          >
            <Minimize2 className="size-4" />
          </button>
        </div>

        {/* Center: Caller Avatar with Pulse Animation */}
        <div className="my-10 flex flex-col items-center text-center">
          <div className="relative flex items-center justify-center">
            {/* Animated Audio Wave Rings */}
            {callState.status === "active" && !isMuted && (
              <>
                <div className="absolute size-40 rounded-full border border-sky-400/20 animate-ping opacity-60" />
                <div className="absolute size-32 rounded-full bg-sky-500/10 animate-pulse" />
              </>
            )}
            {callState.status === "outgoing_calling" && (
              <div className="absolute size-36 rounded-full border-2 border-emerald-400/30 animate-pulse" />
            )}

            <div className="relative z-10 rounded-2xl bg-slate-800 p-2 shadow-xl ring-4 ring-white/10">
              <PixelBlockIcon
                type={callState.caller.avatarType}
                className="size-24 rounded-xl shadow-lg sm:size-28"
              />
            </div>
          </div>

          <h3 className="mt-5 text-xl font-bold tracking-tight text-white">
            {callState.caller.name}
          </h3>

          <p className="mt-1.5 flex items-center gap-1.5 text-sm font-semibold tracking-wide">
            {callState.status === "active" ? (
              <span className="font-mono text-emerald-400 bg-emerald-950/60 px-3 py-1 rounded-full border border-emerald-500/30">
                {formatDuration(callDuration)}
              </span>
            ) : (
              <span className="text-slate-400">{statusText()}</span>
            )}
          </p>

          {callState.caller.subtitle && (
            <span className="mt-1 text-xs text-slate-400">
              {callState.caller.subtitle}
            </span>
          )}
        </div>

        {/* Bottom Interactive Controls */}
        <div className="flex w-full items-center justify-center gap-5 pt-4">
          {/* Mute Button */}
          <button
            type="button"
            onClick={() => setIsMuted(!isMuted)}
            title={isMuted ? "Bật micro" : "Tắt micro"}
            disabled={callState.status !== "active"}
            className={cn(
              "flex flex-col items-center gap-1.5 text-xs font-medium transition cursor-pointer disabled:opacity-40",
              isMuted ? "text-rose-400" : "text-slate-300 hover:text-white"
            )}
          >
            <div
              className={cn(
                "flex size-12 items-center justify-center rounded-2xl transition-all shadow-md active:scale-95",
                isMuted
                  ? "bg-rose-500/20 border border-rose-500/40 text-rose-400"
                  : "bg-white/10 hover:bg-white/20 text-white"
              )}
            >
              {isMuted ? <MicOff className="size-5" /> : <Mic className="size-5" />}
            </div>
            <span>{isMuted ? "Bật mic" : "Tắt mic"}</span>
          </button>

          {/* End Call Button */}
          <button
            type="button"
            onClick={onEndCall}
            title="Kết thúc cuộc gọi"
            className="flex flex-col items-center gap-1.5 text-xs font-medium text-rose-300 hover:text-rose-200 transition cursor-pointer"
          >
            <div className="flex size-14 items-center justify-center rounded-2xl bg-rose-600 text-white shadow-lg shadow-rose-600/30 hover:bg-rose-500 active:scale-95 transition-all">
              <PhoneOff className="size-6" />
            </div>
            <span>Kết thúc</span>
          </button>

          {/* Speaker Button */}
          <button
            type="button"
            onClick={() => setIsSpeakerOn(!isSpeakerOn)}
            title={isSpeakerOn ? "Tắt loa ngoài" : "Bật loa ngoài"}
            disabled={callState.status !== "active"}
            className={cn(
              "flex flex-col items-center gap-1.5 text-xs font-medium transition cursor-pointer disabled:opacity-40",
              isSpeakerOn ? "text-slate-300 hover:text-white" : "text-slate-500"
            )}
          >
            <div
              className={cn(
                "flex size-12 items-center justify-center rounded-2xl transition-all shadow-md active:scale-95",
                isSpeakerOn
                  ? "bg-white/10 hover:bg-white/20 text-white"
                  : "bg-white/5 border border-white/10 text-slate-500"
              )}
            >
              {isSpeakerOn ? (
                <Volume2 className="size-5" />
              ) : (
                <VolumeX className="size-5" />
              )}
            </div>
            <span>{isSpeakerOn ? "Loa ngoài" : "Loa tai"}</span>
          </button>
        </div>
      </div>
    </div>
  );
}
