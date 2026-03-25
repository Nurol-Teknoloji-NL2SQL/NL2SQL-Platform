import { createContext, useContext, useState, useCallback, useRef } from "react";

const ToastContext = createContext(null);

let toastIdCounter = 0;

export function ToastProvider({ children }) {
  const [toasts, setToasts] = useState([]);
  const timersRef = useRef({});

  const removeToast = useCallback((id) => {
    clearTimeout(timersRef.current[id]);
    delete timersRef.current[id];
    setToasts((prev) => prev.filter((t) => t.id !== id));
  }, []);

  const addToast = useCallback(
    (type, message) => {
      const id = ++toastIdCounter;

      setToasts((prev) => {
        const next = [...prev, { id, type, message, createdAt: Date.now() }];
        // Max 3 toasts — drop oldest
        if (next.length > 3) {
          const removed = next.shift();
          clearTimeout(timersRef.current[removed.id]);
          delete timersRef.current[removed.id];
        }
        return next;
      });

      // Auto-dismiss after 4s
      timersRef.current[id] = setTimeout(() => removeToast(id), 4000);

      return id;
    },
    [removeToast]
  );

  const value = {
    toasts,
    removeToast,
    success: (msg) => addToast("success", msg),
    error: (msg) => addToast("error", msg),
    warning: (msg) => addToast("warning", msg),
    info: (msg) => addToast("info", msg),
  };

  return (
    <ToastContext.Provider value={value}>{children}</ToastContext.Provider>
  );
}

export function useToast() {
  const ctx = useContext(ToastContext);
  if (!ctx) throw new Error("useToast must be used within ToastProvider");
  return ctx;
}
