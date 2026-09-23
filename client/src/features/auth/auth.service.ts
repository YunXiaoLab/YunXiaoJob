import { api } from '../../lib/api'
import type { Authentication, RegistrationChallenge, User } from '../../types/api'

export const authService = {
  register: (fullName: string, email: string, password: string) =>
    api<RegistrationChallenge>('/api/auth/register', { method: 'POST', body: { fullName, email, password }, anonymous: true }),
  verifyRegistration: (challengeId: string, otp: string) =>
    api<User>('/api/auth/register/verify', { method: 'POST', body: { challengeId, otp }, anonymous: true }),
  resendRegistrationOtp: (challengeId: string) =>
    api<RegistrationChallenge>('/api/auth/register/resend', { method: 'POST', body: { challengeId }, anonymous: true }),
  login: (email: string, password: string) =>
    api<Authentication>('/api/auth/login', { method: 'POST', body: { email, password }, anonymous: true }),
  refresh: (refreshToken: string) =>
    api<Authentication>('/api/auth/refresh', { method: 'POST', body: { refreshToken }, anonymous: true }),
  forgotPassword: (email: string) =>
    api<void>('/api/auth/forgot-password', { method: 'POST', body: { email }, anonymous: true }),
  resetPassword: (token: string, newPassword: string) =>
    api<void>('/api/auth/reset-password', { method: 'POST', body: { token, newPassword }, anonymous: true }),
}
