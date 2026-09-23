import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAuth } from '../../features/auth/AuthProvider'
import { Loading } from '../ui/feedback'

export function ProtectedRoute({ adminOnly = false }: { adminOnly?: boolean }) {
  const { user, isAdmin, ready } = useAuth()
  const location = useLocation()

  // The session is restored from localStorage after the first paint; redirecting before that logs the user out.
  if (!ready) return <div className="page section"><Loading /></div>
  if (!user) return <Navigate to="/login" replace state={{ from: location.pathname }} />
  if (adminOnly && !isAdmin) return <Navigate to="/" replace />
  return <Outlet />
}
