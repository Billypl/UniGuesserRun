import { useEffect, useState } from 'react'
import styles from '../styles/PaginationButtons.module.scss'

const PaginationButtons = ({
	totalPages,
	currentPage,
	onChangePage,
	siblingCount = 1,
}: {
	totalPages: number
	currentPage: number
	onChangePage: (page: number) => void
	siblingCount?: number
}) => {
	const [pageNumbers, setPageNumbers] = useState<(number | '...')[]>([])

	useEffect(() => {
		if (totalPages > 1) {
			setupPages()
		}
	}, [totalPages, currentPage, siblingCount])

	const setupPages = () => {
		const rangeWithDots: (number | '...')[] = []

		const pages = new Set<number>()
		pages.add(1)
		pages.add(totalPages)
		pages.add(currentPage)

		// move the window of visible pages towards the middle when near the start or end
		const windowMiddle = Math.min(
			Math.max(currentPage, siblingCount + 3),
			totalPages - 2 - siblingCount
		)
		for (let i = windowMiddle - siblingCount; i <= windowMiddle + siblingCount; i++) {
			if (i > 1 && i < totalPages) {
				pages.add(i)
			}
		}

		const range = Array.from(pages).sort((a, b) => a - b)

		// fill gaps with ellipses (or single numbers if only one number is missing)
		let prev: number | undefined
		for (let i of range) {
			if (prev !== undefined) {
				if (i - prev === 2) {
					rangeWithDots.push(prev + 1) // fill single gap
				} else if (i - prev > 2) {
					rangeWithDots.push('...') // insert ellipsis for larger gaps
				}
			}
			rangeWithDots.push(i)
			prev = i
		}

		setPageNumbers(rangeWithDots)
	}

	const showButton = (pageNumber: number | '...') => {
		if (pageNumber === '...') {
			return (
				<div key={Math.random()} className={`${styles.ellipsis}`}>
					...
				</div>
			)
		}
		return (
			<div
				key={pageNumber}
				className={`${styles.pageButton} ${currentPage === pageNumber ? styles.activePage : ''}`}
				onClick={() => onChangePage(pageNumber)}
			>
				{pageNumber}
			</div>
		)
	}

	return totalPages > 1 ? (
		<div className={styles.pagesBox}>
			<div
				className={`${styles.pageButton} ${currentPage === 1 ? styles.disabled : ''}`}
				onClick={() => (currentPage > 1 ? onChangePage(Math.max(1, currentPage - 1)) : undefined)}
			>
				{'<'}
			</div>

			{pageNumbers.map(showButton)}

			<div
				className={`${styles.pageButton} ${currentPage === totalPages ? styles.disabled : ''}`}
				onClick={() =>
					currentPage < totalPages ? onChangePage(Math.min(totalPages, currentPage + 1)) : undefined
				}
			>
				{'>'}
			</div>
		</div>
	) : null
}

export default PaginationButtons
