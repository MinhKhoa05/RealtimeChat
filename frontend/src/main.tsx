import React from "react";
import ReactDOM from "react-dom/client";
import { QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter } from "react-router-dom";
import { Toaster } from "sonner";
import { App } from "./App";
import { queryClient } from "@/shared/api/queryClient";

async function bootstrap() {
  ReactDOM.createRoot(document.getElementById("root")!).render(
    <React.StrictMode>
      <BrowserRouter>
        <QueryClientProvider client={queryClient}>
          <App />
          <Toaster
            richColors
            position="top-right"
            closeButton
            duration={3000}
            toastOptions={{ closeButton: true, duration: 3000 }}
          />
        </QueryClientProvider>
      </BrowserRouter>
    </React.StrictMode>,
  );
}

void bootstrap();
