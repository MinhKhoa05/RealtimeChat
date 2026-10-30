import { useState, type FormEvent } from "react";
import { Mail, Lock, Eye, EyeOff, Loader2 } from "lucide-react";
import { cn } from "@/lib/utils";

interface LoginFormProps {
  onSuccess?: () => void;
  onSwitchToRegister: () => void;
}

export function LoginForm({ onSuccess, onSwitchToRegister }: LoginFormProps) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [rememberMe, setRememberMe] = useState(true);
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isGoogleLoading, setIsGoogleLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  // Form field validation errors
  const [errors, setErrors] = useState<{ email?: string; password?: string }>({});

  const validate = () => {
    const errs: { email?: string; password?: string } = {};
    if (!email.trim()) {
      errs.email = "Vui lòng nhập địa chỉ email";
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim())) {
      errs.email = "Định dạng email không hợp lệ";
    }

    if (!password) {
      errs.password = "Vui lòng nhập mật khẩu";
    } else if (password.length < 6) {
      errs.password = "Mật khẩu tối thiểu 6 ký tự";
    }

    setErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    setErrorMessage(null);

    if (!validate()) return;

    setIsLoading(true);

    setTimeout(() => {
      setIsLoading(false);
      if (email === "error@blockchat.io") {
        setErrorMessage("Email hoặc mật khẩu không chính xác.");
      } else {
        onSuccess?.();
      }
    }, 1000);
  };

  const handleGoogleLogin = () => {
    setErrorMessage(null);
    setIsGoogleLoading(true);

    setTimeout(() => {
      setIsGoogleLoading(false);
      onSuccess?.();
    }, 1200);
  };

  return (
    <div className="space-y-6">
      {/* Primary Form: Email & Password */}
      <form onSubmit={handleSubmit} className="space-y-4">
        {errorMessage && (
          <div className="rounded-xl border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-600">
            {errorMessage}
          </div>
        )}

        {/* Email Input */}
        <div>
          <label className="block text-sm font-semibold text-slate-700 mb-1.5">
            Email
          </label>
          <div
            className={cn(
              "flex items-center rounded-xl bg-white px-3.5 py-3 border border-slate-200 shadow-2xs transition-all",
              "focus-within:border-[#0052cc] focus-within:ring-2 focus-within:ring-[#0052cc]/15",
              errors.email && "border-rose-400 bg-rose-50/20",
            )}
          >
            <Mail className="size-4.5 text-slate-400 shrink-0" />
            <input
              type="email"
              value={email}
              disabled={isLoading || isGoogleLoading}
              onChange={(e) => {
                setEmail(e.target.value);
                if (errors.email) setErrors((prev) => ({ ...prev, email: undefined }));
              }}
              placeholder="name@example.com"
              className="ml-3 w-full bg-transparent text-sm text-slate-800 placeholder:text-slate-400 outline-none disabled:opacity-60"
            />
          </div>
          {errors.email && (
            <p className="mt-1.5 text-xs text-rose-500 font-medium">{errors.email}</p>
          )}
        </div>

        {/* Password Input */}
        <div>
          <label className="block text-sm font-semibold text-slate-700 mb-1.5">
            Mật khẩu
          </label>
          <div
            className={cn(
              "flex items-center rounded-xl bg-white px-3.5 py-3 border border-slate-200 shadow-2xs transition-all",
              "focus-within:border-[#0052cc] focus-within:ring-2 focus-within:ring-[#0052cc]/15",
              errors.password && "border-rose-400 bg-rose-50/20",
            )}
          >
            <Lock className="size-4.5 text-slate-400 shrink-0" />
            <input
              type={showPassword ? "text" : "password"}
              value={password}
              disabled={isLoading || isGoogleLoading}
              onChange={(e) => {
                setPassword(e.target.value);
                if (errors.password) setErrors((prev) => ({ ...prev, password: undefined }));
              }}
              placeholder="Nhập mật khẩu"
              className="ml-3 w-full bg-transparent text-sm text-slate-800 placeholder:text-slate-400 outline-none disabled:opacity-60"
            />
            <button
              type="button"
              tabIndex={-1}
              onClick={() => setShowPassword(!showPassword)}
              className="text-slate-400 hover:text-slate-600 transition ml-1 cursor-pointer"
            >
              {showPassword ? <EyeOff className="size-4.5" /> : <Eye className="size-4.5" />}
            </button>
          </div>
          {errors.password && (
            <p className="mt-1.5 text-xs text-rose-500 font-medium">{errors.password}</p>
          )}
        </div>

        {/* Remember me & Forgot Password */}
        <div className="flex items-center justify-between pt-0.5">
          <label className="flex items-center gap-2 cursor-pointer select-none">
            <input
              type="checkbox"
              checked={rememberMe}
              onChange={(e) => setRememberMe(e.target.checked)}
              className="size-4 rounded border-slate-300 text-[#0052cc] accent-[#0052cc] focus:ring-[#0052cc]"
            />
            <span className="text-xs font-medium text-slate-600">Ghi nhớ đăng nhập</span>
          </label>
          <a
            href="#forgot"
            onClick={(e) => e.preventDefault()}
            className="text-xs font-semibold text-[#0052cc] hover:underline"
          >
            Quên mật khẩu?
          </a>
        </div>

        {/* Submit Button (without arrow icon) */}
        <button
          type="submit"
          disabled={isLoading || isGoogleLoading}
          className={cn(
            "w-full flex items-center justify-center rounded-xl py-3 px-4 text-sm font-semibold text-white shadow-sm transition-all active:scale-[0.99] mt-3",
            isLoading
              ? "bg-[#0052cc]/70 cursor-not-allowed"
              : "bg-[#0052cc] hover:bg-[#0043a8] cursor-pointer",
          )}
        >
          {isLoading ? (
            <Loader2 className="size-4.5 animate-spin" />
          ) : (
            <span>Đăng nhập</span>
          )}
        </button>
      </form>

      {/* Divider */}
      <div className="relative flex items-center justify-center">
        <div className="w-full border-t border-slate-200/80" />
        <span className="absolute bg-white px-3 text-xs font-normal text-slate-400">
          hoặc
        </span>
      </div>

      {/* Secondary: Google Login Button */}
      <button
        type="button"
        disabled={isLoading || isGoogleLoading}
        onClick={handleGoogleLogin}
        className={cn(
          "w-full flex items-center justify-center gap-3 rounded-xl py-3 px-4 border border-slate-200 bg-white text-sm font-semibold text-slate-700 shadow-2xs transition-all hover:bg-slate-50 hover:border-slate-300 active:scale-[0.99] cursor-pointer disabled:opacity-60 disabled:cursor-not-allowed",
        )}
      >
        {isGoogleLoading ? (
          <Loader2 className="size-4.5 animate-spin text-[#0052cc]" />
        ) : (
          <svg className="size-4.5 shrink-0" viewBox="0 0 24 24">
            <path
              fill="#4285F4"
              d="M23.745 12.27c0-.7-.06-1.4-.19-2.07H12v4.51h6.6c-.29 1.52-1.14 2.82-2.4 3.68v3.05h3.88c2.27-2.09 3.66-5.17 3.66-9.17z"
            />
            <path
              fill="#34A853"
              d="M12 24c3.24 0 5.95-1.08 7.93-2.91l-3.88-3.05c-1.08.72-2.45 1.16-4.05 1.16-3.12 0-5.77-2.1-6.72-4.93H1.25v3.15C3.26 21.36 7.33 24 12 24z"
            />
            <path
              fill="#FBBC05"
              d="M5.28 14.27c-.25-.72-.38-1.49-.38-2.27s.13-1.55.38-2.27V6.58H1.25C.45 8.18 0 9.98 0 12s.45 3.82 1.25 5.42l4.03-3.15z"
            />
            <path
              fill="#EA4335"
              d="M12 4.75c1.77 0 3.35.61 4.6 1.8l3.42-3.42C17.95 1.19 15.24 0 12 0 7.33 0 3.26 2.64 1.25 6.58l4.03 3.15c.95-2.83 3.6-4.98 6.72-4.98z"
            />
          </svg>
        )}
        <span>{isGoogleLoading ? "Đang kết nối..." : "Đăng nhập bằng Google"}</span>
      </button>

      {/* Switch to Register */}
      <div className="text-center text-xs text-slate-500">
        Chưa có tài khoản?{" "}
        <button
          type="button"
          onClick={onSwitchToRegister}
          className="font-bold text-[#0052cc] hover:underline cursor-pointer"
        >
          Tạo tài khoản mới
        </button>
      </div>
    </div>
  );
}
