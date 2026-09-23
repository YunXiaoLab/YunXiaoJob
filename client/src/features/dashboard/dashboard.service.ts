import { api } from '../../lib/api'
import type { CompanyDashboard, PlatformDashboard } from '../../types/api'

export const dashboardService = {
  platform: () => api<PlatformDashboard>('/api/dashboard/platform'),
  company: (companyId: string) => api<CompanyDashboard>(`/api/dashboard/company/${companyId}`),
}
