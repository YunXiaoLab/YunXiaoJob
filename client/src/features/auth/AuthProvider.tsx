import { createContext, useContext, useEffect, useState, type ReactNode } from 'react'
import { authService } from './auth.service'
import type { Authentication, RegistrationChallenge, User } from './auth.types'

type AuthContextValue = { user: User | null; accessToken: string | null; login(email: string, password: string): Promise<void>; logout(): void; register(fullName: string, email: string, password: string): Promise<RegistrationChallenge>; verifyRegistration(challengeId: string, otp: string): Promise<void> }
const AuthContext = createContext<AuthContextValue | undefined>(undefined)
const storageKey = 'yunxiaojob.session'

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<Authentication | null>(null)
  useEffect(() => { const value = localStorage.getItem(storageKey); if (value) setSession(JSON.parse(value) as Authentication) }, [])
  const save = (value: Authentication) => { localStorage.setItem(storageKey, JSON.stringify(value)); setSession(value) }
  return <AuthContext.Provider value={{ user: session?.user ?? null, accessToken: session?.accessToken ?? null, login: async (email, password) => save(await authService.login(email, password)), register: (fullName, email, password) => authService.register(fullName, email, password), verifyRegistration: async (challengeId, otp) => { await authService.verifyRegistration(challengeId, otp) }, logout: () => { localStorage.removeItem(storageKey); setSession(null) } }}>{children}</AuthContext.Provider>
}
export function useAuth() { const value = useContext(AuthContext); if (!value) throw new Error('useAuth must be used inside AuthProvider'); return value }
