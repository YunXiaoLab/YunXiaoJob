import { BrowserRouter, Link, Route, Routes } from 'react-router-dom'
import { ProtectedRoute } from './components/auth/ProtectedRoute'
import { AppShell } from './components/layout/AppShell'
import { EmptyState } from './components/ui/feedback'
import { ToastProvider } from './components/ui/ToastProvider'
import { AuthProvider } from './features/auth/AuthProvider'
import { CompanyProvider } from './features/companies/CompanyProvider'
import { AdminPage } from './pages/admin/AdminPage'
import { AuthPage } from './pages/AuthPage'
import { CandidateApplicationsPage } from './pages/candidate/CandidateApplicationsPage'
import { CandidateOverviewPage } from './pages/candidate/CandidateOverviewPage'
import { CandidateProfilePage } from './pages/candidate/CandidateProfilePage'
import { CompanyPage } from './pages/CompanyPage'
import { EmployerApplicationsPage } from './pages/employer/EmployerApplicationsPage'
import { EmployerCompanyPage } from './pages/employer/EmployerCompanyPage'
import { EmployerDashboardPage } from './pages/employer/EmployerDashboardPage'
import { EmployerJobsPage } from './pages/employer/EmployerJobsPage'
import { EmployerLayout } from './pages/employer/EmployerLayout'
import { EmployerLandingPage } from './pages/EmployerLandingPage'
import { HomePage } from './pages/HomePage'
import { JobDetailPage } from './pages/JobDetailPage'
import { JobsPage } from './pages/JobsPage'
import { NotificationsPage } from './pages/NotificationsPage'
import { ForgotPasswordPage, ResetPasswordPage } from './pages/PasswordPages'

function NotFoundPage() {
  return (
    <section className="page section">
      <EmptyState title="Không tìm thấy trang" description="Liên kết có thể đã thay đổi hoặc không còn tồn tại.">
        <Link className="button" to="/">Về trang chủ →</Link>
      </EmptyState>
    </section>
  )
}

export default function App() {
  return (
    <AuthProvider>
      <CompanyProvider>
        <ToastProvider>
          <BrowserRouter>
            <Routes>
              <Route element={<AppShell />}>
                <Route path="/" element={<HomePage />} />
                <Route path="/jobs" element={<JobsPage />} />
                <Route path="/jobs/:id" element={<JobDetailPage />} />
                <Route path="/companies/:id" element={<CompanyPage />} />
                <Route path="/for-employers" element={<EmployerLandingPage />} />

                <Route element={<ProtectedRoute />}>
                  <Route path="/notifications" element={<NotificationsPage />} />
                  <Route path="/candidate" element={<CandidateOverviewPage />} />
                  <Route path="/candidate/profile" element={<CandidateProfilePage />} />
                  <Route path="/candidate/applications" element={<CandidateApplicationsPage />} />
                  <Route path="/employer" element={<EmployerLayout />}>
                    <Route index element={<EmployerDashboardPage />} />
                    <Route path="jobs" element={<EmployerJobsPage />} />
                    <Route path="jobs/:jobId/applications" element={<EmployerApplicationsPage />} />
                    <Route path="company" element={<EmployerCompanyPage />} />
                  </Route>
                </Route>

                <Route element={<ProtectedRoute adminOnly />}>
                  <Route path="/admin" element={<AdminPage />} />
                </Route>

                <Route path="*" element={<NotFoundPage />} />
              </Route>

              <Route path="/login" element={<AuthPage />} />
              <Route path="/register" element={<AuthPage />} />
              <Route path="/forgot-password" element={<ForgotPasswordPage />} />
              <Route path="/reset-password" element={<ResetPasswordPage />} />
            </Routes>
          </BrowserRouter>
        </ToastProvider>
      </CompanyProvider>
    </AuthProvider>
  )
}
