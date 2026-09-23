import { useState } from 'react'
import { Link } from 'react-router-dom'
import {
  Alert, Badge, CompanyStatusBadge, JobStatusBadge, Loading, SectionCard, StatTile,
} from '../../components/ui/feedback'
import { EnumSelect, Field } from '../../components/ui/form'
import { useToast } from '../../components/ui/ToastProvider'
import { adminService } from '../../features/admin/admin.service'
import { dashboardService } from '../../features/dashboard/dashboard.service'
import { errorMessage } from '../../lib/api'
import { formatDate } from '../../lib/format'
import { useAsync } from '../../lib/useAsync'
import {
  CompanyRegistrationStatus, CompanyStatus, JobPostingStatus, companyRegistrationStatusLabels,
  companyStatusLabels, jobPostingStatusLabels,
} from '../../types/api'

const tabs = [
  { key: 'overview', label: 'Tổng quan' },
  { key: 'registrations', label: 'Đăng ký doanh nghiệp' },
  { key: 'companies', label: 'Doanh nghiệp' },
  { key: 'jobs', label: 'Tin tuyển dụng' },
  { key: 'users', label: 'Người dùng' },
] as const

export function AdminPage() {
  const [tab, setTab] = useState<(typeof tabs)[number]['key']>('overview')

  return (
    <section className="page section">
      <p className="eyebrow">QUẢN TRỊ HỆ THỐNG</p>
      <h1>Bảng điều khiển quản trị</h1>

      <nav className="sub-nav">
        {tabs.map(item => (
          <button
            key={item.key}
            type="button"
            className={tab === item.key ? 'active' : ''}
            onClick={() => setTab(item.key)}
          >
            {item.label}
          </button>
        ))}
      </nav>

      {tab === 'overview' ? <OverviewTab /> : null}
      {tab === 'registrations' ? <RegistrationsTab /> : null}
      {tab === 'companies' ? <CompaniesTab /> : null}
      {tab === 'jobs' ? <JobsTab /> : null}
      {tab === 'users' ? <UsersTab /> : null}
    </section>
  )
}

function OverviewTab() {
  const dashboard = useAsync(() => dashboardService.platform(), [])
  if (dashboard.loading) return <Loading />
  return (
    <>
      <Alert tone="danger">{dashboard.error}</Alert>
      {dashboard.data ? (
        <div className="stat-row">
          <StatTile label="Người dùng" value={dashboard.data.users} />
          <StatTile label="Ứng viên" value={dashboard.data.candidates} />
          <StatTile label="Doanh nghiệp" value={dashboard.data.companies} hint={`${dashboard.data.activeCompanies} đang hoạt động`} />
          <StatTile label="Tin tuyển dụng" value={dashboard.data.jobPostings} hint={`${dashboard.data.publishedJobPostings} đang đăng`} />
          <StatTile label="Hồ sơ ứng tuyển" value={dashboard.data.applications} />
        </div>
      ) : null}
    </>
  )
}

