import { createContext, useCallback, useContext, useEffect, useMemo, useState, type ReactNode } from 'react'
import { errorMessage } from '../../lib/api'
import { useAuth } from '../auth/AuthProvider'
import { companyService } from './company.service'
import type { CompanyMembership } from '../../types/api'

const storageKey = 'yunxiaojob.company'

type CompanyContextValue = {
  memberships: CompanyMembership[]
  active: CompanyMembership | null
  loading: boolean
  error: string | undefined
  select: (companyId: string) => void
  reload: () => Promise<void>
}

const CompanyContext = createContext<CompanyContextValue | undefined>(undefined)

export function CompanyProvider({ children }: { children: ReactNode }) {
  const { user } = useAuth()
  const [memberships, setMemberships] = useState<CompanyMembership[]>([])
  const [activeId, setActiveId] = useState<string | null>(() => localStorage.getItem(storageKey))
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | undefined>(undefined)

  const load = useCallback(async () => {
    if (!user) { setMemberships([]); return }
    setLoading(true)
    setError(undefined)
    try {
      setMemberships(await companyService.mine())
    } catch (reason) {
      setError(errorMessage(reason))
    } finally {
      setLoading(false)
    }
  }, [user])

  useEffect(() => { void load() }, [load])

  const select = useCallback((companyId: string) => {
    setActiveId(companyId)
    localStorage.setItem(storageKey, companyId)
  }, [])

  const value = useMemo<CompanyContextValue>(() => {
    // A remembered company that the user no longer belongs to falls back to the first membership.
    const active = memberships.find(x => x.company.id === activeId) ?? memberships[0] ?? null
    return { memberships, active, loading, error, select, reload: load }
  }, [memberships, activeId, loading, error, select, load])

  return <CompanyContext.Provider value={value}>{children}</CompanyContext.Provider>
}

export function useCompanies() {
  const value = useContext(CompanyContext)
  if (!value) throw new Error('useCompanies must be used inside CompanyProvider')
  return value
}
