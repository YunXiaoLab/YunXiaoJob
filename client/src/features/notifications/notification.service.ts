import { api } from '../../lib/api'
import type { Notification } from '../../types/api'

export const notificationService = {
  mine: () => api<Notification[]>('/api/notifications'),
  markRead: (id: string) => api<Notification>(`/api/notifications/${id}/read`, { method: 'PUT' }),
}
