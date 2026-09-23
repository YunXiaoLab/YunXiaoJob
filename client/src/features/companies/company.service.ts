import { api } from '../../lib/api'
import type {
  Company, CompanyMemberDetail, CompanyMemberRole, CompanyMembership, CompanyRegistration, CompanyStaffAccount,
} from '../../types/api'

export type CompanyInput = {
  name: string
  description: string | null
  website: string | null
  logoUrl: string | null
  address: string | null
  industry: string | null
  employeeCount: number | null
}

export type CompanyRegistrationInput = {
  companyName: string
  website: string | null
  address: string | null
  industry: string | null
  description: string | null
  employeeCount: number | null
  contactFullName: string
  contactEmail: string
  contactPhoneNumber: string | null
}

export const companyService = {
  mine: () => api<CompanyMembership[]>('/api/companies/mine'),
  get: (id: string) => api<Company>(`/api/companies/${id}`, { anonymous: true }),
  members: (id: string) => api<CompanyMemberDetail[]>(`/api/companies/${id}/members`),
  create: (input: CompanyInput) => api<Company>('/api/companies', { method: 'POST', body: input }),
  update: (id: string, input: CompanyInput) => api<Company>(`/api/companies/${id}`, { method: 'PUT', body: input }),
  addMember: (id: string, userId: string, role: CompanyMemberRole) =>
    api<CompanyMemberDetail>(`/api/companies/${id}/members`, { method: 'POST', body: { userId, role } }),
  createStaff: (id: string, input: { email: string; fullName: string; phoneNumber: string | null; role: CompanyMemberRole }) =>
    api<CompanyStaffAccount>(`/api/companies/${id}/staff`, { method: 'POST', body: input }),
  submitRegistration: (input: CompanyRegistrationInput) =>
    api<CompanyRegistration>('/api/company-registrations', { method: 'POST', body: input, anonymous: true }),
}
