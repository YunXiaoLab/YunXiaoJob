import { useState, type FormEvent } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router-dom'
import { Alert } from '../components/ui/feedback'
import { useToast } from '../components/ui/ToastProvider'
import { authService } from '../features/auth/auth.service'
import { errorMessage } from '../lib/api'

export function ForgotPasswordPage() {
  const [email, setEmail] = useState('')
  const [sent, setSent] = useState(false)
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  const submit = async (event: FormEvent) => {
    event.preventDefault()
    setBusy(true)
    setError('')
    try {
      await authService.forgotPassword(email)
      setSent(true)
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  return (
    <section className="auth-page">
      <div>
        <p className="eyebrow"><Link to="/">YUNXIAOJOB</Link></p>
        <h1>Quên mật khẩu?</h1>
        <p>Nhập email đã đăng ký, chúng tôi sẽ gửi liên kết đặt lại mật khẩu cho bạn.</p>
      </div>
      <form className="auth-card" onSubmit={submit}>
        <h2>Đặt lại mật khẩu</h2>
        {sent ? (
          <Alert tone="success">
            Nếu email tồn tại trong hệ thống, liên kết đặt lại mật khẩu đã được gửi. Vui lòng kiểm tra hộp thư.
          </Alert>
        ) : (
          <>
            <input required type="email" value={email} onChange={event => setEmail(event.target.value)} placeholder="Email" />
            <Alert tone="danger">{error}</Alert>
            <button className="button" disabled={busy}>{busy ? 'Đang gửi…' : 'Gửi liên kết →'}</button>
          </>
        )}
        <p><Link to="/login">Quay lại đăng nhập</Link></p>
      </form>
    </section>
  )
}

export function ResetPasswordPage() {
  const [params] = useSearchParams()
  const navigate = useNavigate()
  const { notify } = useToast()
  const [token, setToken] = useState(params.get('token') ?? '')
  const [password, setPassword] = useState('')
  const [confirm, setConfirm] = useState('')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  const submit = async (event: FormEvent) => {
    event.preventDefault()
    if (password !== confirm) { setError('Mật khẩu xác nhận không khớp.'); return }
    setBusy(true)
    setError('')
    try {
      await authService.resetPassword(token, password)
      notify('Đặt lại mật khẩu thành công.')
      navigate('/login', { replace: true })
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  return (
    <section className="auth-page">
      <div>
        <p className="eyebrow"><Link to="/">YUNXIAOJOB</Link></p>
        <h1>Tạo mật khẩu mới.</h1>
        <p>Liên kết đặt lại mật khẩu chỉ có hiệu lực trong thời gian ngắn.</p>
      </div>
      <form className="auth-card" onSubmit={submit}>
        <h2>Mật khẩu mới</h2>
        {params.get('token') ? null : (
          <input required value={token} onChange={event => setToken(event.target.value)} placeholder="Mã đặt lại mật khẩu" />
        )}
        <input required minLength={8} type="password" value={password} onChange={event => setPassword(event.target.value)} placeholder="Mật khẩu mới" />
        <input required minLength={8} type="password" value={confirm} onChange={event => setConfirm(event.target.value)} placeholder="Nhập lại mật khẩu" />
        <Alert tone="danger">{error}</Alert>
        <button className="button" disabled={busy}>{busy ? 'Đang lưu…' : 'Đặt lại mật khẩu →'}</button>
        <p><Link to="/login">Quay lại đăng nhập</Link></p>
      </form>
    </section>
  )
}
