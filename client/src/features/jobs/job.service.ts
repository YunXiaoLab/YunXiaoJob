import { api, query } from '../../lib/api'
import type {
  EmploymentType, JobPosting, JobPostingDetail, JobPostingSummary, WorkplaceType,
} from '../../types/api'

export type JobSearchFilters = {
  keyword?: string
  location?: string
  employmentType?: EmploymentType | ''
  workplaceType?: WorkplaceType | ''
}

export type JobPostingInput = {
  title: string
  description: string
  requirements: string
  benefits: string | null
  location: string
  employmentType: EmploymentType
  workplaceType: WorkplaceType
  minSalary: number | null
  maxSalary: number | null
  currency: string
  applicationDeadline: string | null
  skills: string[]
}

export const jobService = {
  search: (filters: JobSearchFilters = {}) =>
    api<JobPostingSummary[]>(`/api/job-postings${query({
      keyword: filters.keyword,
      location: filters.location,
      employmentType: filters.employmentType === '' ? undefined : filters.employmentType,
      workplaceType: filters.workplaceType === '' ? undefined : filters.workplaceType,
    })}`, { anonymous: true }),
  detail: (id: string) => api<JobPostingDetail>(`/api/job-postings/${id}`, { anonymous: true }),
  byCompany: (companyId: string) => api<JobPosting[]>(`/api/job-postings/company/${companyId}`),
  create: (companyId: string, input: JobPostingInput) =>
    api<JobPosting>('/api/job-postings', { method: 'POST', body: { companyId, ...input } }),
  update: (id: string, input: JobPostingInput) =>
    api<JobPosting>(`/api/job-postings/${id}`, { method: 'PUT', body: input }),
  submit: (id: string) => api<JobPosting>(`/api/job-postings/${id}/submit`, { method: 'POST' }),
}
