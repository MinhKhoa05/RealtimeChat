import { AuthLayout } from "../components/AuthLayout";
import { LoginForm } from "../components/LoginForm";

interface LoginPageProps {
  onSuccess?: () => void;
  onNavigateToRegister: () => void;
}

export function LoginPage({ onSuccess, onNavigateToRegister }: LoginPageProps) {
  return (
    <AuthLayout
      title="Đăng nhập"
      subtitle="Chào mừng bạn quay trở lại BlockChat"
    >
      <LoginForm
        onSuccess={onSuccess}
        onSwitchToRegister={onNavigateToRegister}
      />
    </AuthLayout>
  );
}
