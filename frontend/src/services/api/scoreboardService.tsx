import axios, { AxiosInstance } from 'axios'

const API_URL = 'https://localhost:7157/api/scoreboard'

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
			baseURL: API_URL,
			headers: {
				'Content-Type': 'application/json',
			},
		})
	}

	saveScore() {
		return this.axiosInstance.post('/save_score', '', {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem('token')}`,
			},
		})
	}

	async getScores(): Promise<FinishedGame[]> {
		const result = await this.axiosInstance.get<PagedResult<FinishedGame>>('')
		return result.data.items
	}
}

export default new ScoreboardService()
