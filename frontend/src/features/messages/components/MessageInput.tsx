
import { useState, type FormEvent, type KeyboardEvent } from "react";
import { Paperclip, Send, Smile } from "lucide-react";

type MessageInputProps = {
  onSend: (content: string) => void;
  disabled?: boolean;
  placeholder?: string;
};

export function MessageInput({
  onSend,
  disabled = false,
  placeholder = "Type a message...",
}: MessageInputProps) {
  const [content, setContent] = useState("");

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const trimmedContent = content.trim();

    if (!trimmedContent || disabled) return;

    onSend(trimmedContent);
    setContent("");
  };

  const handleKeyDown = (event: KeyboardEvent<HTMLTextAreaElement>) => {
    if (event.key === "Enter" && !event.shiftKey) {
      event.preventDefault();
      event.currentTarget.form?.requestSubmit();
    }
  };

  return (
    <form
      onSubmit={handleSubmit}
      className="flex shrink-0 items-end gap-2 border-t border-[#e7e2da] bg-[#fffdf9] px-3 py-3 sm:px-4"
    >
      <button
        type="button"
        disabled={disabled}
        aria-label="Attach file"
        className="grid size-11 shrink-0 place-items-center rounded-xl border border-[#e7e2da] bg-white text-[#6f6b65] transition hover:bg-[#f2e8d7] disabled:opacity-50"
      >
        <Paperclip size={19} />
      </button>

      <div className="flex min-h-11 min-w-0 flex-1 items-end gap-2 rounded-xl border border-[#e7e2da] bg-white px-3 py-2 transition focus-within:border-[#c5a16b]">
        <textarea
          value={content}
          onChange={(event) => setContent(event.target.value)}
          onKeyDown={handleKeyDown}
          disabled={disabled}
          placeholder={placeholder}
          aria-label="Message content"
          rows={1}
          className="max-h-32 min-h-6 min-w-0 flex-1 resize-none bg-transparent text-sm leading-6 text-[#171717] outline-none placeholder:text-[#99938a] disabled:opacity-50"
        />

        <button
          type="button"
          disabled={disabled}
          aria-label="Add emoji"
          className="mb-0.5 shrink-0 text-[#8a8379] transition hover:text-[#c5a16b] disabled:opacity-50"
        >
          <Smile size={20} />
        </button>
      </div>

      <button
        type="submit"
        disabled={disabled || !content.trim()}
        aria-label="Send message"
        className="grid size-11 shrink-0 place-items-center rounded-xl border border-[#c5a16b] bg-[#c5a16b] text-white transition hover:bg-[#b18d57] disabled:cursor-not-allowed disabled:opacity-50"
      >
        <Send size={18} />
      </button>
    </form>
  );
}
