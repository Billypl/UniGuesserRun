import axios, { AxiosInstance } from 'axios'
import { SESSIONS_API_URL, GAME_TOKEN_KEY } from '../../Constants'
import FinishedGame from '../../models/scoreboard/UserStats'
import { PagedResult } from '../../models/scoreboard/PagedResult'
import { ScoreboardQuery } from '../../models/scoreboard/SearchQuery'

export class ScoreboardService {
	private axiosInstance: AxiosInstance

	constructor() {
		this.axiosInstance = axios.create({
			baseURL: SESSIONS_API_URL,
			headers: {
				'Content-Type': 'application/json',
			},
		})
	}

	saveScore() {
		return this.axiosInstance.post('/save_score', '', {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(GAME_TOKEN_KEY)}`,
			},
		})
	}

	async getScores(scoreboardQuery: ScoreboardQuery): Promise<PagedResult<FinishedGame>> {
		const result = await this.axiosInstance.get<PagedResult<FinishedGame>>('/scoreboard', {
			params: {
				PageNumber: scoreboardQuery.pageNumber,
				PageSize: scoreboardQuery.pageSize,
				SortDirection: scoreboardQuery.sortDirection,
				SearchNickname: scoreboardQuery.searchNickname,
				DifficultyLevel: scoreboardQuery.difficultyLevel,
			},
		})

		console.log(result.data)
		return result.data
	}
}

export default new ScoreboardService()
