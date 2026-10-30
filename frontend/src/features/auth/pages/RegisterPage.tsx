import { AuthLayout } from "../components/AuthLayout";
import { RegisterForm } from "../components/RegisterForm";

interface RegisterPageProps {
  onSuccess?: () => void;
  onNavigateToLogin: () => void;
}

export function RegisterPage({ onSuccess, onNavigateToLogin }: RegisterPageProps) {
  return (
    <AuthLayout
      title="Đăng ký"
      subtitle="Tạo tài khoản để tham gia BlockChat"
    >
      <RegisterForm
        onSuccess={onSuccess}
        onSwitchToLogin={onNavigateToLogin}
      />
    </AuthLayout>
  );
}
