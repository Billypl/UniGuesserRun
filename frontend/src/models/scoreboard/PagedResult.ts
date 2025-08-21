export interface PagedResult<T> {
	items: T[]
	totalPages: number
	itemFrom: number
	itemsTo: number
	totalItemsCount: number
}
