import { api } from '../../lib/api'
import type {
  CandidateEducation, CandidateExperience, CandidateProfile, CandidateSkill, EducationLevel, EmploymentType,
  Gender, JobApplicationListItem, Resume, SkillProficiency,
} from '../../types/api'

export type CandidateProfileInput = {
  headline: string | null
  summary: string | null
  location: string | null
  yearsOfExperience: number | null
  isSearchable: boolean
  avatarUrl: string | null
  dateOfBirth: string | null
  gender: Gender
  expectedMinSalary: number | null
  expectedMaxSalary: number | null
  expectedSalaryCurrency: string | null
  linkedInUrl: string | null
  gitHubUrl: string | null
  portfolioUrl: string | null
}

export type EducationInput = {
  schoolName: string
  major: string | null
  level: EducationLevel
  startYear: number | null
  endYear: number | null
  gpa: number | null
  gpaScale: number | null
  isCurrent: boolean
  description: string | null
}

export type ExperienceInput = {
  companyName: string
  jobTitle: string
  employmentType: EmploymentType | null
  location: string | null
  startDate: string
  endDate: string | null
  isCurrent: boolean
  description: string | null
}

export type SkillInput = {
  name: string
  proficiency: SkillProficiency
  yearsOfExperience: number | null
  lastUsedYear: number | null
}

export const candidateService = {
  profile: () => api<CandidateProfile>('/api/candidate/profile'),
  updateProfile: (input: CandidateProfileInput) =>
    api<CandidateProfile>('/api/candidate/profile', { method: 'PUT', body: input }),
  addResume: (name: string, fileUrl: string, isDefault: boolean) =>
    api<Resume>('/api/candidate/resumes', { method: 'POST', body: { name, fileUrl, isDefault } }),
  saveEducations: (educations: EducationInput[]) =>
    api<CandidateEducation[]>('/api/candidate/educations', { method: 'PUT', body: { educations } }),
  saveExperiences: (experiences: ExperienceInput[]) =>
    api<CandidateExperience[]>('/api/candidate/experiences', { method: 'PUT', body: { experiences } }),
  saveSkills: (skills: SkillInput[]) =>
    api<CandidateSkill[]>('/api/candidate/skills', { method: 'PUT', body: { skills } }),
  applications: () => api<JobApplicationListItem[]>('/api/candidate/applications'),
}
