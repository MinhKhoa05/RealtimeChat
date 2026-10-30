import React from "react";
import { PixelBlockIcon } from "@/components/PixelBlockIcon";

interface AuthLayoutProps {
  children: React.ReactNode;
  title: string;
  subtitle: string;
}

export function AuthLayout({ children, title, subtitle }: AuthLayoutProps) {
  return (
    <div className="min-h-screen w-full bg-[#edf3fa] p-4 sm:p-6 select-none flex items-center justify-center">
      {/* Center Auth Card with distinct blue header section */}
      <div className="w-full max-w-[480px] overflow-hidden rounded-3xl border border-[#dbe6f5] bg-white shadow-[0_12px_36px_-12px_rgba(0,82,204,0.12)] transition-all">
        {/* Header: Blue background section */}
        <div className="bg-[#0052cc] px-8 py-7 sm:px-10 border-b border-[#0047b3] text-white">
          <div className="flex items-center gap-4">
            <PixelBlockIcon
              type="block_chat"
              className="size-14 rounded-2xl shadow-sm ring-2 ring-white/30 shrink-0"
            />
            <div>
              <h1 className="text-2xl sm:text-3xl font-extrabold tracking-tight text-white leading-tight">
                {title}
              </h1>
              <p className="text-xs sm:text-sm text-blue-100/90 font-medium mt-1">
                {subtitle}
              </p>
            </div>
          </div>
        </div>

        {/* Form Body */}
        <div className="px-8 py-8 sm:px-10 sm:py-9">{children}</div>
      </div>
    </div>
  );
}
