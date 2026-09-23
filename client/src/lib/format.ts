const dateFormat = new Intl.DateTimeFormat('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
const dateTimeFormat = new Intl.DateTimeFormat('vi-VN', {
  day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit',
})

export function formatDate(value: string | null | undefined) {
  if (!value) return '—'
  const date = new Date(value.length === 10 ? `${value}T00:00:00` : value)
  return Number.isNaN(date.getTime()) ? '—' : dateFormat.format(date)
}

export function formatDateTime(value: string | null | undefined) {
  if (!value) return '—'
  const date = new Date(value.endsWith('Z') || value.includes('+') ? value : `${value}Z`)
  return Number.isNaN(date.getTime()) ? '—' : dateTimeFormat.format(date)
}

export function formatRelative(value: string | null | undefined) {
  if (!value) return '—'
  const date = new Date(value.endsWith('Z') || value.includes('+') ? value : `${value}Z`)
  if (Number.isNaN(date.getTime())) return '—'
  const days = Math.floor((Date.now() - date.getTime()) / 86_400_000)
  if (days <= 0) return 'Hôm nay'
  if (days === 1) return 'Hôm qua'
  if (days < 30) return `${days} ngày trước`
  if (days < 365) return `${Math.floor(days / 30)} tháng trước`
  return `${Math.floor(days / 365)} năm trước`
}

export function formatMoney(value: number | null | undefined, currency = 'VND') {
  if (value === null || value === undefined) return '—'
  const normalized = currency === 'VND' && value >= 1_000_000 ? `${(value / 1_000_000).toFixed(1).replace(/\.0$/, '')} triệu` : value.toLocaleString('vi-VN')
  return `${normalized} ${currency === 'VND' ? '' : currency}`.trim()
}

export function formatSalaryRange(min: number | null, max: number | null, currency = 'VND') {
  if (min === null && max === null) return 'Thương lượng'
  if (min !== null && max !== null) return `${formatMoney(min, currency)} – ${formatMoney(max, currency)}`
  return min !== null ? `Từ ${formatMoney(min, currency)}` : `Đến ${formatMoney(max, currency)}`
}

/** `2026-09-15T08:30:00Z` -> `2026-09-15T15:30` in the browser zone, for <input type="datetime-local">. */
export function toLocalInputValue(value: string | null | undefined) {
  if (!value) return ''
  const date = new Date(value.endsWith('Z') || value.includes('+') ? value : `${value}Z`)
  if (Number.isNaN(date.getTime())) return ''
  const pad = (part: number) => String(part).padStart(2, '0')
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`
}

/** The reverse of {@link toLocalInputValue}: a local datetime-local value as a UTC instant. */
export function toUtcIsoString(value: string) {
  const date = new Date(value)
  return Number.isNaN(date.getTime()) ? '' : date.toISOString()
}

export function initials(value: string) {
  return value.trim().slice(0, 1).toUpperCase() || '?'
}

export function optionsOf(labels: Record<number, string>) {
  return Object.entries(labels).map(([value, label]) => ({ value: Number(value), label }))
}

export function splitList(value: string) {
  return value.split(',').map(item => item.trim()).filter(Boolean)
}
