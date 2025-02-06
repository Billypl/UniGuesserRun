import axios, { AxiosInstance } from 'axios'
import { SCOREBOARD_API_URL, GAME_TOKEN_KEY } from '../../Constants'

export interface FinishedGame {
	id: number
	nickname: string
	finalScore: number
	// rounds: Round[];
	difficultyLevel: string
}

export interface PagedResult<FinishedGame> {
	items: FinishedGame[]
	totalPages: number
	itemFrom: number
	itemsTo: number
	totalItemsCount: number
}

export class ScoreboardService {
	private axiosInstance: AxiosInstance

	constructor() {
		this.axiosInstance = axios.create({
			baseURL: SCOREBOARD_API_URL,
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

	async getScores(): Promise<FinishedGame[]> {
		const result = await this.axiosInstance.get<PagedResult<FinishedGame>>('')
		return result.data.items
	}
}

export default new ScoreboardService()
