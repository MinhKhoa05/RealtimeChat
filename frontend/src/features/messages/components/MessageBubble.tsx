
import { cn } from "@/lib/utils";

type MessageBubbleProps = {
  content: string;
  time: string;
  isMine?: boolean;
  isRead?: boolean;
  showMetadata?: boolean;
  className?: string;
};

export function MessageBubble({
  content,
  time,
  isMine = false,
  isRead = false,
  showMetadata = true,
  className,
}: MessageBubbleProps) {
  return (
    <div
      className={cn(
        "flex w-fit min-w-0 max-w-[75%] flex-col",
        isMine ? "ml-auto items-end" : "mr-auto items-start",
        className,
      )}
    >
      <div
        className={cn(
          "min-w-0 rounded-xl border px-3 py-2",
          isMine
            ? "rounded-br-sm border-[#e6d6bc] bg-[#f2e7d6] text-[#514638]"
            : "rounded-bl-sm border-[#e7e5e1] bg-white text-[#37352f]",
        )}
      >
        <p className="whitespace-pre-wrap wrap-break-words text-sm leading-relaxed">
          {content}
        </p>

        <div className="mt-1 text-[11px] leading-none text-[#8a8379]">
          {time}
        </div>
      </div>

      {isMine && showMetadata && (
        <span
          className={cn(
            "mt-0.5 px-1 text-[10px] leading-tight",
            isRead ? "text-[#648f83]" : "text-[#8a8379]",
          )}
          title={isRead ? "Đã đọc" : "Đã gửi"}
        >
          {isRead ? "Đã đọc" : "Đã gửi"}
        </span>
      )}
    </div>
  );
}
