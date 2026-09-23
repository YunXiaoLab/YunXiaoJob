import { api, query } from '../../lib/api'
import type {
  Company, CompanyRegistration, CompanyRegistrationStatus, CompanyStatus, JobPostingStatus, JobPostingSummary,
  User,
} from '../../types/api'

export const adminService = {
  users: () => api<User[]>('/api/admin/users'),
  setUserActive: (id: string, isActive: boolean) =>
    api<User>(`/api/admin/users/${id}/active${query({ isActive })}`, { method: 'PUT' }),
  companies: (status?: CompanyStatus | '') =>
    api<Company[]>(`/api/admin/companies${query({ status: status === '' ? undefined : status })}`),
  jobPostings: (status?: JobPostingStatus | '') =>
    api<JobPostingSummary[]>(`/api/admin/job-postings${query({ status: status === '' ? undefined : status })}`),
  registrations: (status?: CompanyRegistrationStatus | '') =>
    api<CompanyRegistration[]>(`/api/admin/company-registrations${query({ status: status === '' ? undefined : status })}`),
  approveRegistration: (id: string, note: string | null) =>
    api<CompanyRegistration>(`/api/admin/company-registrations/${id}/approve`, { method: 'POST', body: { note } }),
  rejectRegistration: (id: string, note: string | null) =>
    api<CompanyRegistration>(`/api/admin/company-registrations/${id}/reject`, { method: 'POST', body: { note } }),
  approveCompany: (id: string) => api<Company>(`/api/admin/companies/${id}/approve`, { method: 'POST' }),
  approveJobPosting: (id: string) => api<unknown>(`/api/admin/job-postings/${id}/approve`, { method: 'POST' }),
}
