import { UserRole } from './UserRole'

export interface AccountDetailsDto {
	guid: string
	nickname: string
	email: string
	role: UserRole
	createdAt: string
}
