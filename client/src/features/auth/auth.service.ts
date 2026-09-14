import { api } from '../../lib/api'
import type { Authentication, RegistrationChallenge, User } from './auth.types'
export const authService = {
  login: (email: string, password: string) => api<Authentication>('/api/auth/login', { method: 'POST', body: JSON.stringify({ email, password }) }),
  register: (fullName: string, email: string, password: string) => api<RegistrationChallenge>('/api/auth/register', { method: 'POST', body: JSON.stringify({ fullName, email, password }) }),
  verifyRegistration: (challengeId: string, otp: string) => api<User>('/api/auth/register/verify', { method: 'POST', body: JSON.stringify({ challengeId, otp }) }),
  forgotPassword: (email: string) => api<void>('/api/auth/forgot-password', { method: 'POST', body: JSON.stringify({ email }) }),
}
