import { SortDirection } from './SortDirection'

export interface ScoreboardQuery {
	searchNickname?: string
	difficultyLevel?: string
	pageNumber: number
	pageSize: number
	sortDirection: SortDirection
}
