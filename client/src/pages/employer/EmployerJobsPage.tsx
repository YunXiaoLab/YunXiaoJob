import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Alert, EmptyState, JobStatusBadge, Loading, SectionCard } from '../../components/ui/feedback'
import { EnumSelect, Field, FormGrid, asInput, numberOrNull, textOrNull } from '../../components/ui/form'
import { Modal } from '../../components/ui/Modal'
import { useToast } from '../../components/ui/ToastProvider'
import { useCompanies } from '../../features/companies/CompanyProvider'
import { jobService, type JobPostingInput } from '../../features/jobs/job.service'
import { errorMessage } from '../../lib/api'
import { formatDate, formatSalaryRange, splitList } from '../../lib/format'
import { useAsync } from '../../lib/useAsync'
import {
  EmploymentType, JobPostingStatus, WorkplaceType, employmentTypeLabels, workplaceTypeLabels, type JobPosting,
} from '../../types/api'

type JobDraft = {
  title: string; description: string; requirements: string; benefits: string; location: string
  employmentType: EmploymentType; workplaceType: WorkplaceType; minSalary: string; maxSalary: string
  currency: string; applicationDeadline: string; skills: string
}

const emptyDraft: JobDraft = {
  title: '', description: '', requirements: '', benefits: '', location: '',
  employmentType: EmploymentType.FullTime, workplaceType: WorkplaceType.OnSite,
  minSalary: '', maxSalary: '', currency: 'VND', applicationDeadline: '', skills: '',
}

const toDraft = (job: JobPosting): JobDraft => ({
  title: job.title, description: job.description, requirements: job.requirements, benefits: asInput(job.benefits),
  location: job.location, employmentType: job.employmentType, workplaceType: job.workplaceType,
  minSalary: asInput(job.minSalary), maxSalary: asInput(job.maxSalary), currency: job.currency,
  applicationDeadline: asInput(job.applicationDeadline), skills: job.skills.join(', '),
})

const toInput = (draft: JobDraft): JobPostingInput => ({
  title: draft.title.trim(),
  description: draft.description.trim(),
  requirements: draft.requirements.trim(),
  benefits: textOrNull(draft.benefits),
  location: draft.location.trim(),
  employmentType: draft.employmentType,
  workplaceType: draft.workplaceType,
  minSalary: numberOrNull(draft.minSalary),
  maxSalary: numberOrNull(draft.maxSalary),
  currency: draft.currency.trim() || 'VND',
  applicationDeadline: textOrNull(draft.applicationDeadline),
  skills: splitList(draft.skills),
})

