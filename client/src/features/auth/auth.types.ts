export type User = { id: string; email: string; fullName: string; phoneNumber?: string; platformRole: number; isActive: boolean }
export type Authentication = { accessToken: string; refreshToken: string; refreshTokenExpiresAtUtc: string; user: User }
export type RegistrationChallenge = { challengeId: string; expiresAtUtc: string; resendAvailableAtUtc: string }
