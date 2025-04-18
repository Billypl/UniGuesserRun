import axios, { AxiosInstance } from 'axios'
import { GAME_API_URL, GAME_TOKEN_KEY, ACCOUNT_TOKEN_KEY,GAME_STATE } from '../../Constants'
import { Coordinates } from '../../models/Coordinates'
import { StartGameData } from '../../models/game/StartGameData'
import { StartGameResponse } from '../../models/game/StartGameResponse'
import { GuessingPlaceDto } from '../../models/game/GuessingPlaceDto'
import { RoundResultDto } from '../../models/game/RoundResultDto'
import { FinishedGameDto } from '../../models/game/SummarizeGameDto'
import { GameSessionStateDto } from '../../models/game/GameSessionState'

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

	async startNewGameSession(nickname: string, difficulty: string, signal?: AbortSignal) {
		const startData: StartGameData = {
			nickname: nickname,
			difficulty: difficulty,
		}

		const response = await this.axiosInstance.post<StartGameResponse>('/start', startData, {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
			},
			signal,
		})

		console.log(response.data.token)
		window.sessionStorage.setItem(GAME_TOKEN_KEY, response.data.token)
	}

	async checkGameState(signal?: AbortSignal):Promise<GameSessionStateDto> {
			const response = await this.axiosInstance.get<GameSessionStateDto>(GAME_STATE ,{
				headers: {
					Authorization: `Bearer ${sessionStorage.getItem(GAME_TOKEN_KEY)}`,
				},
				signal
			})
			console.log('game state:',response)
			return response.data;
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
		window.sessionStorage.removeItem(GAME_TOKEN_KEY)
		console.log('konczenie gry')
		return response.data
	}

	hasToken(): boolean {
		return !!window.sessionStorage.getItem(GAME_TOKEN_KEY)
	}
}

export default new GameService()
