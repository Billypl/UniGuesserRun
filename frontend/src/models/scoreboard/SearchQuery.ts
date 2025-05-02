export enum SortDirection {
	ASC = 'ASC',
	DESC = 'DESC',
}

export interface ScoreboardQuery {
	searchNickname?: string
	difficultyLevel?: string
	pageNumber: number
	pageSize: number
	sortDirection: SortDirection
}
