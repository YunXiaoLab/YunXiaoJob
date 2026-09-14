const baseUrl = (import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5018').replace(/\/$/, '')

export type ApiResponse<T> = { success: boolean; data: T; error: string | null }

export async function api<T>(path: string, init: RequestInit = {}, token?: string) {
  const response = await fetch(`${baseUrl}${path}`, {
    ...init,
    headers: { 'Content-Type': 'application/json', ...(token ? { Authorization: `Bearer ${token}` } : {}), ...init.headers },
  })
  const body = await response.json() as ApiResponse<T>
  if (!response.ok || !body.success) throw new Error(body.error || 'Không thể thực hiện yêu cầu.')
  return body.data
}