export function EmployerJobsPage() {
  const { active } = useCompanies()
  const companyId = active?.company.id ?? ''
  const jobs = useAsync(() => jobService.byCompany(companyId), [companyId], Boolean(companyId))
  const { notify } = useToast()
  const [editing, setEditing] = useState<JobPosting | null>(null)
  const [creating, setCreating] = useState(false)
  const [error, setError] = useState('')

  const submitForApproval = async (job: JobPosting) => {
    setError('')
    try {
      await jobService.submit(job.id)
      notify('Đã gửi tin tuyển dụng để duyệt.')
      jobs.reload()
    } catch (reason) {
      setError(errorMessage(reason))
    }
  }

  return (
    <div className="stack">
      <SectionCard
        title="Tin tuyển dụng"
        description="Tin ở trạng thái nháp cần được gửi duyệt trước khi hiển thị công khai."
        actions={<button type="button" className="button button-small" onClick={() => setCreating(true)}>+ Tạo tin mới</button>}
      >
        {jobs.loading ? <Loading /> : null}
        <Alert tone="danger">{jobs.error ?? error}</Alert>

        {jobs.data?.length === 0 ? (
          <EmptyState title="Chưa có tin tuyển dụng nào" description="Tạo tin đầu tiên để bắt đầu nhận hồ sơ." />
        ) : (
          <div className="table-wrap">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Vị trí</th>
                  <th>Địa điểm</th>
                  <th>Mức lương</th>
                  <th>Hạn nộp</th>
                  <th>Trạng thái</th>
                  <th />
                </tr>
              </thead>
              <tbody>
                {jobs.data?.map(job => (
                  <tr key={job.id}>
                    <td>
                      <b>{job.title}</b>
                      <small className="muted"> · {workplaceTypeLabels[job.workplaceType]} · {employmentTypeLabels[job.employmentType]}</small>
                    </td>
                    <td>{job.location}</td>
                    <td>{formatSalaryRange(job.minSalary, job.maxSalary, job.currency)}</td>
                    <td>{formatDate(job.applicationDeadline)}</td>
                    <td><JobStatusBadge status={job.status} /></td>
                    <td className="row-actions">
                      <Link className="text-link" to={`/employer/jobs/${job.id}/applications`}>Hồ sơ</Link>
                      <button type="button" className="text-link" onClick={() => setEditing(job)}>Sửa</button>
                      {job.status === JobPostingStatus.Draft || job.status === JobPostingStatus.Rejected ? (
                        <button type="button" className="text-link" onClick={() => submitForApproval(job)}>Gửi duyệt</button>
                      ) : null}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </SectionCard>

      {creating ? (
        <JobFormModal
          title="Tạo tin tuyển dụng"
          initial={emptyDraft}
          onClose={() => setCreating(false)}
          onSubmit={async draft => {
            await jobService.create(companyId, toInput(draft))
            notify('Đã tạo tin tuyển dụng ở trạng thái nháp.')
            setCreating(false)
            jobs.reload()
          }}
        />
      ) : null}

      {editing ? (
        <JobFormModal
          title={`Chỉnh sửa: ${editing.title}`}
          initial={toDraft(editing)}
          onClose={() => setEditing(null)}
          onSubmit={async draft => {
            await jobService.update(editing.id, toInput(draft))
            notify('Đã cập nhật tin tuyển dụng.')
            setEditing(null)
            jobs.reload()
          }}
        />
      ) : null}
    </div>
  )
}

function JobFormModal({ title, initial, onClose, onSubmit }: {
  title: string
  initial: JobDraft
  onClose: () => void
  onSubmit: (draft: JobDraft) => Promise<void>
}) {
  const [draft, setDraft] = useState(initial)
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)
  const set = <K extends keyof JobDraft>(key: K, value: JobDraft[K]) => setDraft(current => ({ ...current, [key]: value }))

  const save = async () => {
    setBusy(true)
    setError('')
    try {
      await onSubmit(draft)
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  return (
    <Modal title={title} onClose={onClose} wide>
      <FormGrid>
        <Field label="Tiêu đề *" wide>
          <input value={draft.title} onChange={event => set('title', event.target.value)} />
        </Field>
        <Field label="Mô tả công việc *" wide>
          <textarea rows={5} value={draft.description} onChange={event => set('description', event.target.value)} />
        </Field>
        <Field label="Yêu cầu ứng viên *" wide>
          <textarea rows={4} value={draft.requirements} onChange={event => set('requirements', event.target.value)} />
        </Field>
        <Field label="Quyền lợi" wide>
          <textarea rows={3} value={draft.benefits} onChange={event => set('benefits', event.target.value)} />
        </Field>
        <Field label="Địa điểm *">
          <input value={draft.location} onChange={event => set('location', event.target.value)} />
        </Field>
        <Field label="Hạn nộp hồ sơ">
          <input type="date" value={draft.applicationDeadline} onChange={event => set('applicationDeadline', event.target.value)} />
        </Field>
        <Field label="Hình thức làm việc">
          <EnumSelect labels={employmentTypeLabels} value={draft.employmentType} onChange={value => set('employmentType', (value === '' ? EmploymentType.FullTime : value) as EmploymentType)} />
        </Field>
        <Field label="Nơi làm việc">
          <EnumSelect labels={workplaceTypeLabels} value={draft.workplaceType} onChange={value => set('workplaceType', (value === '' ? WorkplaceType.OnSite : value) as WorkplaceType)} />
        </Field>
        <Field label="Lương tối thiểu">
          <input type="number" min={0} value={draft.minSalary} onChange={event => set('minSalary', event.target.value)} />
        </Field>
        <Field label="Lương tối đa">
          <input type="number" min={0} value={draft.maxSalary} onChange={event => set('maxSalary', event.target.value)} />
        </Field>
        <Field label="Đơn vị tiền tệ">
          <input value={draft.currency} onChange={event => set('currency', event.target.value)} />
        </Field>
        <Field label="Kỹ năng" hint="Phân tách bằng dấu phẩy: React, TypeScript, CSS" wide>
          <input value={draft.skills} onChange={event => set('skills', event.target.value)} />
        </Field>
      </FormGrid>
      <Alert tone="danger">{error}</Alert>
      <div className="form-actions">
        <button type="button" className="outline-button" onClick={onClose}>Huỷ</button>
        <button
          type="button"
          className="button"
          disabled={busy || !draft.title.trim() || !draft.description.trim() || !draft.requirements.trim() || !draft.location.trim()}
          onClick={save}
        >
          {busy ? 'Đang lưu…' : 'Lưu tin tuyển dụng'}
        </button>
      </div>
    </Modal>
  )
}
