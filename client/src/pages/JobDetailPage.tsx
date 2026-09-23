import { useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { Alert, EmptyState, JobStatusBadge, Loading } from '../components/ui/feedback'
import { Field } from '../components/ui/form'
import { Modal } from '../components/ui/Modal'
import { useToast } from '../components/ui/ToastProvider'
import { useAuth } from '../features/auth/AuthProvider'
import { applicationService } from '../features/applications/application.service'
import { candidateService } from '../features/candidate/candidate.service'
import { jobService } from '../features/jobs/job.service'
import { errorMessage } from '../lib/api'
import { formatDate, formatSalaryRange, initials } from '../lib/format'
import { useAsync } from '../lib/useAsync'
import { employmentTypeLabels, workplaceTypeLabels } from '../types/api'

export function JobDetailPage() {
  const { id = '' } = useParams()
  const { user } = useAuth()
  const navigate = useNavigate()
  const { notify } = useToast()
  const detail = useAsync(() => jobService.detail(id), [id])
  const profile = useAsync(() => candidateService.profile(), [user?.id], Boolean(user))
  const [applying, setApplying] = useState(false)
  const [resumeId, setResumeId] = useState('')
  const [coverLetter, setCoverLetter] = useState('')
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')

  if (detail.loading) return <section className="page section"><Loading /></section>
  if (detail.error || !detail.data) {
    return (
      <section className="page section">
        <EmptyState title="Không tìm thấy tin tuyển dụng" description={detail.error ?? 'Tin có thể đã bị gỡ hoặc chưa được duyệt.'}>
          <Link className="button" to="/jobs">Quay lại danh sách →</Link>
        </EmptyState>
      </section>
    )
  }

  const { job, company } = detail.data
  const resumes = profile.data?.resumes ?? []

  const openApply = () => {
    if (!user) { navigate('/login', { state: { from: `/jobs/${id}` } }); return }
    setResumeId(resumes.find(resume => resume.isDefault)?.id ?? resumes[0]?.id ?? '')
    setCoverLetter('')
    setError('')
    setApplying(true)
  }

  const submit = async () => {
    setBusy(true)
    setError('')
    try {
      await applicationService.apply(job.id, resumeId, coverLetter.trim() || null)
      setApplying(false)
      notify('Đã gửi hồ sơ ứng tuyển.')
      navigate('/candidate/applications')
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  return (
    <section className="page section detail-layout">
      <article>
        <p className="eyebrow">
          <Link to={`/companies/${company.id}`}>{company.name}</Link>
        </p>
        <h1>{job.title}</h1>
        <div className="job-meta">
          <span>⌖ {job.location}</span>
          <span>{workplaceTypeLabels[job.workplaceType]}</span>
          <span>{employmentTypeLabels[job.employmentType]}</span>
          <span>{formatSalaryRange(job.minSalary, job.maxSalary, job.currency)}</span>
          <JobStatusBadge status={job.status} />
        </div>
        <hr />
        <h2>Mô tả công việc</h2>
        <p className="prewrap">{job.description}</p>
        <h2>Yêu cầu ứng viên</h2>
        <p className="prewrap">{job.requirements}</p>
        {job.benefits ? (<><h2>Quyền lợi</h2><p className="prewrap">{job.benefits}</p></>) : null}
        {job.skills.length > 0 ? (
          <>
            <h2>Kỹ năng</h2>
            <div className="tags">{job.skills.map(skill => <span key={skill}>{skill}</span>)}</div>
          </>
        ) : null}
        <h2>Về {company.name}</h2>
        <p className="prewrap">{company.description ?? 'Doanh nghiệp chưa cập nhật giới thiệu.'}</p>
      </article>

      <aside className="apply-card">
        <div className="company-brief">
          {company.logoUrl
            ? <img className="company-mark" src={company.logoUrl} alt={company.name} />
            : <div className="company-mark">{initials(company.name)}</div>}
          <div>
            <b>{company.name}</b>
            <small>{company.industry ?? 'Chưa cập nhật ngành nghề'}</small>
          </div>
        </div>
        <p>Hạn nộp hồ sơ: <b>{formatDate(job.applicationDeadline)}</b></p>
        <button type="button" className="button" onClick={openApply}>
          {user ? 'Ứng tuyển ngay →' : 'Đăng nhập để ứng tuyển →'}
        </button>
        <Link className="outline-button" to={`/companies/${company.id}`}>Xem doanh nghiệp</Link>
      </aside>

      {applying ? (
        <Modal title={`Ứng tuyển: ${job.title}`} onClose={() => setApplying(false)}>
          {profile.loading ? <Loading /> : null}
          {profile.error ? <Alert tone="warning">Không đọc được hồ sơ ứng viên: {profile.error}</Alert> : null}
          {!profile.loading && resumes.length === 0 ? (
            <EmptyState title="Bạn chưa có CV nào" description="Thêm ít nhất một CV trong hồ sơ trước khi ứng tuyển.">
              <Link className="button" to="/candidate/profile">Cập nhật hồ sơ →</Link>
            </EmptyState>
          ) : null}
          {resumes.length > 0 ? (
            <>
              <Field label="Chọn CV">
                <select value={resumeId} onChange={event => setResumeId(event.target.value)}>
                  {resumes.map(resume => (
                    <option key={resume.id} value={resume.id}>{resume.name}{resume.isDefault ? ' (mặc định)' : ''}</option>
                  ))}
                </select>
              </Field>
              <Field label="Thư giới thiệu" hint="Không bắt buộc, tối đa vài đoạn ngắn.">
                <textarea rows={6} value={coverLetter} onChange={event => setCoverLetter(event.target.value)} />
              </Field>
              <Alert tone="danger">{error}</Alert>
              <div className="form-actions">
                <button type="button" className="outline-button" onClick={() => setApplying(false)}>Huỷ</button>
                <button type="button" className="button" disabled={busy || !resumeId} onClick={submit}>
                  {busy ? 'Đang gửi…' : 'Gửi hồ sơ →'}
                </button>
              </div>
            </>
          ) : null}
        </Modal>
      ) : null}
    </section>
  )
}
