export interface GameSessionStateDto {
	id: string
	actualRoundNumber: number
	difficulty: string
	expirationDate: Date
	gameStatus: string
	gameMode: string
}
