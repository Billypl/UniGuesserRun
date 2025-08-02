export interface PagedResult<FinishedGame> {
	items: FinishedGame[];
	totalPages: number;
	itemFrom: number;
	itemsTo: number;
	totalItemsCount: number;
}
