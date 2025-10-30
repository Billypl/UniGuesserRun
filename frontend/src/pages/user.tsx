import React, { use, useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import Header from '../components/Header'
import styles from '../styles/User.module.scss'
import accountService from '../services/api/accountService'
import { AccountDetailsDto } from '../models/account/AccountDetailsDto'
import gameSessionService from '../services/api/gameSessionService'
import { SortDirection } from '../models/scoreboard/SortDirection'
import { PagedResult } from '../models/scoreboard/PagedResult'
import { FinishedGameDto } from '../models/game/FinishedGameDto'
import { ReactComponent as CrownIcon } from '../assets/images/crown.svg'
import { ReactComponent as ShieldIcon } from '../assets/images/shield.svg'
import { GAME_RESULTS_ROUTE, USER_ROUTE } from '../Constants'
import { UserHistoryQuery } from '../models/game/UserHistoryQuery'
import PaginationButtons from '../components/PaginationButtons'

const User: React.FC = () => {
	const navigate = useNavigate()

	const { userId } = useParams<{ userId: string }>()
	const [accountDetails, setAccountDetails] = useState<AccountDetailsDto | null>(null)
	const [userHistoryQuery, setUserHistoryQuery] = useState<UserHistoryQuery>({
		difficultyLevel: null,
		pageNumber: 1,
		pageSize: 5,
		sortDirection: SortDirection.DESC,
	})
	const [gamesHistory, setGamesHistory] = useState<PagedResult<FinishedGameDto>>({
		items: [],
		totalPages: 0,
		itemFrom: 0,
		itemsTo: 0,
		totalItemsCount: 0,
	})

	useEffect(() => {
		if (!userId) {
			navigate('/')
			return
		}
		fetchAccountDetails(userId)
		fetchHistoryPage(userId)
	}, [])

	useEffect(() => {
		if (userId) {
			fetchHistoryPage(userId)
		}
	}, [userHistoryQuery])

	const fetchAccountDetails = async (userId: string) => {
		try {
			const details = await accountService.getAccountDetails(userId)
			setAccountDetails(details)
		} catch (error: any) {
			if (error.response?.status === 401) {
				navigate('/login')
			} else {
				console.error('Error fetching account details:', error)
			}
		}
	}

	const fetchHistoryPage = async (userId: string) => {
		try {
			const history = await gameSessionService.getHistoryPagesByUser(userId, userHistoryQuery)
			setGamesHistory(history)
			console.log('Fetched game history:', history)
		} catch (error) {
			console.error('Error fetching game history:', error)
		}
	}

	const changeHistoryPage = (pageNumber: number) => {
		console.log('Changing to page: ' + pageNumber)
		if (pageNumber !== userHistoryQuery.pageNumber) {
			setUserHistoryQuery((prev) => ({
				...prev,
				pageNumber,
			}))
		}
	}

	const showRoleIcon = () => {
		if (accountDetails === null) {
			return null
		}
		if (accountDetails.role.toLowerCase() === 'admin') {
			return <CrownIcon title="Admin" />
		}
		if (accountDetails.role.toLowerCase() === 'moderator') {
			return <ShieldIcon title="Moderator" />
		}
		return null
	}

	const showGameHistory = () => {
		return (
			<>
				<p className={styles.games_history_title}>Recent games:</p>
				<table className={styles.games_table}>
					<thead>
						<tr>
							<th>Date</th>
							<th>Difficulty</th>
							<th>Score</th>
							<th>Game mode</th>
							<th>Status</th>
						</tr>
					</thead>
					<tbody>
						{gamesHistory.items.length === 0 ? (
							<tr>
								<td colSpan={3} className={styles.empty_state}>
									<div className={styles.empty_content}>
										<p className={styles.empty_icon}>🎮</p>
										<h4>No games</h4>
										<p>No games have been played yet.</p>
									</div>
								</td>
							</tr>
						) : (
							gamesHistory.items.map((game) => (
								<tr
									key={game.id}
									onClick={() => navigateToResults(game.id)}
									className={styles.clickable_row}
								>
									<td>{new Date().toLocaleDateString()}</td>
									<td>{game.difficulty.toUpperCase()}</td>
									<td>{game.finalScore.toFixed(0)}</td>
									<td>{game.gameMode}</td>
									<td>{game.gameState}</td>
								</tr>
							))
						)}
					</tbody>
				</table>
				<PaginationButtons
					totalPages={gamesHistory.totalPages}
					currentPage={userHistoryQuery.pageNumber}
					onChangePage={changeHistoryPage}
				/>
			</>
		)
	}

	const navigateToResults = (gameId: string) => {
		navigate(`${USER_ROUTE}/${userId}/${GAME_RESULTS_ROUTE}/${gameId}`)
	}

	return (
		<>
			<div className={styles.background}></div>
			<Header />
			{accountDetails && gamesHistory && (
				<div className={styles.profile_container}>
					<div className={styles.account_info}>
						<h2 className={styles.nickname}>
							{accountDetails.nickname}
							{showRoleIcon()}
						</h2>
						<p className={styles.details}>
							{accountDetails.email} | Joined on:{' '}
							{new Date(accountDetails.createdAt).toLocaleDateString()}
						</p>
						{showGameHistory()}
						<button onClick={() => navigate('/')}>Back to menu</button>
					</div>
				</div>
			)}
		</>
	)
}

export default User
