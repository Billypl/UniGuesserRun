import React, { useEffect, useRef, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import Header from '../components/Header'
import scoreboardService from '../services/api/scoreboardService'
import UserStats from '../models/scoreboard/UserStats'
import styles from '../styles/Scoreboard.module.scss'
import { MENU_ROUTE } from '../Constants'
import { ScoreboardQuery } from '../models/scoreboard/SearchQuery'
import { SortDirection } from '../models/scoreboard/SortDirection'
import { PagedResult } from '../models/scoreboard/PagedResult'
import PaginationButtons from '../components/PaginationButtons'

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

	const nicknameRef = useRef<HTMLInputElement>(null)
	const difficultyRef = useRef<HTMLSelectElement>(null)
	const pageSizeRef = useRef<HTMLSelectElement>(null)

	useEffect(() => {
		getAllRecords()
	}, [scoreboardQuery])

	const getAllRecords = async () => {
		try {
			const result = await scoreboardService.getScores(scoreboardQuery)
			setPagedResult(result)
			setRecords(result.items)
		} catch (error) {
			console.error('Error while fetching scores:', error)
		}
	}

	const handleFindClick = () => {
		const nickname = nicknameRef.current?.value || ''
		const difficulty = difficultyRef.current?.value || 'any'
		const pageSize = pageSizeRef.current?.value || '1'

		setScoreboardQuery({
			...scoreboardQuery,
			searchNickname: nickname,
			difficultyLevel: difficulty as 'easy' | 'normal' | 'hard' | 'any',
			pageNumber: 1,
			pageSize: parseInt(pageSize),
		})
	}

	const changePage = (pageNumber: number) => {
		if (pageNumber !== scoreboardQuery.pageNumber) {
			setScoreboardQuery((prev) => ({
				...prev,
				pageNumber,
			}))
		}
	}

	const showRecord = (record: UserStats) => (
		<tr
			key={record.guid || record.nickname}
			className={styles.record}
			onClick={() => navigateToUserProfile(record.guid)}
		>
			<td>{record.guid}</td>
			<td>{record.nickname}</td>
			<td>{record.gamePlayed}</td>
			<td>{Number(record.averageScore.toFixed(4))}</td>
		</tr>
	)

	const navigateToUserProfile = (userId: string) => {
		navigate(`/user/${userId}`)
	}

	return (
		<>
			<Header />
			<div className={styles.page}>
				<div className={styles.container}>
					<h1 className={styles.header}>User Scoreboard</h1>

					<div className={styles.filters_container}>
						<div className={styles.formBox}>
							<div className={styles.inputField}>
								<label htmlFor="nickname">Nickname:</label>
								<input
									type="text"
									id="nickname"
									name="nickname"
									placeholder="Enter nickname"
									ref={nicknameRef}
								/>
							</div>

							<div className={styles.inputField}>
								<label htmlFor="difficulty">Difficulty:</label>
								<select id="difficulty" name="difficulty" ref={difficultyRef}>
									<option value="easy">Easy</option>
									<option value="normal">Normal</option>
									<option value="hard">Hard</option>
								</select>
							</div>

							<div className={styles.inputField}>
								<label htmlFor="pageSize">Page size:</label>
								<select
									className={styles.pageSizeSelect}
									id="pageSize"
									name="pageSize"
									ref={pageSizeRef}
									defaultValue="3"
								>
									<option value="1">1</option>
									<option value="3">3</option>
									<option value="5">5</option>
								</select>
							</div>
						</div>

						<button className={styles.applyButton} onClick={handleFindClick}>
							Apply Filters
						</button>
					</div>

					<div className={styles.table_container}>
						<table className={styles.table}>
							<thead>
								<tr>
									<th>User Id</th>
									<th>Nickname</th>
									<th>Games Played</th>
									<th>Average Score</th>
								</tr>
							</thead>
							<tbody>
								{records.length > 0 ? (
									records.map(showRecord)
								) : (
									<tr>
										<td colSpan={4} className={styles.empty_state}>
											<div className={styles.empty_content}>
												<p className={styles.empty_icon}>🏆</p>
												<h3>Brak wyników</h3>
												<p>Nie znaleziono żadnych rekordów spełniających kryteria.</p>
											</div>
										</td>
									</tr>
								)}
							</tbody>
						</table>
					</div>

					<PaginationButtons
						totalPages={pagedResult.totalPages}
						currentPage={scoreboardQuery.pageNumber}
						onChangePage={changePage}
					/>

					<button className={styles.backButton} onClick={() => navigate(MENU_ROUTE)}>
						Back to menu
					</button>
				</div>
			</div>
		</>
	)
}

export default Scoreboard
