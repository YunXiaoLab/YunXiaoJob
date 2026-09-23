const baseUrl = (import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5018').replace(/\/$/, '')

export type ApiEnvelope<T> = { success: boolean; data: T; error: string | null }

export class ApiError extends Error {
  status: number
  constructor(message: string, status: number) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

/** Lets AuthProvider own the tokens while the request layer stays a plain module. */
export type AuthBridge = {
  accessToken(): string | null
  refresh(): Promise<string | null>
  signOut(): void
}

let bridge: AuthBridge | null = null
export function connectAuth(value: AuthBridge | null) { bridge = value }

export type QueryValue = string | number | boolean | null | undefined
export function query(values: Record<string, QueryValue>) {
  const search = new URLSearchParams()
  for (const [key, value] of Object.entries(values))
    if (value !== undefined && value !== null && value !== '') search.set(key, String(value))
  const text = search.toString()
  return text ? `?${text}` : ''
}

type Options = { method?: string; body?: unknown; anonymous?: boolean }

async function send(path: string, options: Options, token: string | null): Promise<Response> {
  return fetch(`${baseUrl}${path}`, {
    method: options.method ?? 'GET',
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    body: options.body === undefined ? undefined : JSON.stringify(options.body),
  })
}

export async function api<T>(path: string, options: Options = {}): Promise<T> {
  const token = options.anonymous ? null : (bridge?.accessToken() ?? null)
  let response = await send(path, options, token)

  // A 15-minute access token expires while the tab is open: renew once, then replay the request.
  if (response.status === 401 && !options.anonymous && bridge) {
    const renewed = await bridge.refresh()
    if (renewed) response = await send(path, options, renewed)
    else bridge.signOut()
  }

  let envelope: ApiEnvelope<T> | null = null
  try { envelope = (await response.json()) as ApiEnvelope<T> } catch { envelope = null }

  if (!response.ok || !envelope?.success) {
    const message = envelope?.error || (response.status === 401
      ? 'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.'
      : 'Không thể thực hiện yêu cầu.')
    throw new ApiError(message, response.status)
  }
  return envelope.data
}

export function errorMessage(error: unknown) {
  return error instanceof Error ? error.message : 'Đã có lỗi xảy ra.'
}
