import axios, { AxiosInstance } from 'axios'
import { GAME_API_URL, GAME_TOKEN_KEY, ACCOUNT_TOKEN_KEY, GAME_STATE, GAME_GUID } from '../../Constants'
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

	private getGameGuid(): string | null {
		return window.sessionStorage.getItem(GAME_GUID)
	}

	async startNewGameSession(nickname: string, difficulty: string, gameMode: string, signal?: AbortSignal) {
		const startData: StartGameData = { nickname, difficulty, gameMode }
		console.log(startData)
		const response = await this.axiosInstance.post<StartGameResponse>('/start', startData, {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
			},
			signal,
		})

		console.log(response.data)
		const { token, gameGuid } = response.data
		window.sessionStorage.setItem(GAME_TOKEN_KEY, token)
		window.sessionStorage.setItem(GAME_GUID, gameGuid)
	}

	async checkGameState(signal?: AbortSignal): Promise<GameSessionStateDto> {
		const gameGuid = this.getGameGuid()
		if (!gameGuid) throw new Error('Game GUID is missing')

		console.log('GAME GUID:', gameGuid)
		const url = `/${gameGuid}/game_state`
		const response = await this.axiosInstance.get<GameSessionStateDto>(url, {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(GAME_TOKEN_KEY)}`,
			},
			signal,
		})
		return response.data
	}

	async checkGameForUser(signal?: AbortSignal): Promise<GameSessionStateDto> {
		const url = `/active`
		const response = await this.axiosInstance.get<GameSessionStateDto>(url, {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
			},
			signal,
		})
		return response.data
	}

	async checkGuess(guessingCoordinates: Coordinates): Promise<RoundResultDto> {
		const gameGuid = this.getGameGuid()
		if (!gameGuid) throw new Error('Game GUID is missing')

		const url = `/${gameGuid}/check`
		const response = await this.axiosInstance.patch<RoundResultDto>(url, guessingCoordinates, {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(GAME_TOKEN_KEY)}`,
			},
		})
		return response.data
	}

	async getGuessingPlace(roundNumber: number): Promise<GuessingPlaceDto> {
		const gameGuid = this.getGameGuid()
		if (!gameGuid) throw new Error('Game GUID is missing')

		const url = `/${gameGuid}/round/${roundNumber}`
		const response = await this.axiosInstance.get<GuessingPlaceDto>(url, {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(GAME_TOKEN_KEY)}`,
			},
		})
		return response.data
	}

	async finishGame(): Promise<FinishedGameDto> {
		const gameGuid = this.getGameGuid()
		if (!gameGuid) throw new Error('Game GUID is missing')

		const url = `/${gameGuid}/finish`
		const response = await this.axiosInstance.patch<FinishedGameDto>(url, null, {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(GAME_TOKEN_KEY)}`,
			},
		})
		window.sessionStorage.removeItem(GAME_TOKEN_KEY)
		window.sessionStorage.removeItem(GAME_GUID)
		return response.data
	}

	async setUpGameTokenIfUserHasGame() {
		try {
			const state = await this.checkGameForUser()
			const accountToken = window.sessionStorage.getItem(ACCOUNT_TOKEN_KEY)
			window.sessionStorage.setItem(GAME_GUID, state.id)
			if (accountToken) {
				window.sessionStorage.setItem(GAME_TOKEN_KEY, accountToken)
			} else {
				console.error('Account token is null and cannot be set as GAME_TOKEN_KEY')
			}
		} catch (error) {
			console.error('Failed to check if the user has a game:', error)
		}
	}

	hasToken(): boolean {
		return !!window.sessionStorage.getItem(GAME_TOKEN_KEY)
	}
}

export default new GameService()
