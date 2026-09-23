import { useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import {
  Alert, ApplicationBadge, EmptyState, InterviewBadge, Loading, OfferBadge, SectionCard,
} from '../../components/ui/feedback'
import { EnumSelect, Field, FormGrid, numberOrNull, textOrNull } from '../../components/ui/form'
import { Modal } from '../../components/ui/Modal'
import { useToast } from '../../components/ui/ToastProvider'
import { applicationService } from '../../features/applications/application.service'
import { companyService } from '../../features/companies/company.service'
import { useCompanies } from '../../features/companies/CompanyProvider'
import { errorMessage } from '../../lib/api'
import { formatDate, formatDateTime, formatMoney, toUtcIsoString } from '../../lib/format'
import { useAsync } from '../../lib/useAsync'
import {
  ApplicationStatus, CompanyMemberRole, InterviewStatus, applicationStatusLabels, companyMemberRoleLabels,
} from '../../types/api'

export function EmployerApplicationsPage() {
  const { jobId = '' } = useParams()
  const { active } = useCompanies()
  const [status, setStatus] = useState<ApplicationStatus | ''>('')
  const applications = useAsync(() => applicationService.forJob(jobId, status), [jobId, status])
  const [openId, setOpenId] = useState<string | null>(null)

  const job = applications.data?.[0]?.job

  return (
    <div className="stack">
      <SectionCard
        title={job ? `Hồ sơ ứng tuyển · ${job.title}` : 'Hồ sơ ứng tuyển'}
        description={active ? `Doanh nghiệp: ${active.company.name}` : undefined}
        actions={<Link className="text-link" to="/employer/jobs">← Danh sách tin</Link>}
      >
        <Field label="Lọc theo trạng thái">
          <EnumSelect
            labels={applicationStatusLabels}
            value={status}
            emptyLabel="Tất cả trạng thái"
            onChange={value => setStatus(value as ApplicationStatus | '')}
          />
        </Field>

        {applications.loading ? <Loading /> : null}
        <Alert tone="danger">{applications.error}</Alert>

        {applications.data?.length === 0 ? (
          <EmptyState title="Chưa có hồ sơ nào" description="Hồ sơ ứng tuyển sẽ xuất hiện ở đây ngay khi ứng viên gửi." />
        ) : (
          <div className="table-wrap">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Ứng viên</th>
                  <th>Liên hệ</th>
                  <th>Ngày nộp</th>
                  <th>Trạng thái</th>
                  <th />
                </tr>
              </thead>
              <tbody>
                {applications.data?.map(item => (
                  <tr key={item.application.id}>
                    <td>
                      <b>{item.candidate?.fullName ?? 'Ứng viên'}</b>
                      <small className="muted"> · {item.candidate?.headline ?? 'Chưa có tiêu đề hồ sơ'}</small>
                    </td>
                    <td>{item.candidate?.email ?? '—'}</td>
                    <td>{formatDate(item.application.submittedAtUtc)}</td>
                    <td><ApplicationBadge status={item.application.status} /></td>
                    <td className="row-actions">
                      {item.candidate?.resume
                        ? <a className="text-link" href={item.candidate.resume.fileUrl} target="_blank" rel="noreferrer">CV ↗</a>
                        : null}
                      <button type="button" className="text-link" onClick={() => setOpenId(item.application.id)}>Xử lý</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </SectionCard>

      {openId ? (
        <ApplicationWorkspace
          applicationId={openId}
          companyId={active?.company.id ?? ''}
          canManage={active?.role !== CompanyMemberRole.Recruiter}
          onClose={() => setOpenId(null)}
          onChanged={() => applications.reload()}
        />
      ) : null}
    </div>
  )
}

function ApplicationWorkspace({ applicationId, companyId, canManage, onClose, onChanged }: {
  applicationId: string
  companyId: string
  canManage: boolean
  onClose: () => void
  onChanged: () => void
}) {
  const detail = useAsync(() => applicationService.detail(applicationId), [applicationId])
  const members = useAsync(() => companyService.members(companyId), [companyId], Boolean(companyId))
  const { notify } = useToast()
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  const [statusValue, setStatusValue] = useState<ApplicationStatus | ''>('')
  const [statusNote, setStatusNote] = useState('')
  const [recruiterId, setRecruiterId] = useState('')
  const [interview, setInterview] = useState({ startsAt: '', endsAt: '', place: '', note: '' })
  const [offer, setOffer] = useState({ salary: '', currency: 'VND', startDate: '', expiresOn: '', note: '' })

  const run = async (action: () => Promise<unknown>, message: string) => {
    setBusy(true)
    setError('')
    try {
      await action()
      notify(message)
      detail.reload()
      onChanged()
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  const candidate = detail.data?.candidate
  const recruiters = members.data?.filter(member => member.isActive) ?? []

  return (
    <Modal title="Xử lý hồ sơ ứng tuyển" onClose={onClose} wide>
      {detail.loading ? <Loading /> : null}
      <Alert tone="danger">{detail.error ?? error}</Alert>

      {detail.data ? (
        <div className="stack">
          <SectionCard title={candidate?.fullName ?? 'Ứng viên'} description={candidate?.headline ?? undefined}>
            <div className="job-meta">
              <ApplicationBadge status={detail.data.application.status} />
              <span>{candidate?.email}</span>
              {candidate?.phoneNumber ? <span>{candidate.phoneNumber}</span> : null}
              {candidate?.location ? <span>⌖ {candidate.location}</span> : null}
              {candidate?.yearsOfExperience ? <span>{candidate.yearsOfExperience} năm kinh nghiệm</span> : null}
            </div>
            <div className="job-meta">
              {candidate?.resume ? <a className="text-link" href={candidate.resume.fileUrl} target="_blank" rel="noreferrer">Mở CV ↗</a> : null}
              {candidate?.linkedInUrl ? <a className="text-link" href={candidate.linkedInUrl} target="_blank" rel="noreferrer">LinkedIn ↗</a> : null}
              {candidate?.gitHubUrl ? <a className="text-link" href={candidate.gitHubUrl} target="_blank" rel="noreferrer">GitHub ↗</a> : null}
              {candidate?.portfolioUrl ? <a className="text-link" href={candidate.portfolioUrl} target="_blank" rel="noreferrer">Portfolio ↗</a> : null}
            </div>
            {candidate && candidate.skills.length > 0
              ? <div className="tags">{candidate.skills.map(skill => <span key={skill.id}>{skill.name}</span>)}</div>
              : null}
            <h3>Thư giới thiệu</h3>
            <p className="prewrap">{detail.data.application.coverLetter ?? 'Ứng viên không gửi thư giới thiệu.'}</p>
          </SectionCard>

          <SectionCard title="Cập nhật trạng thái">
            <FormGrid>
              <Field label="Trạng thái mới">
                <EnumSelect
                  labels={applicationStatusLabels}
                  value={statusValue}
                  emptyLabel="Chọn trạng thái"
                  onChange={value => setStatusValue(value as ApplicationStatus | '')}
                />
              </Field>
              <Field label="Ghi chú">
                <input value={statusNote} onChange={event => setStatusNote(event.target.value)} />
              </Field>
            </FormGrid>
            <div className="form-actions">
              <button
                type="button"
                className="button"
                disabled={busy || statusValue === ''}
                onClick={() => run(
                  () => applicationService.changeStatus(applicationId, statusValue as ApplicationStatus, textOrNull(statusNote)),
                  'Đã cập nhật trạng thái hồ sơ.',
                )}
              >
                Cập nhật trạng thái
              </button>
            </div>
          </SectionCard>

          {canManage ? (
            <SectionCard title="Phân công phụ trách" description="Chỉ Owner và HR được phân công người phụ trách.">
              {members.loading ? <Loading /> : null}
              <FormGrid>
                <Field label="Thành viên phụ trách">
                  <select value={recruiterId} onChange={event => setRecruiterId(event.target.value)}>
                    <option value="">Chọn thành viên</option>
                    {recruiters.map(member => (
                      <option key={member.id} value={member.id}>
                        {member.fullName} · {companyMemberRoleLabels[member.role]}
                      </option>
                    ))}
                  </select>
                </Field>
              </FormGrid>
              <div className="form-actions">
                <button
                  type="button"
                  className="button"
                  disabled={busy || !recruiterId}
                  onClick={() => run(
                    () => applicationService.assignRecruiter(applicationId, recruiterId),
                    'Đã phân công người phụ trách.',
                  )}
                >
                  Phân công
                </button>
              </div>
            </SectionCard>
          ) : null}

          <SectionCard title="Lịch phỏng vấn">
            {detail.data.interviews.length === 0 ? <p className="muted">Chưa có lịch phỏng vấn.</p> : (
              <ul className="list">
                {detail.data.interviews.map(item => (
                  <li key={item.id}>
                    <div>
                      <b>{formatDateTime(item.startsAtUtc)} → {formatDateTime(item.endsAtUtc)}</b>
                      <small>{item.locationOrMeetingUrl ?? 'Chưa có địa điểm/liên kết'}</small>
                      {item.evaluationNote ? <small>Đánh giá: {item.evaluationNote} {item.rating ? `(${item.rating}/5)` : ''}</small> : null}
                    </div>
                    <div className="row-actions">
                      <InterviewBadge status={item.status} />
                      {item.status !== InterviewStatus.Completed ? (
                        <CompleteInterviewButton
                          busy={busy}
                          onComplete={(note, rating) => run(
                            () => applicationService.completeInterview(item.id, note, rating),
                            'Đã ghi nhận kết quả phỏng vấn.',
                          )}
                        />
                      ) : null}
                    </div>
                  </li>
                ))}
              </ul>
            )}
            <FormGrid>
              <Field label="Bắt đầu">
                <input type="datetime-local" value={interview.startsAt} onChange={event => setInterview({ ...interview, startsAt: event.target.value })} />
              </Field>
              <Field label="Kết thúc">
                <input type="datetime-local" value={interview.endsAt} onChange={event => setInterview({ ...interview, endsAt: event.target.value })} />
              </Field>
              <Field label="Địa điểm hoặc liên kết họp" wide>
                <input value={interview.place} onChange={event => setInterview({ ...interview, place: event.target.value })} placeholder="Văn phòng tầng 3 hoặc https://meet…" />
              </Field>
              <Field label="Ghi chú" wide>
                <input value={interview.note} onChange={event => setInterview({ ...interview, note: event.target.value })} />
              </Field>
            </FormGrid>
            <div className="form-actions">
              <button
                type="button"
                className="button"
                disabled={busy || !interview.startsAt || !interview.endsAt}
                onClick={() => run(
                  () => applicationService.scheduleInterview(applicationId, {
                    startsAtUtc: toUtcIsoString(interview.startsAt),
                    endsAtUtc: toUtcIsoString(interview.endsAt),
                    locationOrMeetingUrl: textOrNull(interview.place),
                    note: textOrNull(interview.note),
                  }),
                  'Đã tạo lịch phỏng vấn.',
                )}
              >
                Tạo lịch phỏng vấn
              </button>
            </div>
          </SectionCard>

          {canManage ? (
            <SectionCard title="Thư mời nhận việc" description="Chỉ Owner và HR được gửi thư mời.">
              {detail.data.offers.length === 0 ? <p className="muted">Chưa gửi thư mời nào.</p> : (
                <ul className="list">
                  {detail.data.offers.map(item => (
                    <li key={item.id}>
                      <div>
                        <b>{formatMoney(item.salary, item.currency)}</b>
                        <small>Bắt đầu {formatDate(item.startDate)} · Hết hạn {formatDate(item.expiresOn)}</small>
                        {item.note ? <small>{item.note}</small> : null}
                      </div>
                      <OfferBadge status={item.status} />
                    </li>
                  ))}
                </ul>
              )}
              <FormGrid>
                <Field label="Mức lương">
                  <input type="number" min={0} value={offer.salary} onChange={event => setOffer({ ...offer, salary: event.target.value })} />
                </Field>
                <Field label="Đơn vị tiền tệ">
                  <input value={offer.currency} onChange={event => setOffer({ ...offer, currency: event.target.value })} />
                </Field>
                <Field label="Ngày bắt đầu làm việc">
                  <input type="date" value={offer.startDate} onChange={event => setOffer({ ...offer, startDate: event.target.value })} />
                </Field>
                <Field label="Hạn phản hồi">
                  <input type="date" value={offer.expiresOn} onChange={event => setOffer({ ...offer, expiresOn: event.target.value })} />
                </Field>
                <Field label="Ghi chú" wide>
                  <textarea rows={3} value={offer.note} onChange={event => setOffer({ ...offer, note: event.target.value })} />
                </Field>
              </FormGrid>
              <div className="form-actions">
                <button
                  type="button"
                  className="button"
                  disabled={busy}
                  onClick={() => run(
                    () => applicationService.createOffer(applicationId, {
                      salary: numberOrNull(offer.salary),
                      currency: offer.currency.trim() || 'VND',
                      startDate: textOrNull(offer.startDate),
                      expiresOn: textOrNull(offer.expiresOn),
                      note: textOrNull(offer.note),
                    }),
                    'Đã gửi thư mời nhận việc.',
                  )}
                >
                  Gửi thư mời
                </button>
              </div>
            </SectionCard>
          ) : null}

          <SectionCard title="Lịch sử trạng thái">
            {detail.data.history.length === 0 ? <p className="muted">Chưa có thay đổi nào.</p> : (
              <ul className="timeline">
                {detail.data.history.map(entry => (
                  <li key={entry.id}>
                    <b>{applicationStatusLabels[entry.toStatus]}</b>
                    <small>{formatDateTime(entry.createdAtUtc)}</small>
                    {entry.note ? <p>{entry.note}</p> : null}
                  </li>
                ))}
              </ul>
            )}
          </SectionCard>
        </div>
      ) : null}
    </Modal>
  )
}

function CompleteInterviewButton({ busy, onComplete }: { busy: boolean; onComplete: (note: string | null, rating: number | null) => void }) {
  const [open, setOpen] = useState(false)
  const [note, setNote] = useState('')
  const [rating, setRating] = useState('')

  if (!open) return <button type="button" className="text-link" onClick={() => setOpen(true)}>Ghi kết quả</button>

  return (
    <div className="inline-form">
      <input value={note} onChange={event => setNote(event.target.value)} placeholder="Nhận xét" />
      <input type="number" min={1} max={5} value={rating} onChange={event => setRating(event.target.value)} placeholder="Điểm 1–5" />
      <button
        type="button"
        className="button button-small"
        disabled={busy}
        onClick={() => { onComplete(textOrNull(note), numberOrNull(rating)); setOpen(false) }}
      >
        Lưu
      </button>
      <button type="button" className="text-link" onClick={() => setOpen(false)}>Huỷ</button>
    </div>
  )
}
