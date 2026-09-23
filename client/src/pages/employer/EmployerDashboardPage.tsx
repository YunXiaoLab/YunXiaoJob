import { Link } from 'react-router-dom'
import { Alert, EmptyState, JobStatusBadge, Loading, SectionCard, StatTile } from '../../components/ui/feedback'
import { useCompanies } from '../../features/companies/CompanyProvider'
import { dashboardService } from '../../features/dashboard/dashboard.service'
import { jobService } from '../../features/jobs/job.service'
import { formatDate } from '../../lib/format'
import { useAsync } from '../../lib/useAsync'
import { ApplicationStatus, CompanyMemberRole, applicationStatusLabels } from '../../types/api'

export function EmployerDashboardPage() {
  const { active } = useCompanies()
  const companyId = active?.company.id ?? ''
  // Recruiters are not allowed to read the company dashboard, so the request is not even attempted for them.
  const canReadDashboard = active?.role !== CompanyMemberRole.Recruiter
  const dashboard = useAsync(() => dashboardService.company(companyId), [companyId], Boolean(companyId) && canReadDashboard)
  const jobs = useAsync(() => jobService.byCompany(companyId), [companyId], Boolean(companyId))

  const byStatus = dashboard.data?.applicationsByStatus ?? {}

  return (
    <div className="stack">
      {dashboard.loading || jobs.loading ? <Loading /> : null}
      <Alert tone="danger">{dashboard.error ?? jobs.error}</Alert>

      {canReadDashboard && dashboard.data ? (
        <>
          <div className="stat-row">
            <StatTile label="Tin tuyển dụng" value={dashboard.data.jobPostings} />
            <StatTile label="Đang đăng" value={dashboard.data.publishedJobPostings} />
            <StatTile label="Hồ sơ nhận được" value={dashboard.data.applications} />
            <StatTile label="Đã tuyển" value={byStatus[String(ApplicationStatus.Hired)] ?? 0} />
          </div>

          <SectionCard title="Hồ sơ theo trạng thái">
            <div className="pipeline">
              {Object.entries(applicationStatusLabels).map(([value, label]) => (
                <div key={value} className="pipeline-step">
                  <b>{byStatus[value] ?? 0}</b>
                  <span>{label}</span>
                </div>
              ))}
            </div>
          </SectionCard>
        </>
      ) : null}

      <SectionCard
        title="Tin tuyển dụng gần đây"
        actions={<Link className="text-link" to="/employer/jobs">Quản lý tin →</Link>}
      >
        {jobs.data?.length === 0 ? (
          <EmptyState title="Chưa có tin tuyển dụng" description="Tạo tin đầu tiên và gửi duyệt để bắt đầu nhận hồ sơ.">
            <Link className="button" to="/employer/jobs">Tạo tin tuyển dụng →</Link>
          </EmptyState>
        ) : (
          <ul className="list">
            {jobs.data?.slice(0, 6).map(job => (
              <li key={job.id}>
                <div>
                  <b>{job.title}</b>
                  <small>{job.location} · hạn nộp {formatDate(job.applicationDeadline)}</small>
                </div>
                <div className="row-actions">
                  <JobStatusBadge status={job.status} />
                  <Link className="text-link" to={`/employer/jobs/${job.id}/applications`}>Hồ sơ →</Link>
                </div>
              </li>
            ))}
          </ul>
        )}
      </SectionCard>

      {!canReadDashboard ? (
        <Alert tone="info">
          Tài khoản Recruiter chỉ xem được các hồ sơ đã được phân công, nên phần thống kê doanh nghiệp bị ẩn.
        </Alert>
      ) : null}
    </div>
  )
}
