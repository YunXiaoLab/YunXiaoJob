import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from 'react'
import type { Tone } from './feedback'

type Toast = { id: number; message: string; tone: Tone }
type ToastContextValue = { notify: (message: string, tone?: Tone) => void }

const ToastContext = createContext<ToastContextValue | undefined>(undefined)

export function ToastProvider({ children }: { children: ReactNode }) {
  const [toasts, setToasts] = useState<Toast[]>([])

  const notify = useCallback((message: string, tone: Tone = 'success') => {
    const id = Date.now() + Math.random()
    setToasts(current => [...current, { id, message, tone }])
    setTimeout(() => setToasts(current => current.filter(toast => toast.id !== id)), 4000)
  }, [])

  const value = useMemo(() => ({ notify }), [notify])

  return (
    <ToastContext.Provider value={value}>
      {children}
      <div className="toast-stack">
        {toasts.map(toast => <div key={toast.id} className={`toast toast-${toast.tone}`}>{toast.message}</div>)}
      </div>
    </ToastContext.Provider>
  )
}

export function useToast() {
  const value = useContext(ToastContext)
  if (!value) throw new Error('useToast must be used inside ToastProvider')
  return value
}
