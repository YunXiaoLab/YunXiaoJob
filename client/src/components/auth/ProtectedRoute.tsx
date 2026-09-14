import { Navigate, Outlet } from 'react-router-dom'
import { useAuth } from '../../features/auth/AuthProvider'
export function ProtectedRoute() { return useAuth().user ? <Outlet/> : <Navigate to="/login" replace/> }
