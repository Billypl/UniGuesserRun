import { SortDirection } from "../scoreboard/SortDirection"
import { Difficulty } from "./Difficulty"

export interface UserHistoryQuery {
	difficultyLevel: Difficulty | null
	pageNumber: number
	pageSize: number
	sortDirection: SortDirection
}
