import { Difficulty } from './Difficulty'
import { GameMode } from './GameMode'

export interface StartGameData {
	nickname: string
	difficulty: Difficulty
	gameMode: GameMode
}
