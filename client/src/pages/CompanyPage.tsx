import { useParams } from 'react-router-dom'
import { JobCard } from '../components/jobs/JobCard'
import { Alert, CompanyStatusBadge, EmptyState, Loading } from '../components/ui/feedback'
import { companyService } from '../features/companies/company.service'
import { jobService } from '../features/jobs/job.service'
import { initials } from '../lib/format'
import { useAsync } from '../lib/useAsync'

export function CompanyPage() {
  const { id = '' } = useParams()
  const company = useAsync(() => companyService.get(id), [id])
  const jobs = useAsync(() => jobService.search(), [])
  const companyJobs = jobs.data?.filter(job => job.companyId === id) ?? []

  if (company.loading) return <section className="page section"><Loading /></section>
  if (company.error || !company.data) {
    return (
      <section className="page section">
        <EmptyState title="Không tìm thấy doanh nghiệp" description={company.error} />
      </section>
    )
  }

  const value = company.data
  return (
    <section className="page section">
      <div className="company-header">
        {value.logoUrl
          ? <img className="company-logo" src={value.logoUrl} alt={value.name} />
          : <div className="company-logo">{initials(value.name)}</div>}
        <div>
          <p className="eyebrow">DOANH NGHIỆP</p>
          <h1>{value.name}</h1>
          <div className="job-meta">
            <CompanyStatusBadge status={value.status} />
            {value.industry ? <span>{value.industry}</span> : null}
            {value.address ? <span>⌖ {value.address}</span> : null}
            {value.employeeCount ? <span>{value.employeeCount} nhân sự</span> : null}
            {value.website ? <a className="text-link" href={value.website} target="_blank" rel="noreferrer">Website ↗</a> : null}
          </div>
        </div>
      </div>

      <p className="prewrap lead">{value.description ?? 'Doanh nghiệp chưa cập nhật giới thiệu.'}</p>

      <div className="section-heading">
        <h2>Vị trí đang tuyển ({companyJobs.length})</h2>
      </div>
      {jobs.loading ? <Loading /> : null}
      <Alert tone="danger">{jobs.error}</Alert>
      {!jobs.loading && companyJobs.length === 0
        ? <EmptyState title="Chưa có vị trí nào đang tuyển" />
        : <div className="job-list">{companyJobs.map(job => <JobCard key={job.id} job={job} />)}</div>}
    </section>
  )
}
