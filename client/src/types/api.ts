// Mirrors YunXiaoJob.Application.DTOs and YunXiaoJob.Domain.Enums.
// The API serialises enums as numbers, so every enum here is a numeric union with a Vietnamese label map.

export const PlatformRole = { User: 0, Admin: 1 } as const
export type PlatformRole = (typeof PlatformRole)[keyof typeof PlatformRole]

export const Gender = { Unspecified: 0, Male: 1, Female: 2, Other: 3 } as const
export type Gender = (typeof Gender)[keyof typeof Gender]

export const EducationLevel = { HighSchool: 0, Vocational: 1, Associate: 2, Bachelor: 3, Master: 4, Doctorate: 5, Other: 6 } as const
export type EducationLevel = (typeof EducationLevel)[keyof typeof EducationLevel]

export const EmploymentType = { FullTime: 0, PartTime: 1, Contract: 2, Internship: 3, Freelance: 4 } as const
export type EmploymentType = (typeof EmploymentType)[keyof typeof EmploymentType]

export const WorkplaceType = { OnSite: 0, Hybrid: 1, Remote: 2 } as const
export type WorkplaceType = (typeof WorkplaceType)[keyof typeof WorkplaceType]

export const SkillProficiency = { Beginner: 0, Intermediate: 1, Advanced: 2, Expert: 3 } as const
export type SkillProficiency = (typeof SkillProficiency)[keyof typeof SkillProficiency]

export const CompanyStatus = { PendingVerification: 0, Active: 1, Rejected: 2, Suspended: 3 } as const
export type CompanyStatus = (typeof CompanyStatus)[keyof typeof CompanyStatus]

export const CompanyMemberRole = { Owner: 0, HR: 1, Recruiter: 2 } as const
export type CompanyMemberRole = (typeof CompanyMemberRole)[keyof typeof CompanyMemberRole]

export const CompanyRegistrationStatus = { Pending: 0, Approved: 1, Rejected: 2 } as const
export type CompanyRegistrationStatus = (typeof CompanyRegistrationStatus)[keyof typeof CompanyRegistrationStatus]

export const JobPostingStatus = { Draft: 0, PendingApproval: 1, Published: 2, Paused: 3, Closed: 4, Expired: 5, Rejected: 6 } as const
export type JobPostingStatus = (typeof JobPostingStatus)[keyof typeof JobPostingStatus]

export const ApplicationStatus = { Submitted: 0, Reviewing: 1, Shortlisted: 2, Interviewing: 3, Offered: 4, Hired: 5, Rejected: 6, Withdrawn: 7 } as const
export type ApplicationStatus = (typeof ApplicationStatus)[keyof typeof ApplicationStatus]

export const InterviewStatus = { Scheduled: 0, Confirmed: 1, Completed: 2, Cancelled: 3, NoShow: 4 } as const
export type InterviewStatus = (typeof InterviewStatus)[keyof typeof InterviewStatus]

export const OfferStatus = { Draft: 0, Sent: 1, Accepted: 2, Rejected: 3, Withdrawn: 4, Expired: 5 } as const
export type OfferStatus = (typeof OfferStatus)[keyof typeof OfferStatus]

export const NotificationType = { ApplicationStatusChanged: 0, InterviewScheduled: 1, OfferSent: 2, System: 3 } as const
export type NotificationType = (typeof NotificationType)[keyof typeof NotificationType]

export const genderLabels: Record<number, string> = { 0: 'Không tiết lộ', 1: 'Nam', 2: 'Nữ', 3: 'Khác' }
export const educationLevelLabels: Record<number, string> = { 0: 'THPT', 1: 'Trung cấp', 2: 'Cao đẳng', 3: 'Đại học', 4: 'Thạc sĩ', 5: 'Tiến sĩ', 6: 'Khác' }
export const employmentTypeLabels: Record<number, string> = { 0: 'Toàn thời gian', 1: 'Bán thời gian', 2: 'Hợp đồng', 3: 'Thực tập', 4: 'Freelance' }
export const workplaceTypeLabels: Record<number, string> = { 0: 'Tại văn phòng', 1: 'Hybrid', 2: 'Từ xa' }
export const skillProficiencyLabels: Record<number, string> = { 0: 'Cơ bản', 1: 'Trung bình', 2: 'Thành thạo', 3: 'Chuyên gia' }
export const companyStatusLabels: Record<number, string> = { 0: 'Chờ duyệt', 1: 'Đang hoạt động', 2: 'Bị từ chối', 3: 'Tạm ngưng' }
export const companyMemberRoleLabels: Record<number, string> = { 0: 'Chủ sở hữu', 1: 'HR', 2: 'Recruiter' }
export const companyRegistrationStatusLabels: Record<number, string> = { 0: 'Chờ duyệt', 1: 'Đã duyệt', 2: 'Từ chối' }
export const jobPostingStatusLabels: Record<number, string> = { 0: 'Nháp', 1: 'Chờ duyệt', 2: 'Đang đăng', 3: 'Tạm dừng', 4: 'Đã đóng', 5: 'Hết hạn', 6: 'Bị từ chối' }
export const applicationStatusLabels: Record<number, string> = { 0: 'Mới nộp', 1: 'Đang xem xét', 2: 'Vào vòng trong', 3: 'Phỏng vấn', 4: 'Đã gửi offer', 5: 'Đã tuyển', 6: 'Từ chối', 7: 'Đã rút' }
export const interviewStatusLabels: Record<number, string> = { 0: 'Đã lên lịch', 1: 'Đã xác nhận', 2: 'Hoàn tất', 3: 'Đã huỷ', 4: 'Vắng mặt' }
export const offerStatusLabels: Record<number, string> = { 0: 'Nháp', 1: 'Đã gửi', 2: 'Đã nhận', 3: 'Đã từ chối', 4: 'Đã thu hồi', 5: 'Hết hạn' }
export const notificationTypeLabels: Record<number, string> = { 0: 'Trạng thái hồ sơ', 1: 'Lịch phỏng vấn', 2: 'Thư mời nhận việc', 3: 'Hệ thống' }

