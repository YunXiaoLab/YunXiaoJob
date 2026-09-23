import type { ReactNode } from 'react'
import {
  ApplicationStatus, CompanyStatus, JobPostingStatus, OfferStatus, applicationStatusLabels, companyStatusLabels,
  interviewStatusLabels, jobPostingStatusLabels, offerStatusLabels, type InterviewStatus,
} from '../../types/api'

export type Tone = 'neutral' | 'info' | 'success' | 'warning' | 'danger'

export function Badge({ children, tone = 'neutral' }: { children: ReactNode; tone?: Tone }) {
  return <span className={`badge badge-${tone}`}>{children}</span>
}

export function Alert({ children, tone = 'info' }: { children: ReactNode; tone?: Tone }) {
  return children ? <p className={`alert alert-${tone}`}>{children}</p> : null
}

export function Loading({ label = 'Đang tải…' }: { label?: string }) {
  return <p className="loading">{label}</p>
}

export function EmptyState({ title, description, children }: { title: string; description?: string; children?: ReactNode }) {
  return (
    <div className="empty-state">
      <b>{title}</b>
      {description ? <p>{description}</p> : null}
      {children}
    </div>
  )
}

export function SectionCard({ title, description, actions, children }: { title: string; description?: string; actions?: ReactNode; children: ReactNode }) {
  return (
    <section className="panel">
      <header className="panel-head">
        <div>
          <h2>{title}</h2>
          {description ? <p>{description}</p> : null}
        </div>
        {actions ? <div className="panel-actions">{actions}</div> : null}
      </header>
      {children}
    </section>
  )
}

export function StatTile({ label, value, hint }: { label: string; value: number | string; hint?: string }) {
  return (
    <div className="stat-tile">
      <span>{label}</span>
      <b>{value}</b>
      {hint ? <small>{hint}</small> : null}
    </div>
  )
}

export function applicationTone(status: ApplicationStatus): Tone {
  if (status === ApplicationStatus.Hired) return 'success'
  if (status === ApplicationStatus.Rejected || status === ApplicationStatus.Withdrawn) return 'danger'
  if (status === ApplicationStatus.Offered || status === ApplicationStatus.Interviewing) return 'info'
  if (status === ApplicationStatus.Shortlisted) return 'warning'
  return 'neutral'
}

export function jobTone(status: JobPostingStatus): Tone {
  if (status === JobPostingStatus.Published) return 'success'
  if (status === JobPostingStatus.PendingApproval) return 'warning'
  if (status === JobPostingStatus.Rejected || status === JobPostingStatus.Closed || status === JobPostingStatus.Expired) return 'danger'
  return 'neutral'
}

export function companyTone(status: CompanyStatus): Tone {
  if (status === CompanyStatus.Active) return 'success'
  if (status === CompanyStatus.PendingVerification) return 'warning'
  return 'danger'
}

export function offerTone(status: OfferStatus): Tone {
  if (status === OfferStatus.Accepted) return 'success'
  if (status === OfferStatus.Sent) return 'info'
  if (status === OfferStatus.Rejected || status === OfferStatus.Withdrawn || status === OfferStatus.Expired) return 'danger'
  return 'neutral'
}

export function ApplicationBadge({ status }: { status: ApplicationStatus }) {
  return <Badge tone={applicationTone(status)}>{applicationStatusLabels[status]}</Badge>
}

export function JobStatusBadge({ status }: { status: JobPostingStatus }) {
  return <Badge tone={jobTone(status)}>{jobPostingStatusLabels[status]}</Badge>
}

export function CompanyStatusBadge({ status }: { status: CompanyStatus }) {
  return <Badge tone={companyTone(status)}>{companyStatusLabels[status]}</Badge>
}

export function OfferBadge({ status }: { status: OfferStatus }) {
  return <Badge tone={offerTone(status)}>{offerStatusLabels[status]}</Badge>
}

export function InterviewBadge({ status }: { status: InterviewStatus }) {
  return <Badge tone="info">{interviewStatusLabels[status]}</Badge>
}
