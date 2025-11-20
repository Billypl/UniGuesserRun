import { Difficulty } from '../game/Difficulty'
import { SortDirection } from './SortDirection'

export interface ScoreboardQuery {
	searchNickname?: string
	difficultyLevel?: Difficulty
	pageNumber: number
	pageSize: number
	sortDirection: SortDirection
}
