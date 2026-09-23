import { useEffect, useState, type FormEvent } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import { Alert } from '../components/ui/feedback'
import { useToast } from '../components/ui/ToastProvider'
import { useAuth } from '../features/auth/AuthProvider'
import { errorMessage } from '../lib/api'
import type { RegistrationChallenge } from '../types/api'

function secondsUntil(value: string) {
  const target = new Date(value.endsWith('Z') ? value : `${value}Z`).getTime()
  return Math.max(0, Math.ceil((target - Date.now()) / 1000))
}

export function AuthPage() {
  const location = useLocation()
  const navigate = useNavigate()
  const auth = useAuth()
  const { notify } = useToast()
  const isLogin = location.pathname === '/login'
  const redirectTo = (location.state as { from?: string } | null)?.from ?? '/candidate'

  const [fullName, setFullName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [otp, setOtp] = useState('')
  const [challenge, setChallenge] = useState<RegistrationChallenge | null>(null)
  const [cooldown, setCooldown] = useState(0)
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    if (!challenge) return
    setCooldown(secondsUntil(challenge.resendAvailableAtUtc))
    const timer = setInterval(() => setCooldown(secondsUntil(challenge.resendAvailableAtUtc)), 1000)
    return () => clearInterval(timer)
  }, [challenge])

  const submit = async (event: FormEvent) => {
    event.preventDefault()
    setBusy(true)
    setError('')
    try {
      if (isLogin) {
        await auth.login(email, password)
        navigate(redirectTo, { replace: true })
      } else if (challenge) {
        await auth.verifyRegistration(challenge.challengeId, otp)
        notify('Tài khoản đã được kích hoạt. Mời bạn đăng nhập.')
        navigate('/login', { replace: true })
      } else {
        setChallenge(await auth.register(fullName, email, password))
      }
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  const resend = async () => {
    if (!challenge) return
    setBusy(true)
    setError('')
    try {
      setChallenge(await auth.resendOtp(challenge.challengeId))
      notify('Đã gửi lại mã OTP.')
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  const title = isLogin ? 'Chào mừng trở lại.' : challenge ? 'Xác thực email của bạn.' : 'Bắt đầu hành trình mới.'
  const description = challenge
    ? `Mã gồm 6 chữ số đã được gửi tới ${email}, hiệu lực trong 5 phút.`
    : isLogin
      ? 'Đăng nhập để tiếp tục quản lý hồ sơ và đơn ứng tuyển của bạn.'
      : 'Tạo tài khoản ứng viên miễn phí chỉ trong vài phút.'

  return (
    <section className="auth-page">
      <div>
        <p className="eyebrow"><Link to="/">YUNXIAOJOB</Link></p>
        <h1>{title}</h1>
        <p>{description}</p>
      </div>

      <form className="auth-card" onSubmit={submit}>
        <h2>{isLogin ? 'Đăng nhập' : challenge ? 'Xác thực email' : 'Tạo tài khoản'}</h2>

        {!isLogin && !challenge ? (
          <input required value={fullName} onChange={event => setFullName(event.target.value)} placeholder="Họ và tên" />
        ) : null}

        {!challenge ? (
          <>
            <input required type="email" value={email} onChange={event => setEmail(event.target.value)} placeholder="Email" />
            <input required minLength={8} type="password" value={password} onChange={event => setPassword(event.target.value)} placeholder="Mật khẩu" />
          </>
        ) : (
          <input
            required
            inputMode="numeric"
            pattern="[0-9]{6}"
            maxLength={6}
            value={otp}
            onChange={event => setOtp(event.target.value)}
            placeholder="Mã OTP 6 chữ số"
          />
        )}

        <Alert tone="danger">{error}</Alert>

        <button className="button" disabled={busy}>
          {busy ? 'Đang xử lý…' : challenge ? 'Xác thực →' : isLogin ? 'Đăng nhập →' : 'Gửi mã OTP →'}
        </button>

        {challenge ? (
          <button type="button" className="outline-button" disabled={busy || cooldown > 0} onClick={resend}>
            {cooldown > 0 ? `Gửi lại mã sau ${cooldown}s` : 'Gửi lại mã OTP'}
          </button>
        ) : (
          <>
            {isLogin ? <Link className="text-link" to="/forgot-password">Quên mật khẩu?</Link> : null}
            <p>
              {isLogin ? 'Chưa có tài khoản?' : 'Đã có tài khoản?'}{' '}
              <Link to={isLogin ? '/register' : '/login'}>{isLogin ? 'Đăng ký' : 'Đăng nhập'}</Link>
            </p>
            <p>Bạn là doanh nghiệp? <Link to="/for-employers">Đăng ký tuyển dụng</Link></p>
          </>
        )}
      </form>
    </section>
  )
}
