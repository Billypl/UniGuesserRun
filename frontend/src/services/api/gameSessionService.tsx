import axios, { AxiosError, AxiosInstance } from 'axios'
import { ACCOUNT_TOKEN_KEY, SESSIONS_API_URL } from '../../Constants'
import { UserHistoryQuery } from '../../models/game/UserHistoryQuery'
import { PagedResult } from '../../models/scoreboard/PagedResult'
import { FinishedGameDto } from '../../models/game/FinishedGameDto'

export class GameSessionService {
	private axiosInstance: AxiosInstance

	constructor() {
		this.axiosInstance = axios.create({
			baseURL: SESSIONS_API_URL,
			headers: {
				'Content-Type': 'application/json',
			},
		})
	}

	async getHistoryPagesByUser(userId: string, userHistoryQuery: UserHistoryQuery): Promise<PagedResult<FinishedGameDto>> {
		const response = await this.axiosInstance.get<PagedResult<FinishedGameDto>>(`/history/user/${userId}`, {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
			},
			params: userHistoryQuery,
		})
		return response.data
	}

	async getResultDetails(gameId: string): Promise<FinishedGameDto> {
		const response = await this.axiosInstance.get<FinishedGameDto>(`/${gameId}`, {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
			},
		})
		return response.data
	}
}

export default new GameSessionService()
