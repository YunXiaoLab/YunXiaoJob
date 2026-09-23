import { useState } from 'react'
import { Link } from 'react-router-dom'
import {
  Alert, ApplicationBadge, EmptyState, InterviewBadge, Loading, OfferBadge, SectionCard,
} from '../../components/ui/feedback'
import { Modal } from '../../components/ui/Modal'
import { useToast } from '../../components/ui/ToastProvider'
import { applicationService } from '../../features/applications/application.service'
import { candidateService } from '../../features/candidate/candidate.service'
import { errorMessage } from '../../lib/api'
import { formatDate, formatDateTime, formatMoney, formatSalaryRange } from '../../lib/format'
import { useAsync } from '../../lib/useAsync'
import { ApplicationStatus, OfferStatus, applicationStatusLabels } from '../../types/api'

const closedStatuses: number[] = [ApplicationStatus.Hired, ApplicationStatus.Rejected, ApplicationStatus.Withdrawn]

export function CandidateApplicationsPage() {
  const applications = useAsync(() => candidateService.applications(), [])
  const [openId, setOpenId] = useState<string | null>(null)
  const { notify } = useToast()
  const [error, setError] = useState('')

  const withdraw = async (id: string) => {
    if (!window.confirm('Rút hồ sơ ứng tuyển này?')) return
    setError('')
    try {
      await applicationService.withdraw(id)
      notify('Đã rút hồ sơ ứng tuyển.')
      applications.reload()
    } catch (reason) {
      setError(errorMessage(reason))
    }
  }

  return (
    <section className="page section">
      <p className="eyebrow">ỨNG VIÊN</p>
      <h1>Đơn ứng tuyển của tôi</h1>

      {applications.loading ? <Loading /> : null}
      <Alert tone="danger">{applications.error ?? error}</Alert>

      {applications.data?.length === 0 ? (
        <EmptyState title="Bạn chưa ứng tuyển vị trí nào" description="Tìm một công việc phù hợp và gửi hồ sơ ngay hôm nay.">
          <Link className="button" to="/jobs">Tìm việc làm →</Link>
        </EmptyState>
      ) : null}

      <div className="table-wrap">
        {applications.data && applications.data.length > 0 ? (
          <table className="data-table">
            <thead>
              <tr>
                <th>Vị trí</th>
                <th>Doanh nghiệp</th>
                <th>Ngày nộp</th>
                <th>Trạng thái</th>
                <th />
              </tr>
            </thead>
            <tbody>
              {applications.data.map(item => (
                <tr key={item.application.id}>
                  <td>
                    {item.job
                      ? <Link className="text-link" to={`/jobs/${item.job.id}`}>{item.job.title}</Link>
                      : 'Tin đã gỡ'}
                  </td>
                  <td>{item.job?.companyName ?? '—'}</td>
                  <td>{formatDate(item.application.submittedAtUtc)}</td>
                  <td><ApplicationBadge status={item.application.status} /></td>
                  <td className="row-actions">
                    <button type="button" className="text-link" onClick={() => setOpenId(item.application.id)}>Chi tiết</button>
                    {closedStatuses.includes(item.application.status) ? null : (
                      <button type="button" className="link-danger" onClick={() => withdraw(item.application.id)}>Rút hồ sơ</button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : null}
      </div>

      {openId ? (
        <ApplicationDetailModal
          applicationId={openId}
          onClose={() => setOpenId(null)}
          onChanged={() => applications.reload()}
        />
      ) : null}
    </section>
  )
}

function ApplicationDetailModal({ applicationId, onClose, onChanged }: { applicationId: string; onClose: () => void; onChanged: () => void }) {
  const detail = useAsync(() => applicationService.detail(applicationId), [applicationId])
  const { notify } = useToast()
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')

  const respond = async (offerId: string, accept: boolean) => {
    setBusy(true)
    setError('')
    try {
      await applicationService.respondToOffer(offerId, accept)
      notify(accept ? 'Bạn đã nhận thư mời nhận việc.' : 'Bạn đã từ chối thư mời.')
      detail.reload()
      onChanged()
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setBusy(false)
    }
  }

  return (
    <Modal title="Chi tiết đơn ứng tuyển" onClose={onClose} wide>
      {detail.loading ? <Loading /> : null}
      <Alert tone="danger">{detail.error ?? error}</Alert>

      {detail.data ? (
        <div className="stack">
          <SectionCard title={detail.data.job?.title ?? 'Tin tuyển dụng'} description={detail.data.job?.companyName}>
            <div className="job-meta">
              <ApplicationBadge status={detail.data.application.status} />
              <span>Nộp ngày {formatDate(detail.data.application.submittedAtUtc)}</span>
              {detail.data.job ? <span>{formatSalaryRange(detail.data.job.minSalary, detail.data.job.maxSalary, detail.data.job.currency)}</span> : null}
            </div>
            {detail.data.application.coverLetter
              ? <p className="prewrap">{detail.data.application.coverLetter}</p>
              : <p className="muted">Không có thư giới thiệu.</p>}
          </SectionCard>

          <SectionCard title="Lịch phỏng vấn">
            {detail.data.interviews.length === 0 ? <p className="muted">Chưa có lịch phỏng vấn.</p> : (
              <ul className="list">
                {detail.data.interviews.map(interview => (
                  <li key={interview.id}>
                    <div>
                      <b>{formatDateTime(interview.startsAtUtc)} → {formatDateTime(interview.endsAtUtc)}</b>
                      <small>{interview.locationOrMeetingUrl ?? 'Chưa có địa điểm/liên kết'}</small>
                      {interview.evaluationNote ? <small>Nhận xét: {interview.evaluationNote}</small> : null}
                    </div>
                    <InterviewBadge status={interview.status} />
                  </li>
                ))}
              </ul>
            )}
          </SectionCard>

          <SectionCard title="Thư mời nhận việc">
            {detail.data.offers.length === 0 ? <p className="muted">Chưa có thư mời nào.</p> : (
              <ul className="list">
                {detail.data.offers.map(offer => (
                  <li key={offer.id}>
                    <div>
                      <b>{formatMoney(offer.salary, offer.currency)}</b>
                      <small>Ngày bắt đầu: {formatDate(offer.startDate)} · Hạn phản hồi: {formatDate(offer.expiresOn)}</small>
                      {offer.note ? <small>{offer.note}</small> : null}
                    </div>
                    <div className="row-actions">
                      <OfferBadge status={offer.status} />
                      {offer.status === OfferStatus.Sent ? (
                        <>
                          <button type="button" className="button button-small" disabled={busy} onClick={() => respond(offer.id, true)}>Nhận việc</button>
                          <button type="button" className="link-danger" disabled={busy} onClick={() => respond(offer.id, false)}>Từ chối</button>
                        </>
                      ) : null}
                    </div>
                  </li>
                ))}
              </ul>
            )}
          </SectionCard>

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
