import axios, { AxiosInstance } from 'axios'
import { GAME_API_URL, GAME_TOKEN_KEY, ACCOUNT_TOKEN_KEY } from '../../Constants'
import { Coordinates } from '../../models/Coordinates'
import { StartGameData } from '../../models/game/StartGameData'
import { StartGameResponse } from '../../models/game/StartGameResponse'
import { GuessingPlaceDto } from '../../models/game/GuessingPlaceDto'
import { RoundResultDto } from '../../models/game/RoundResultDto'
import { FinishedGameDto } from '../../models/game/SummarizeGameDto'

export class GameService {
	private axiosInstance: AxiosInstance

	constructor() {
		this.axiosInstance = axios.create({
			baseURL: GAME_API_URL,
			headers: {
				'Content-Type': 'application/json',
			},
		})
	}

	async startGame(nickname: string, difficulty: string) {
		const startData: StartGameData = {
			nickname: nickname,
			difficulty: difficulty,
		}
		const response = await this.axiosInstance.post<StartGameResponse>('/start', startData, {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
			},
		})
		window.sessionStorage.setItem(GAME_TOKEN_KEY, response.data.token)
	}

	async checkGuess(guessingCoordinates: Coordinates): Promise<RoundResultDto> {
		const response = await this.axiosInstance.patch<RoundResultDto>('/check', guessingCoordinates, {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(GAME_TOKEN_KEY)}`,
			},
		})
		return response.data
	}

	async getGuessingPlace(roundNumber: number): Promise<GuessingPlaceDto> {
		const response = await this.axiosInstance.get<GuessingPlaceDto>(`/round/${roundNumber}`, {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(GAME_TOKEN_KEY)}`,
			},
		})
		return response.data
	}

	async finishGame(): Promise<FinishedGameDto> {
		const response = await this.axiosInstance.patch<FinishedGameDto>('/finish', '', {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(GAME_TOKEN_KEY)}`,
			},
		})
		return response.data
	}

	hasToken(): boolean {
		return !!window.sessionStorage.getItem(GAME_TOKEN_KEY)
	}
}

export default new GameService()
