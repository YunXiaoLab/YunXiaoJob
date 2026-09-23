import { useEffect, type ReactNode } from 'react'

export function Modal({ title, onClose, children, wide = false }: { title: string; onClose: () => void; children: ReactNode; wide?: boolean }) {
  useEffect(() => {
    const onKey = (event: KeyboardEvent) => { if (event.key === 'Escape') onClose() }
    window.addEventListener('keydown', onKey)
    document.body.style.overflow = 'hidden'
    return () => {
      window.removeEventListener('keydown', onKey)
      document.body.style.overflow = ''
    }
  }, [onClose])

  return (
    <div className="modal-backdrop" onClick={onClose}>
      <div className={wide ? 'modal modal-wide' : 'modal'} role="dialog" aria-modal onClick={event => event.stopPropagation()}>
        <header className="modal-head">
          <h2>{title}</h2>
          <button type="button" className="icon-button" aria-label="Đóng" onClick={onClose}>×</button>
        </header>
        <div className="modal-body">{children}</div>
      </div>
    </div>
  )
}
