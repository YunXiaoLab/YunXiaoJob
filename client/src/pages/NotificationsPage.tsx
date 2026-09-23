import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Alert, Badge, EmptyState, Loading, SectionCard } from '../components/ui/feedback'
import { notificationService } from '../features/notifications/notification.service'
import { errorMessage } from '../lib/api'
import { formatDateTime } from '../lib/format'
import { useAsync } from '../lib/useAsync'
import { notificationTypeLabels } from '../types/api'

export function NotificationsPage() {
  const notifications = useAsync(() => notificationService.mine(), [])
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  const markRead = async (id: string) => {
    setBusy(true)
    setError('')
    try {
      await notificationService.markRead(id)
      notifications.reload()
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  const unread = notifications.data?.filter(item => !item.isRead).length ?? 0

  return (
    <section className="page section">
      <p className="eyebrow">THÔNG BÁO</p>
      <h1>Hộp thư của bạn</h1>

      {notifications.loading ? <Loading /> : null}
      <Alert tone="danger">{notifications.error ?? error}</Alert>

      {notifications.data?.length === 0 ? (
        <EmptyState title="Chưa có thông báo nào" description="Cập nhật về hồ sơ ứng tuyển sẽ xuất hiện ở đây.">
          <Link className="button" to="/jobs">Tìm việc làm →</Link>
        </EmptyState>
      ) : null}

      {notifications.data && notifications.data.length > 0 ? (
        <SectionCard title={`Tất cả thông báo (${unread} chưa đọc)`}>
          <ul className="list">
            {notifications.data.map(item => (
              <li key={item.id} className={item.isRead ? undefined : 'unread'}>
                <div>
                  <b>{item.title}</b>
                  <small>{notificationTypeLabels[item.type]} · {formatDateTime(item.createdAtUtc)}</small>
                  <p className="prewrap">{item.content}</p>
                  {item.targetUrl ? <Link className="text-link" to={item.targetUrl}>Xem chi tiết →</Link> : null}
                </div>
                <div className="row-actions">
                  {item.isRead
                    ? <Badge>Đã đọc</Badge>
                    : <button type="button" className="text-link" disabled={busy} onClick={() => markRead(item.id)}>Đánh dấu đã đọc</button>}
                </div>
              </li>
            ))}
          </ul>
        </SectionCard>
      ) : null}
    </section>
  )
}
