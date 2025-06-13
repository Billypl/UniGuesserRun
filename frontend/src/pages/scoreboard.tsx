import React, { useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import scoreboardService from '../services/api/scoreboardService'
import UserStats from '../models/scoreboard/UserStats'
import styles from '../styles/Scoreboard.module.scss'
import { MENU_ROUTE } from '../Constants'
import { ScoreboardQuery, SortDirection } from '../models/scoreboard/SearchQuery'
import { PagedResult } from '../models/scoreboard/PagedResult'

const Scoreboard: React.FC = () => {
	const navigate = useNavigate()
	const [records, setRecords] = useState<UserStats[]>([])
	const [scoreboardQuery, setScoreboardQuery] = useState<ScoreboardQuery>({
		searchNickname: '',
		difficultyLevel: 'easy',
		pageNumber: 1,
		pageSize: 3,
		sortDirection: SortDirection.DESC,
	})

	const [pagedResult, setPagedResult] = useState<PagedResult<UserStats>>({
		items: [],
		totalPages: 0,
		itemFrom: 0,
		itemsTo: 0,
		totalItemsCount: 0,
	})

	const [pageNumbers, setPageNumbers] = useState<number[]>([])

	const nicknameRef = useRef<HTMLInputElement>(null)
	const difficultyRef = useRef<HTMLSelectElement>(null)
	const pageSizeRef = useRef<HTMLSelectElement>(null)

	useEffect(() => {
		getAllRecords()
	}, [scoreboardQuery])

	useEffect(() => {
		setupPages()
	}, [pagedResult])

	const getAllRecords = async () => {
		try {
			const result = await scoreboardService.getScores(scoreboardQuery)
			setPagedResult(result)
			setRecords(result.items)
		} catch (error) {
			console.error('Error while fetching scores:', error)
		}
	}

	const setupPages = () => {
		const { totalPages } = pagedResult
		const currentPage = scoreboardQuery.pageNumber

		if (totalPages > 5) {
			const pages = new Set<number>()
			pages.add(1)
			pages.add(totalPages)
			pages.add(currentPage)

			if (currentPage + 1 <= totalPages) pages.add(currentPage + 1)
			if (currentPage - 1 > 0) pages.add(currentPage - 1)

			setPageNumbers(Array.from(pages).sort((a, b) => a - b))
		} else {
			setPageNumbers(Array.from({ length: totalPages }, (_, i) => i + 1))
		}
	}

	const handleFindClick = () => {
		const nickname = nicknameRef.current?.value || ''
		const difficulty = difficultyRef.current?.value || 'any'
		const pageSize = pageSizeRef.current?.value || '1'

		setScoreboardQuery({
			...scoreboardQuery,
			searchNickname: nickname,
			difficultyLevel: difficulty as 'easy' | 'medium' | 'hard' | 'any',
			pageNumber: 1,
			pageSize: parseInt(pageSize),
		})
	}

	const changePage = (pageNumber: number) => {
		if (pageNumber !== scoreboardQuery.pageNumber) {
			setScoreboardQuery(prev => ({
				...prev,
				pageNumber,
			}))
		}
	}

	const showRecord = (record: UserStats) => (
		<tr key={record.guid || record.nickname} className={styles.record}>
			<td>{record.guid}</td>
			<td>{record.nickname}</td>
			<td>{record.gamePlayed}</td>
			<td>{Number(record.averageScore.toFixed(4))}</td>
		</tr>
	)

	const showPage = (pageNumber: number) => (
		<div
			key={pageNumber}
			className={`${styles.boxNumber} ${scoreboardQuery.pageNumber === pageNumber ? styles.activePage : ''}`}
			onClick={() => changePage(pageNumber)}>
			{pageNumber}
		</div>
	)

	return (
		<div className={styles.container}>
			<h1>USERS</h1>

			<div className={styles.formBox}>
				<div className={styles.inputField}>
					<label htmlFor='nickname'>Nickname:</label>
					<input type='text' id='nickname' name='nickname' placeholder='Enter nickname' ref={nicknameRef} />
				</div>

				<div className={styles.inputField}>
					<label htmlFor='difficulty'>Difficulty:</label>
					<select id='difficulty' name='difficulty' ref={difficultyRef}>
						<option value='easy'>Easy</option>
						<option value='medium'>Medium</option>
						<option value='hard'>Hard</option>
					</select>
				</div>

				<div className={styles.inputField}>
					<label htmlFor='difficulty'>Page size:</label>
					<select
						className={styles.pageSizeSelect}
						id='difficulty'
						name='difficulty'
						ref={pageSizeRef}
						defaultValue='3'>
						<option value='1'>1</option>
						<option value='3'>3</option>
						<option value='5'>5</option>
					</select>
				</div>
			</div>

			<button className={`${styles.rankingButton} ${styles.findButton}`} onClick={handleFindClick}>
				Apply
			</button>

			<table>
				<thead>
					<tr>
						<th>User Id</th>
						<th>Nickname</th>
						<th>Game Played</th>
						<th>Average Score</th>
					</tr>
				</thead>
				<tbody className='records-list'>
					{records.length > 0 ? (
						records.map(showRecord)
					) : (
						<tr>
							<td colSpan={4}>
								No records found. 
							</td>
						</tr>
					)}
				</tbody>
			</table>

			<div className={styles.pagesBox}>{pageNumbers.map(showPage)}</div>

			<button className={`${styles.rankingButton} ${styles.backToMenuButton}`} onClick={() => navigate(MENU_ROUTE)}>
				Back to menu
			</button>
		</div>
	)
}

export default Scoreboard
