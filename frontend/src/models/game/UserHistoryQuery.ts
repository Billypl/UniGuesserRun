import { SortDirection } from "../scoreboard/SortDirection"

export interface UserHistoryQuery {
	difficultyLevel: string | null
	pageNumber: number
	pageSize: number
	sortDirection: SortDirection
}