function RegistrationsTab() {
  const [status, setStatus] = useState<CompanyRegistrationStatus | ''>(CompanyRegistrationStatus.Pending)
  const registrations = useAsync(() => adminService.registrations(status), [status])
  const { notify } = useToast()
  const [error, setError] = useState('')
  const [notes, setNotes] = useState<Record<string, string>>({})
  const [busy, setBusy] = useState(false)

  const review = async (id: string, approve: boolean) => {
    setBusy(true)
    setError('')
    try {
      const note = notes[id]?.trim() || null
      if (approve) await adminService.approveRegistration(id, note)
      else await adminService.rejectRegistration(id, note)
      notify(approve ? 'Đã duyệt đăng ký và tạo tài khoản chủ sở hữu.' : 'Đã từ chối đăng ký.')
      registrations.reload()
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  return (
    <SectionCard title="Đăng ký doanh nghiệp" description="Duyệt hồ sơ sẽ tạo doanh nghiệp và tài khoản chủ sở hữu tương ứng.">
      <Field label="Lọc theo trạng thái">
        <EnumSelect
          labels={companyRegistrationStatusLabels}
          value={status}
          emptyLabel="Tất cả"
          onChange={value => setStatus(value as CompanyRegistrationStatus | '')}
        />
      </Field>

      {registrations.loading ? <Loading /> : null}
      <Alert tone="danger">{registrations.error ?? error}</Alert>

      {registrations.data?.length === 0 ? <p className="muted">Không có hồ sơ nào.</p> : null}

      {registrations.data?.map(item => (
        <div className="repeat-row" key={item.id}>
          <div className="row-between">
            <div>
              <b>{item.companyName}</b>
              <small className="muted"> · gửi ngày {formatDate(item.createdAtUtc)}</small>
              <p className="muted">
                Liên hệ: {item.contactFullName} · {item.contactEmail}
                {item.contactPhoneNumber ? ` · ${item.contactPhoneNumber}` : ''}
              </p>
              {item.description ? <p className="prewrap">{item.description}</p> : null}
              {item.reviewNote ? <p className="muted">Ghi chú duyệt: {item.reviewNote}</p> : null}
              {item.createdCompanyId ? (
                <p className="muted">
                  Doanh nghiệp đã tạo: <Link className="text-link" to={`/companies/${item.createdCompanyId}`}>xem hồ sơ →</Link>
                </p>
              ) : null}
            </div>
            <Badge tone={item.status === CompanyRegistrationStatus.Pending ? 'warning' : item.status === CompanyRegistrationStatus.Approved ? 'success' : 'danger'}>
              {companyRegistrationStatusLabels[item.status]}
            </Badge>
          </div>
          {item.status === CompanyRegistrationStatus.Pending ? (
            <div className="inline-form">
              <input
                value={notes[item.id] ?? ''}
                onChange={event => setNotes({ ...notes, [item.id]: event.target.value })}
                placeholder="Ghi chú cho quyết định (không bắt buộc)"
              />
              <button type="button" className="button button-small" disabled={busy} onClick={() => review(item.id, true)}>Duyệt</button>
              <button type="button" className="link-danger" disabled={busy} onClick={() => review(item.id, false)}>Từ chối</button>
            </div>
          ) : null}
        </div>
      ))}
    </SectionCard>
  )
}

function CompaniesTab() {
  const [status, setStatus] = useState<CompanyStatus | ''>(CompanyStatus.PendingVerification)
  const companies = useAsync(() => adminService.companies(status), [status])
  const { notify } = useToast()
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  const approve = async (id: string) => {
    setBusy(true)
    setError('')
    try {
      await adminService.approveCompany(id)
      notify('Đã kích hoạt doanh nghiệp.')
      companies.reload()
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  return (
    <SectionCard title="Doanh nghiệp" description="Doanh nghiệp phải ở trạng thái đang hoạt động mới được đăng tin và tạo tài khoản nhân sự.">
      <Field label="Lọc theo trạng thái">
        <EnumSelect labels={companyStatusLabels} value={status} emptyLabel="Tất cả" onChange={value => setStatus(value as CompanyStatus | '')} />
      </Field>

      {companies.loading ? <Loading /> : null}
      <Alert tone="danger">{companies.error ?? error}</Alert>

      <div className="table-wrap">
        <table className="data-table">
          <thead>
            <tr>
              <th>Doanh nghiệp</th>
              <th>Ngành nghề</th>
              <th>Trạng thái</th>
              <th />
            </tr>
          </thead>
          <tbody>
            {companies.data?.map(company => (
              <tr key={company.id}>
                <td><Link className="text-link" to={`/companies/${company.id}`}>{company.name}</Link></td>
                <td>{company.industry ?? '—'}</td>
                <td><CompanyStatusBadge status={company.status} /></td>
                <td className="row-actions">
                  {company.status !== CompanyStatus.Active ? (
                    <button type="button" className="text-link" disabled={busy} onClick={() => approve(company.id)}>Kích hoạt</button>
                  ) : null}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      {companies.data?.length === 0 ? <p className="muted">Không có doanh nghiệp nào.</p> : null}
    </SectionCard>
  )
}

function JobsTab() {
  const [status, setStatus] = useState<JobPostingStatus | ''>(JobPostingStatus.PendingApproval)
  const jobs = useAsync(() => adminService.jobPostings(status), [status])
  const { notify } = useToast()
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  const approve = async (id: string) => {
    setBusy(true)
    setError('')
    try {
      await adminService.approveJobPosting(id)
      notify('Đã duyệt và đăng tin tuyển dụng.')
      jobs.reload()
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  return (
    <SectionCard title="Tin tuyển dụng" description="Tin ở trạng thái chờ duyệt sẽ được đăng công khai ngay sau khi duyệt.">
      <Field label="Lọc theo trạng thái">
        <EnumSelect labels={jobPostingStatusLabels} value={status} emptyLabel="Tất cả" onChange={value => setStatus(value as JobPostingStatus | '')} />
      </Field>

      {jobs.loading ? <Loading /> : null}
      <Alert tone="danger">{jobs.error ?? error}</Alert>

      <div className="table-wrap">
        <table className="data-table">
          <thead>
            <tr>
              <th>Vị trí</th>
              <th>Doanh nghiệp</th>
              <th>Địa điểm</th>
              <th>Trạng thái</th>
              <th />
            </tr>
          </thead>
          <tbody>
            {jobs.data?.map(job => (
              <tr key={job.id}>
                <td>{job.title}</td>
                <td>{job.companyName}</td>
                <td>{job.location}</td>
                <td><JobStatusBadge status={job.status} /></td>
                <td className="row-actions">
                  {job.status === JobPostingStatus.PendingApproval ? (
                    <button type="button" className="text-link" disabled={busy} onClick={() => approve(job.id)}>Duyệt tin</button>
                  ) : null}
                  {job.status === JobPostingStatus.Published ? (
                    <Link className="text-link" to={`/jobs/${job.id}`}>Xem tin</Link>
                  ) : null}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      {jobs.data?.length === 0 ? <p className="muted">Không có tin tuyển dụng nào.</p> : null}
    </SectionCard>
  )
}

function UsersTab() {
  const users = useAsync(() => adminService.users(), [])
  const { notify } = useToast()
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  const toggle = async (id: string, isActive: boolean) => {
    setBusy(true)
    setError('')
    try {
      await adminService.setUserActive(id, isActive)
      notify(isActive ? 'Đã mở khoá tài khoản.' : 'Đã khoá tài khoản.')
      users.reload()
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  return (
    <SectionCard title="Người dùng" description="Mã người dùng dùng để thêm thành viên vào doanh nghiệp.">
      {users.loading ? <Loading /> : null}
      <Alert tone="danger">{users.error ?? error}</Alert>

      <div className="table-wrap">
        <table className="data-table">
          <thead>
            <tr>
              <th>Họ tên</th>
              <th>Email</th>
              <th>Mã người dùng</th>
              <th>Quyền</th>
              <th>Trạng thái</th>
              <th />
            </tr>
          </thead>
          <tbody>
            {users.data?.map(user => (
              <tr key={user.id}>
                <td>{user.fullName}</td>
                <td>{user.email}</td>
                <td><code className="mono">{user.id}</code></td>
                <td>{user.platformRole === 1 ? <Badge tone="info">Quản trị viên</Badge> : 'Người dùng'}</td>
                <td>{user.isActive ? <Badge tone="success">Hoạt động</Badge> : <Badge tone="danger">Đã khoá</Badge>}</td>
                <td className="row-actions">
                  <button type="button" className={user.isActive ? 'link-danger' : 'text-link'} disabled={busy} onClick={() => toggle(user.id, !user.isActive)}>
                    {user.isActive ? 'Khoá' : 'Mở khoá'}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </SectionCard>
  )
}
