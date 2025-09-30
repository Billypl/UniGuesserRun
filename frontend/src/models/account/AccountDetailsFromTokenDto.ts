import { UserRole } from './UserRole'

export interface AccountDetailsFromTokenDto {
	userId: string
	nickname: string
	email: string
	role: UserRole
}
