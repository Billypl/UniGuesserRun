import React, { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import Header from '../components/Header'
import styles from '../styles/User.module.scss'
import accountService from '../services/api/accountService'
import { AccountDetailsDto } from '../models/account/AccountDetailsDto'
import gameSessionService from '../services/api/gameSessionService'
import { SortDirection } from '../models/scoreboard/SortDirection'
import { PagedResult } from '../models/scoreboard/PagedResult'
import { FinishedGameDto } from '../models/game/FinishedGameDto'

const User: React.FC = () => {
	const navigate = useNavigate()

	const { id } = useParams<{ id: string }>()
	const [accountDetails, setAccountDetails] = useState<AccountDetailsDto | null>(null)
	const [gamesHistory, setGamesHistory] = useState<PagedResult<FinishedGameDto> | null>(null)

	useEffect(() => {
		if (!id) {
			navigate('/')
			return
		}
		fetchAccountDetails(id)
		fetchHistoryPage(id)
	}, [])

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
			const history = await gameSessionService.getHistoryPagesByUser(userId, {
				difficultyLevel: null,
				pageNumber: 1,
				pageSize: 5,
				sortDirection: SortDirection.DESC,
			})
			setGamesHistory(history)
		} catch (error) {
			console.error('Error fetching game history:', error)
		}
	}

	return (
		<>
			<div className={styles.background}></div>
			<Header />
			{accountDetails && gamesHistory && (
				<div className={styles.profile_container}>
					<div className={styles.account_info}>
						<h2>{accountDetails.nickname}</h2>
						<p>Email: {accountDetails.email}</p>
						<p>Role: {accountDetails.role}</p>
						<p>Joined on: {new Date(accountDetails.createdAt).toLocaleDateString()}</p>

						<h3>Games played:</h3>
						<table className={styles.games_table}>
							<thead>
								<tr>
									<th>Difficulty</th>
									<th>Score</th>
								</tr>
							</thead>
							<tbody>
								{gamesHistory.items.map((game) => (
									<tr key={game.id}>
										<td>{game.difficulty.toUpperCase()}</td>
										<td>{game.finalScore.toFixed(0)}</td>
									</tr>
								))}
							</tbody>
						</table>

						<button onClick={() => navigate('/')}>Back to menu</button>
					</div>
				</div>
			)}
		</>
	)
}

export default User
