import { api, query } from '../../lib/api'
import type {
  ApplicationStatus, Interview, JobApplication, JobApplicationDetail, JobApplicationListItem, JobOffer,
} from '../../types/api'

export const applicationService = {
  detail: (id: string) => api<JobApplicationDetail>(`/api/applications/${id}`),
  forJob: (jobPostingId: string, status?: ApplicationStatus | '') =>
    api<JobApplicationListItem[]>(`/api/applications/job-posting/${jobPostingId}${query({ status: status === '' ? undefined : status })}`),
  apply: (jobPostingId: string, resumeId: string, coverLetter: string | null) =>
    api<JobApplication>('/api/applications', { method: 'POST', body: { jobPostingId, resumeId, coverLetter } }),
  withdraw: (id: string) => api<JobApplication>(`/api/applications/${id}/withdraw`, { method: 'POST' }),
  changeStatus: (id: string, status: ApplicationStatus, note: string | null) =>
    api<JobApplication>(`/api/applications/${id}/status`, { method: 'PUT', body: { status, note } }),
  assignRecruiter: (id: string, recruiterMemberId: string) =>
    api<JobApplication>(`/api/applications/${id}/recruiter`, { method: 'PUT', body: { recruiterMemberId } }),
  scheduleInterview: (id: string, input: { startsAtUtc: string; endsAtUtc: string; locationOrMeetingUrl: string | null; note: string | null }) =>
    api<Interview>(`/api/applications/${id}/interviews`, { method: 'POST', body: input }),
  completeInterview: (interviewId: string, evaluationNote: string | null, rating: number | null) =>
    api<Interview>(`/api/applications/interviews/${interviewId}/complete`, { method: 'POST', body: { evaluationNote, rating } }),
  createOffer: (id: string, input: { salary: number | null; currency: string; startDate: string | null; expiresOn: string | null; note: string | null }) =>
    api<JobOffer>(`/api/applications/${id}/offers`, { method: 'POST', body: input }),
  respondToOffer: (offerId: string, accept: boolean) =>
    api<JobOffer>(`/api/applications/offers/${offerId}/respond`, { method: 'POST', body: { accept } }),
}
