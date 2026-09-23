import { useEffect, useRef, useState } from 'react'
import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '../../features/auth/AuthProvider'
import { notificationService } from '../../features/notifications/notification.service'
import { initials } from '../../lib/format'

function AccountMenu() {
  const { user, isAdmin, logout } = useAuth()
  const [open, setOpen] = useState(false)
  const [unread, setUnread] = useState(0)
  const container = useRef<HTMLDivElement>(null)
  const navigate = useNavigate()

  useEffect(() => {
    if (!user) { setUnread(0); return }
    let active = true
    notificationService.mine()
      .then(items => { if (active) setUnread(items.filter(item => !item.isRead).length) })
      .catch(() => { /* the badge is optional: a failure here must not break the shell */ })
    return () => { active = false }
  }, [user])

  useEffect(() => {
    const onClick = (event: MouseEvent) => {
      if (container.current && !container.current.contains(event.target as Node)) setOpen(false)
    }
    document.addEventListener('mousedown', onClick)
    return () => document.removeEventListener('mousedown', onClick)
  }, [])

  if (!user) {
    return (
      <div className="nav-actions">
        <Link className="text-link" to="/login">Đăng nhập</Link>
        <Link className="button button-small" to="/register">Đăng ký</Link>
      </div>
    )
  }

  return (
    <div className="nav-actions" ref={container}>
      <Link className="bell" to="/notifications" aria-label="Thông báo">
        <svg viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round">
          <path d="M18 8a6 6 0 1 0-12 0c0 6-2 7-2 7h16s-2-1-2-7" />
          <path d="M13.7 20a2 2 0 0 1-3.4 0" />
        </svg>
        {unread > 0 ? <i>{unread > 9 ? '9+' : unread}</i> : null}
      </Link>
      <button type="button" className="avatar-button" onClick={() => setOpen(value => !value)}>
        <span className="avatar">{initials(user.fullName)}</span>
        <b>{user.fullName}</b>
      </button>
      {open ? (
        <div className="menu" onClick={() => setOpen(false)}>
          <Link to="/candidate">Hồ sơ ứng viên</Link>
          <Link to="/candidate/applications">Đơn ứng tuyển</Link>
          <Link to="/employer">Khu vực tuyển dụng</Link>
          {isAdmin ? <Link to="/admin">Quản trị hệ thống</Link> : null}
          <button type="button" onClick={() => { logout(); navigate('/') }}>Đăng xuất</button>
        </div>
      ) : null}
    </div>
  )
}

export function AppShell() {
  const { user, isAdmin } = useAuth()
  return (
    <div className="app-shell">
      <header className="navbar">
        <Link className="brand" to="/"><span>Y</span> YunXiaoJob</Link>
        <nav>
          <NavLink to="/jobs">Tìm việc</NavLink>
          <NavLink to={user ? '/employer' : '/for-employers'}>Nhà tuyển dụng</NavLink>
          <NavLink to="/candidate">Việc của tôi</NavLink>
          {isAdmin ? <NavLink to="/admin">Quản trị</NavLink> : null}
        </nav>
        <AccountMenu />
      </header>
      <main><Outlet /></main>
      <footer className="footer">
        <div>
          <Link className="brand" to="/"><span>Y</span> YunXiaoJob</Link>
          <p>Kết nối công việc đúng người, đúng thời điểm.</p>
        </div>
        <div>
          <strong>Dành cho ứng viên</strong>
          <Link to="/jobs">Tìm việc làm</Link>
          <Link to="/candidate/profile">Quản lý hồ sơ</Link>
          <Link to="/candidate/applications">Đơn ứng tuyển</Link>
        </div>
        <div>
          <strong>Dành cho doanh nghiệp</strong>
          <Link to="/for-employers">Đăng ký tuyển dụng</Link>
          <Link to="/employer/jobs">Đăng tin tuyển dụng</Link>
          <a href="mailto:hello@yunxiaojob.vn">Liên hệ hỗ trợ</a>
        </div>
        <small>© 2026 YunXiaoJob. All rights reserved.</small>
      </footer>
    </div>
  )
}
