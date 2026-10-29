import React from "react";
import { cn } from "@/lib/utils";

interface PixelBlockIconProps {
  type: string;
  className?: string;
}

export const PixelBlockIcon: React.FC<PixelBlockIconProps> = ({ type, className }) => {
  switch (type) {
    case "grass_block":
    case "block_chat":
      return (
        <div
          className={cn(
            "relative flex items-center justify-center overflow-hidden rounded-xl shadow-xs border border-sky-500/30",
            className,
          )}
        >
          <svg viewBox="0 0 32 32" className="size-full shape-rendering-crisp" style={{ imageRendering: "pixelated" }}>
            {/* Dirt/Stone in deep blue/slate shades */}
            <rect width="32" height="32" fill="#1e3a8a" />
            <rect x="0" y="16" width="32" height="16" fill="#172554" />
            <rect x="4" y="20" width="4" height="4" fill="#0f172a" />
            <rect x="16" y="22" width="6" height="4" fill="#0f172a" />
            <rect x="24" y="18" width="4" height="4" fill="#0f172a" />
            <rect x="8" y="26" width="5" height="3" fill="#2563eb" />
            {/* Cyan glowing crystal top */}
            <rect x="0" y="0" width="32" height="10" fill="#0ea5e9" />
            <rect x="0" y="0" width="32" height="4" fill="#38bdf8" />
            <rect x="2" y="10" width="4" height="5" fill="#0ea5e9" />
            <rect x="3" y="10" width="2" height="6" fill="#0284c7" />
            <rect x="8" y="10" width="4" height="3" fill="#0ea5e9" />
            <rect x="14" y="10" width="5" height="6" fill="#0ea5e9" />
            <rect x="15" y="10" width="3" height="7" fill="#0284c7" />
            <rect x="21" y="10" width="4" height="3" fill="#0ea5e9" />
            <rect x="26" y="10" width="5" height="5" fill="#0ea5e9" />
            <rect x="27" y="10" width="2" height="6" fill="#0284c7" />
          </svg>
        </div>
      );

    case "diamond":
      return (
        <div
          className={cn(
            "relative flex items-center justify-center overflow-hidden rounded-xl bg-sky-50 border border-sky-300 shadow-2xs",
            className,
          )}
        >
          <svg viewBox="0 0 32 32" className="size-full shape-rendering-crisp" style={{ imageRendering: "pixelated" }}>
            <polygon
              points="16,3 28,11 16,29 4,11"
              fill="#0ea5e9"
              stroke="#0284c7"
              strokeWidth="1.5"
            />
            <polygon points="16,3 23,11 16,26 9,11" fill="#38bdf8" />
            <polygon points="16,3 20,11 16,20 12,11" fill="#bae6fd" />
          </svg>
        </div>
      );

    case "chest":
      return (
        <div
          className={cn(
            "relative flex items-center justify-center overflow-hidden rounded-xl bg-[#0369a1] border border-sky-900 shadow-2xs",
            className,
          )}
        >
          <svg viewBox="0 0 32 32" className="size-full shape-rendering-crisp" style={{ imageRendering: "pixelated" }}>
            <rect x="3" y="5" width="26" height="22" fill="#0284c7" rx="1" />
            <rect x="4" y="6" width="24" height="4" fill="#075985" />
            <rect x="4" y="22" width="24" height="4" fill="#075985" />
            <rect x="4" y="10" width="24" height="2" fill="#0c4a6e" />
            <rect x="14" y="9" width="4" height="6" fill="#e0f2fe" />
          </svg>
        </div>
      );

    case "creeper":
      return (
        <div
          className={cn(
            "relative flex items-center justify-center overflow-hidden rounded-xl bg-[#0284c7] border border-sky-900 shadow-2xs",
            className,
          )}
        >
          <svg viewBox="0 0 32 32" className="size-full shape-rendering-crisp" style={{ imageRendering: "pixelated" }}>
            <rect width="32" height="32" fill="#0284c7" />
            <rect x="6" y="8" width="6" height="6" fill="#082f49" />
            <rect x="20" y="8" width="6" height="6" fill="#082f49" />
            <rect x="12" y="14" width="8" height="10" fill="#082f49" />
            <rect x="8" y="18" width="4" height="9" fill="#082f49" />
            <rect x="20" y="18" width="4" height="9" fill="#082f49" />
          </svg>
        </div>
      );

    case "steve":
      return (
        <div
          className={cn(
            "relative flex items-center justify-center overflow-hidden rounded-xl bg-[#fed7aa] border border-amber-300 shadow-2xs",
            className,
          )}
        >
          <svg viewBox="0 0 32 32" className="size-full shape-rendering-crisp" style={{ imageRendering: "pixelated" }}>
            <rect width="32" height="32" fill="#fed7aa" />
            <rect x="0" y="0" width="32" height="10" fill="#451a03" />
            <rect x="0" y="10" width="5" height="8" fill="#451a03" />
            <rect x="27" y="10" width="5" height="8" fill="#451a03" />
            <rect x="5" y="13" width="6" height="3" fill="#ffffff" />
            <rect x="7" y="13" width="4" height="3" fill="#0284c7" />
            <rect x="21" y="13" width="6" height="3" fill="#ffffff" />
            <rect x="21" y="13" width="4" height="3" fill="#0284c7" />
            <rect x="12" y="16" width="8" height="4" fill="#d97706" />
            <rect x="10" y="21" width="12" height="3" fill="#78350f" />
          </svg>
        </div>
      );

    case "user_khoa":
      return (
        <div
          className={cn(
            "relative flex items-center justify-center overflow-hidden rounded-xl bg-[#ffedd5] border border-sky-300 shadow-2xs",
            className,
          )}
        >
          <svg viewBox="0 0 32 32" className="size-full shape-rendering-crisp" style={{ imageRendering: "pixelated" }}>
            <rect width="32" height="32" fill="#fed7aa" />
            <rect x="0" y="0" width="32" height="12" fill="#0f172a" />
            <rect x="0" y="12" width="6" height="9" fill="#0f172a" />
            <rect x="26" y="12" width="6" height="9" fill="#0f172a" />
            <polygon points="6,12 12,16 14,12" fill="#0f172a" />
            <polygon points="18,12 20,16 26,12" fill="#0f172a" />
            <rect x="6" y="14" width="6" height="5" fill="#0284c7" />
            <rect x="7" y="14" width="2" height="2" fill="#ffffff" />
            <rect x="20" y="14" width="6" height="5" fill="#0284c7" />
            <rect x="21" y="14" width="2" height="2" fill="#ffffff" />
            <rect x="13" y="23" width="6" height="2" fill="#ea580c" />
          </svg>
        </div>
      );

    case "pink_cat":
      return (
        <div
          className={cn(
            "relative flex items-center justify-center overflow-hidden rounded-xl bg-pink-50 border border-pink-200 shadow-2xs",
            className,
          )}
        >
          <svg viewBox="0 0 32 32" className="size-full shape-rendering-crisp" style={{ imageRendering: "pixelated" }}>
            <rect width="32" height="32" fill="#fbcfe8" />
            <rect x="2" y="2" width="7" height="7" fill="#f472b6" />
            <rect x="23" y="2" width="7" height="7" fill="#f472b6" />
            <rect x="6" y="14" width="5" height="5" fill="#334155" />
            <rect x="7" y="14" width="2" height="2" fill="#ffffff" />
            <rect x="21" y="14" width="5" height="5" fill="#334155" />
            <rect x="22" y="14" width="2" height="2" fill="#ffffff" />
            <rect x="15" y="18" width="2" height="2" fill="#e11d48" />
          </svg>
        </div>
      );

    case "dog":
    default:
      return (
        <div
          className={cn(
            "relative flex items-center justify-center overflow-hidden rounded-xl bg-amber-50 border border-amber-200 shadow-2xs",
            className,
          )}
        >
          <svg viewBox="0 0 32 32" className="size-full shape-rendering-crisp" style={{ imageRendering: "pixelated" }}>
            <rect width="32" height="32" fill="#fef3c7" />
            <rect x="0" y="0" width="12" height="18" fill="#b45309" />
            <rect x="20" y="0" width="12" height="12" fill="#b45309" />
            <rect x="6" y="13" width="4" height="4" fill="#0f172a" />
            <rect x="22" y="13" width="4" height="4" fill="#0f172a" />
            <rect x="11" y="16" width="10" height="9" fill="#ffffff" />
            <rect x="14" y="17" width="4" height="3" fill="#0f172a" />
          </svg>
        </div>
      );
  }
};