export type User = {
  id: string
  email: string
  fullName: string
  phoneNumber: string | null
  platformRole: PlatformRole
  isActive: boolean
}

export type Authentication = {
  accessToken: string
  refreshToken: string
  refreshTokenExpiresAtUtc: string
  user: User
}

export type RegistrationChallenge = { challengeId: string; expiresAtUtc: string; resendAvailableAtUtc: string }

export type Resume = { id: string; name: string; fileUrl: string; isDefault: boolean }

export type CandidateEducation = {
  id: string
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

export type CandidateExperience = {
  id: string
  companyName: string
  jobTitle: string
  employmentType: EmploymentType | null
  location: string | null
  startDate: string
  endDate: string | null
  isCurrent: boolean
  description: string | null
  totalMonths: number
}

export type CandidateSkill = {
  id: string
  name: string
  proficiency: SkillProficiency
  yearsOfExperience: number | null
  lastUsedYear: number | null
}

export type CandidateProfile = {
  id: string
  userId: string
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
  resumes: Resume[]
  educations: CandidateEducation[]
  experiences: CandidateExperience[]
  skills: CandidateSkill[]
}

export type CandidateSummary = {
  id: string
  userId: string
  fullName: string
  email: string
  phoneNumber: string | null
  headline: string | null
  summary: string | null
  location: string | null
  yearsOfExperience: number | null
  avatarUrl: string | null
  linkedInUrl: string | null
  gitHubUrl: string | null
  portfolioUrl: string | null
  skills: CandidateSkill[]
  resume: Resume | null
}

export type Company = {
  id: string
  name: string
  logoUrl: string | null
  description: string | null
  website: string | null
  address: string | null
  industry: string | null
  employeeCount: number | null
  status: CompanyStatus
}

export type CompanyMembership = { company: Company; memberId: string; role: CompanyMemberRole; isActive: boolean }

export type CompanyMemberDetail = {
  id: string
  companyId: string
  userId: string
  email: string
  fullName: string
  phoneNumber: string | null
  role: CompanyMemberRole
  isActive: boolean
}

export type CompanyStaffAccount = {
  memberId: string
  companyId: string
  userId: string
  email: string
  fullName: string
  role: CompanyMemberRole
  isActive: boolean
}

export type JobPosting = {
  id: string
  companyId: string
  createdByMemberId: string
  assignedRecruiterMemberId: string | null
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
  status: JobPostingStatus
  publishedAtUtc: string | null
  skills: string[]
}

export type JobPostingSummary = {
  id: string
  companyId: string
  companyName: string
  companyLogoUrl: string | null
  title: string
  location: string
  employmentType: EmploymentType
  workplaceType: WorkplaceType
  minSalary: number | null
  maxSalary: number | null
  currency: string
  applicationDeadline: string | null
  status: JobPostingStatus
  publishedAtUtc: string | null
  skills: string[]
}

export type JobPostingDetail = { job: JobPosting; company: Company }

export type JobApplication = {
  id: string
  jobPostingId: string
  candidateProfileId: string
  resumeId: string
  assignedRecruiterMemberId: string | null
  coverLetter: string | null
  status: ApplicationStatus
  submittedAtUtc: string
}

export type Interview = {
  id: string
  jobApplicationId: string
  startsAtUtc: string
  endsAtUtc: string
  locationOrMeetingUrl: string | null
  status: InterviewStatus
  evaluationNote: string | null
  rating: number | null
}

export type JobOffer = {
  id: string
  jobApplicationId: string
  salary: number | null
  currency: string
  startDate: string | null
  expiresOn: string | null
  note: string | null
  status: OfferStatus
}

export type ApplicationStatusHistory = {
  id: string
  fromStatus: ApplicationStatus | null
  toStatus: ApplicationStatus
  note: string | null
  createdAtUtc: string
}

export type JobApplicationListItem = {
  application: JobApplication
  job: JobPostingSummary | null
  candidate: CandidateSummary | null
}

export type JobApplicationDetail = {
  application: JobApplication
  job: JobPostingSummary | null
  candidate: CandidateSummary | null
  interviews: Interview[]
  offers: JobOffer[]
  history: ApplicationStatusHistory[]
}

export type Notification = {
  id: string
  title: string
  content: string
  type: NotificationType
  targetUrl: string | null
  isRead: boolean
  createdAtUtc: string
}

export type PlatformDashboard = {
  users: number
  candidates: number
  companies: number
  activeCompanies: number
  jobPostings: number
  publishedJobPostings: number
  applications: number
}

export type CompanyDashboard = {
  jobPostings: number
  publishedJobPostings: number
  applications: number
  applicationsByStatus: Record<string, number>
}

export type CompanyRegistration = {
  id: string
  companyName: string
  website: string | null
  address: string | null
  industry: string | null
  description: string | null
  employeeCount: number | null
  contactFullName: string
  contactEmail: string
  contactPhoneNumber: string | null
  status: CompanyRegistrationStatus
  reviewNote: string | null
  reviewedAtUtc: string | null
  createdCompanyId: string | null
  createdOwnerUserId: string | null
  createdAtUtc: string
}
