import { useState, useEffect } from "react";
import {
  Mic,
  MicOff,
  Video,
  VideoOff,
  PhoneOff,
  SwitchCamera,
  Maximize2,
  Minimize2,
  Sparkles,
  Signal,
} from "lucide-react";
import { cn } from "@/lib/utils";
import { PixelBlockIcon } from "@/components/PixelBlockIcon";
import type { ActiveCallState } from "../types";

interface VideoCallModalProps {
  callState: ActiveCallState;
  onEndCall: () => void;
  onAcceptCall?: () => void;
}

export function VideoCallModal({
  callState,
  onEndCall,
}: VideoCallModalProps) {
  const [isMuted, setIsMuted] = useState(false);
  const [isVideoOff, setIsVideoOff] = useState(false);
  const [isRemoteVideoOff] = useState(false);
  const [cameraFacing, setCameraFacing] = useState<"front" | "back">("front");
  const [callDuration, setCallDuration] = useState(0);
  const [isMinimized, setIsMinimized] = useState(false);

  // Call duration counter
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
        return "Đang đổ chuông video...";
      case "outgoing_connecting":
        return "Đang thiết lập hình ảnh...";
      case "active":
        return formatDuration(callDuration);
      case "ended":
        return "Cuộc gọi đã kết thúc";
      default:
        return "Cuộc gọi video";
    }
  };

  const handleSwitchCamera = () => {
    setCameraFacing((prev) => (prev === "front" ? "back" : "front"));
  };

  if (isMinimized) {
    return (
      <div className="fixed bottom-6 right-6 z-50 flex w-72 flex-col overflow-hidden rounded-2xl border border-slate-700/80 bg-slate-900/95 shadow-2xl backdrop-blur-xl text-white transition-all animate-in slide-in-from-bottom-4 group">
        {/* Mini video display area */}
        <div className="relative aspect-video w-full overflow-hidden bg-gradient-to-b from-slate-800 to-slate-950 flex items-center justify-center">
          {/* Ambient background effect */}
          <div
            className="absolute inset-0 opacity-20 pointer-events-none"
            style={{
              backgroundImage:
                "linear-gradient(rgba(255, 255, 255, 0.1) 1px, transparent 1px), linear-gradient(90deg, rgba(255, 255, 255, 0.1) 1px, transparent 1px)",
              backgroundSize: "16px 16px",
            }}
          />

          {/* Caller avatar / feed */}
          <div className="relative flex flex-col items-center">
            <PixelBlockIcon
              type={callState.caller.avatarType}
              className="size-16 rounded-xl shadow-lg ring-2 ring-cyan-500/40"
            />
          </div>

          {/* Picture-in-picture mini preview of user */}
          <div className="absolute bottom-2 right-2 size-10 overflow-hidden rounded-lg border border-white/20 bg-slate-800 shadow-md">
            {isVideoOff ? (
              <div className="flex size-full items-center justify-center bg-slate-800 text-rose-400">
                <VideoOff className="size-4" />
              </div>
            ) : (
              <div className="flex size-full items-center justify-center bg-slate-850 p-1">
                <PixelBlockIcon type="user_khoa" className="size-8 rounded-md" />
              </div>
            )}
          </div>

          {/* Top overlay badge & action icons */}
          <div className="absolute top-2 left-2 right-2 flex items-center justify-between pointer-events-auto">
            <div className="flex items-center gap-1.5 rounded-lg bg-black/60 px-2 py-0.5 text-[10px] font-medium backdrop-blur-md">
              <span className="size-2 rounded-full bg-emerald-400 animate-pulse" />
              <span className="font-mono text-emerald-300">{statusText()}</span>
            </div>

            <div className="flex items-center gap-1">
              <button
                type="button"
                onClick={() => setIsMinimized(false)}
                title="Phóng to toàn màn hình"
                className="rounded-lg bg-black/60 p-1 text-slate-300 hover:bg-black/90 hover:text-white transition cursor-pointer"
              >
                <Maximize2 className="size-3.5" />
              </button>
            </div>
          </div>
        </div>

        {/* Mini Bottom Controls Bar */}
        <div className="flex items-center justify-between border-t border-slate-800/80 bg-slate-900/90 px-3 py-2">
          <div className="flex flex-col min-w-0 pr-2">
            <span className="text-xs font-bold text-white truncate">
              {callState.caller.name}
            </span>
            <span className="text-[10px] text-slate-400 truncate">
              {isMuted ? "Đã tắt mic" : "Đang phát trực tiếp"}
            </span>
          </div>

          <div className="flex items-center gap-1.5 shrink-0">
            {/* Mic toggle */}
            <button
              type="button"
              onClick={() => setIsMuted(!isMuted)}
              title={isMuted ? "Bật mic" : "Tắt mic"}
              className={cn(
                "rounded-lg p-1.5 transition cursor-pointer",
                isMuted
                  ? "bg-rose-500/20 text-rose-400 border border-rose-500/40"
                  : "bg-white/10 text-white hover:bg-white/20"
              )}
            >
              {isMuted ? <MicOff className="size-3.5" /> : <Mic className="size-3.5" />}
            </button>

            {/* Video toggle */}
            <button
              type="button"
              onClick={() => setIsVideoOff(!isVideoOff)}
              title={isVideoOff ? "Bật camera" : "Tắt camera"}
              className={cn(
                "rounded-lg p-1.5 transition cursor-pointer",
                isVideoOff
                  ? "bg-rose-500/20 text-rose-400 border border-rose-500/40"
                  : "bg-white/10 text-white hover:bg-white/20"
              )}
            >
              {isVideoOff ? (
                <VideoOff className="size-3.5" />
              ) : (
                <Video className="size-3.5" />
              )}
            </button>

            {/* End call */}
            <button
              type="button"
              onClick={onEndCall}
              title="Kết thúc cuộc gọi"
              className="rounded-lg bg-rose-600 p-1.5 text-white hover:bg-rose-500 transition cursor-pointer shadow-xs"
            >
              <PhoneOff className="size-3.5" />
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/90 backdrop-blur-md animate-in fade-in duration-200">
      <div className="relative flex h-full w-full flex-col overflow-hidden bg-slate-950">
        {/* Main Video Viewport (Remote) */}
        <div className="relative flex-1 overflow-hidden bg-gradient-to-b from-slate-900 via-slate-925 to-slate-950">
          {/* Top Bar inside Viewport */}
          <div className="absolute top-4 left-4 right-4 z-20 flex items-center justify-between">
            <div className="flex items-center gap-3 rounded-2xl bg-black/40 px-3.5 py-1.5 backdrop-blur-md border border-white/10 text-white">
              <div className="flex items-center gap-2">
                <span className="size-2.5 rounded-full bg-emerald-400 animate-pulse" />
                <span className="text-xs font-bold">{callState.caller.name}</span>
              </div>
              <span className="text-slate-400 text-xs">|</span>
              <span className="text-xs font-mono font-semibold text-emerald-300">
                {statusText()}
              </span>
              <Signal className="size-3.5 text-emerald-400" />
            </div>

            <button
              type="button"
              onClick={() => setIsMinimized(true)}
              title="Thu nhỏ"
              className="rounded-xl bg-black/40 p-2 text-slate-300 backdrop-blur-md border border-white/10 hover:bg-white/20 hover:text-white transition cursor-pointer"
            >
              <Minimize2 className="size-4" />
            </button>
          </div>

          {/* Remote Video Stream Content / Simulated Mock */}
          <div className="relative flex size-full items-center justify-center">
            {isRemoteVideoOff || callState.status !== "active" ? (
              /* Avatar Placeholder when video is off or connecting */
              <div className="flex flex-col items-center justify-center p-6 text-center animate-in fade-in duration-300">
                <div className="relative mb-4">
                  {callState.status === "active" && (
                    <div className="absolute inset-0 rounded-3xl bg-[#0052cc]/20 blur-xl animate-pulse" />
                  )}
                  <div className="relative rounded-3xl bg-slate-800 p-4 ring-2 ring-white/15 shadow-2xl">
                    <PixelBlockIcon
                      type={callState.caller.avatarType}
                      className="size-28 rounded-2xl sm:size-36 shadow-lg"
                    />
                  </div>
                </div>
                <h3 className="text-lg font-bold text-white sm:text-xl">
                  {callState.caller.name}
                </h3>
                <p className="mt-1 text-xs text-slate-400">
                  {callState.status === "active"
                    ? "Camera của đối phương đang tắt"
                    : statusText()}
                </p>
              </div>
            ) : (
              /* Simulated Modern Cyber/Pixel Animated Video Feed */
              <div className="relative flex size-full items-center justify-center bg-radial from-slate-800 via-slate-900 to-slate-950">
                {/* Visual Ambient Grid / Tech elements */}
                <div
                  className="absolute inset-0 opacity-15 pointer-events-none"
                  style={{
                    backgroundImage:
                      "linear-gradient(rgba(255, 255, 255, 0.08) 1px, transparent 1px), linear-gradient(90deg, rgba(255, 255, 255, 0.08) 1px, transparent 1px)",
                    backgroundSize: "32px 32px",
                  }}
                />

                {/* Simulated Persona Representation */}
                <div className="relative z-10 flex flex-col items-center">
                  <div className="relative group">
                    <div className="absolute -inset-4 rounded-full bg-cyan-500/10 blur-xl transition-all group-hover:bg-cyan-500/20" />
                    <PixelBlockIcon
                      type={callState.caller.avatarType}
                      className="size-36 rounded-2xl shadow-2xl ring-4 ring-cyan-500/30 sm:size-48"
                    />
                  </div>
                  <div className="mt-4 flex items-center gap-1.5 rounded-full bg-cyan-950/60 px-3 py-1 text-[11px] font-medium text-cyan-300 border border-cyan-500/30 backdrop-blur-xs">
                    <Sparkles className="size-3" />
                    <span>HD 1080p · 60 FPS Simulated Feed</span>
                  </div>
                </div>
              </div>
            )}
          </div>

          {/* Picture-in-Picture Local Self Preview */}
          <div className="absolute bottom-4 right-4 z-20 w-32 sm:w-44 rounded-2xl overflow-hidden border border-white/20 bg-slate-900/90 shadow-2xl backdrop-blur-md">
            <div className="relative aspect-4/3 w-full flex items-center justify-center bg-slate-800">
              {isVideoOff ? (
                <div className="flex flex-col items-center gap-1 text-slate-400">
                  <VideoOff className="size-6 text-rose-400" />
                  <span className="text-[10px] font-semibold">Camera đã tắt</span>
                </div>
              ) : (
                <div className="relative size-full flex items-center justify-center bg-slate-850 p-2">
                  <PixelBlockIcon
                    type="user_khoa"
                    className="size-16 rounded-xl shadow-md"
                  />
                  <div className="absolute bottom-1.5 left-2 flex items-center gap-1 text-[9px] font-bold text-white bg-black/60 px-1.5 py-0.5 rounded-md">
                    <span>Bạn ({cameraFacing === "front" ? "Trước" : "Sau"})</span>
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>

        {/* Bottom Call Control Bar */}
        <div className="flex h-20 shrink-0 items-center justify-center gap-3 sm:gap-6 border-t border-slate-800/80 bg-slate-900/90 px-6 backdrop-blur-md">
          {/* Toggle Mic */}
          <button
            type="button"
            onClick={() => setIsMuted(!isMuted)}
            title={isMuted ? "Bật micro" : "Tắt micro"}
            disabled={callState.status !== "active"}
            className={cn(
              "flex size-11 sm:size-12 items-center justify-center rounded-2xl transition-all shadow-md cursor-pointer disabled:opacity-40 active:scale-95",
              isMuted
                ? "bg-rose-500/20 border border-rose-500/40 text-rose-400"
                : "bg-white/10 hover:bg-white/20 text-white"
            )}
          >
            {isMuted ? <MicOff className="size-5" /> : <Mic className="size-5" />}
          </button>

          {/* Toggle Camera */}
          <button
            type="button"
            onClick={() => setIsVideoOff(!isVideoOff)}
            title={isVideoOff ? "Bật camera" : "Tắt camera"}
            disabled={callState.status !== "active"}
            className={cn(
              "flex size-11 sm:size-12 items-center justify-center rounded-2xl transition-all shadow-md cursor-pointer disabled:opacity-40 active:scale-95",
              isVideoOff
                ? "bg-rose-500/20 border border-rose-500/40 text-rose-400"
                : "bg-white/10 hover:bg-white/20 text-white"
            )}
          >
            {isVideoOff ? (
              <VideoOff className="size-5" />
            ) : (
              <Video className="size-5" />
            )}
          </button>

          {/* Switch Camera (UI toggle) */}
          <button
            type="button"
            onClick={handleSwitchCamera}
            title="Đổi camera trước/sau"
            disabled={isVideoOff || callState.status !== "active"}
            className="flex size-11 sm:size-12 items-center justify-center rounded-2xl bg-white/10 text-white hover:bg-white/20 transition-all shadow-md cursor-pointer disabled:opacity-40 active:scale-95"
          >
            <SwitchCamera className="size-5" />
          </button>

          {/* End Call Button */}
          <button
            type="button"
            onClick={onEndCall}
            title="Kết thúc cuộc gọi video"
            className="flex size-12 sm:size-14 items-center justify-center rounded-2xl bg-rose-600 text-white shadow-lg shadow-rose-600/30 hover:bg-rose-500 transition-all cursor-pointer active:scale-95"
          >
            <PhoneOff className="size-6" />
          </button>
        </div>
      </div>
    </div>
  );
}
