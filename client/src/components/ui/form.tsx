import type { ChangeEvent, ReactNode } from 'react'
import { optionsOf } from '../../lib/format'

export function Field({ label, hint, children, wide = false }: { label: string; hint?: string; children: ReactNode; wide?: boolean }) {
  return (
    <label className={wide ? 'field field-wide' : 'field'}>
      <span>{label}</span>
      {children}
      {hint ? <small>{hint}</small> : null}
    </label>
  )
}

export function FormGrid({ children, columns = 2 }: { children: ReactNode; columns?: number }) {
  return <div className="form-grid" style={{ gridTemplateColumns: `repeat(${columns}, minmax(0, 1fr))` }}>{children}</div>
}

export function FormActions({ children }: { children: ReactNode }) {
  return <div className="form-actions">{children}</div>
}

type EnumSelectProps = {
  labels: Record<number, string>
  value: number | ''
  onChange: (value: number | '') => void
  emptyLabel?: string
  disabled?: boolean
}

/** A <select> over a numeric API enum; `emptyLabel` adds an "all / unset" choice that maps to ''. */
export function EnumSelect({ labels, value, onChange, emptyLabel, disabled }: EnumSelectProps) {
  const handle = (event: ChangeEvent<HTMLSelectElement>) =>
    onChange(event.target.value === '' ? '' : Number(event.target.value))
  return (
    <select value={value === '' ? '' : String(value)} onChange={handle} disabled={disabled}>
      {emptyLabel ? <option value="">{emptyLabel}</option> : null}
      {optionsOf(labels).map(option => (
        <option key={option.value} value={option.value}>{option.label}</option>
      ))}
    </select>
  )
}

export function Checkbox({ label, checked, onChange }: { label: string; checked: boolean; onChange: (value: boolean) => void }) {
  return (
    <label className="checkbox">
      <input type="checkbox" checked={checked} onChange={event => onChange(event.target.checked)} />
      <span>{label}</span>
    </label>
  )
}

export const numberOrNull = (value: string) => (value.trim() === '' ? null : Number(value))
export const textOrNull = (value: string) => (value.trim() === '' ? null : value.trim())
export const asInput = (value: string | number | null | undefined) => (value === null || value === undefined ? '' : String(value))
