import { useState } from "react";
import ChatPage from "@/features/messages/pages/ChatPage";
import { LoginPage } from "@/features/auth/pages/LoginPage";
import { RegisterPage } from "@/features/auth/pages/RegisterPage";

type AuthScreen = "login" | "register" | "app";

function App() {
  const [currentScreen, setCurrentScreen] = useState<AuthScreen>("login");

  if (currentScreen === "login") {
    return (
      <LoginPage
        onSuccess={() => setCurrentScreen("app")}
        onNavigateToRegister={() => setCurrentScreen("register")}
      />
    );
  }

  if (currentScreen === "register") {
    return (
      <RegisterPage
        onSuccess={() => setCurrentScreen("app")}
        onNavigateToLogin={() => setCurrentScreen("login")}
      />
    );
  }

  return <ChatPage onLogout={() => setCurrentScreen("login")} />;
}

export default App;
