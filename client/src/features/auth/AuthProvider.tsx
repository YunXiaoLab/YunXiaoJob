import { createContext, useCallback, useContext, useEffect, useMemo, useRef, useState, type ReactNode } from 'react'
import { connectAuth } from '../../lib/api'
import { authService } from './auth.service'
import { PlatformRole, type Authentication, type RegistrationChallenge, type User } from '../../types/api'

const storageKey = 'yunxiaojob.session'

type AuthContextValue = {
  user: User | null
  isAdmin: boolean
  ready: boolean
  login: (email: string, password: string) => Promise<User>
  logout: () => void
  register: (fullName: string, email: string, password: string) => Promise<RegistrationChallenge>
  resendOtp: (challengeId: string) => Promise<RegistrationChallenge>
  verifyRegistration: (challengeId: string, otp: string) => Promise<User>
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined)

function read(): Authentication | null {
  try {
    const value = localStorage.getItem(storageKey)
    return value ? (JSON.parse(value) as Authentication) : null
  } catch {
    return null
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<Authentication | null>(null)
  const [ready, setReady] = useState(false)
  // The request layer reads the token synchronously, so it has to see the newest value, not a render-old closure.
  const current = useRef<Authentication | null>(null)
  const pendingRefresh = useRef<Promise<string | null> | null>(null)

  const store = useCallback((value: Authentication | null) => {
    current.current = value
    setSession(value)
    if (value) localStorage.setItem(storageKey, JSON.stringify(value))
    else localStorage.removeItem(storageKey)
  }, [])

  useEffect(() => {
    const restored = read()
    current.current = restored
    setSession(restored)
    setReady(true)
  }, [])

  useEffect(() => {
    connectAuth({
      accessToken: () => current.current?.accessToken ?? null,
      signOut: () => store(null),
      refresh: () => {
        const token = current.current?.refreshToken
        if (!token) return Promise.resolve(null)
        // Several requests can fail at once; they all wait on the same renewal.
        pendingRefresh.current ??= authService.refresh(token)
          .then(value => { store(value); return value.accessToken })
          .catch(() => { store(null); return null })
          .finally(() => { pendingRefresh.current = null })
        return pendingRefresh.current
      },
    })
    return () => connectAuth(null)
  }, [store])

  const value = useMemo<AuthContextValue>(() => ({
    user: session?.user ?? null,
    isAdmin: session?.user.platformRole === PlatformRole.Admin,
    ready,
    login: async (email, password) => {
      const authentication = await authService.login(email, password)
      store(authentication)
      return authentication.user
    },
    logout: () => store(null),
    register: (fullName, email, password) => authService.register(fullName, email, password),
    resendOtp: challengeId => authService.resendRegistrationOtp(challengeId),
    verifyRegistration: (challengeId, otp) => authService.verifyRegistration(challengeId, otp),
  }), [session, ready, store])

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const value = useContext(AuthContext)
  if (!value) throw new Error('useAuth must be used inside AuthProvider')
  return value
}
