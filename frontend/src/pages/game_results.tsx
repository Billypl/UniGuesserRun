import React, { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { MENU_ROUTE, USER_ROUTE } from '../Constants'
import styles from '../styles/GameResults.module.scss'
import GameSummaryMap from '../components/GameSummaryMap'
import GameSummaryTable from '../components/GameSummaryTable'
import { FinishedGameDto } from '../models/game/FinishedGameDto'
import gameSessionService from '../services/api/gameSessionService'

const GameResults: React.FC = () => {
	const navigate = useNavigate()
	// userId is optional, if present we came from user profile
	// if not, we came straight from the game
	const { userId, gameId } = useParams<{ userId?: string; gameId?: string }>()
	const [finishedGameData, setFinishedGameData] = useState<FinishedGameDto | null>(null)

	useEffect(() => {
		if (!gameId) {
			navigate('/')
			return
		}
		fetchFinishedGameData()
	}, [])

	const fetchFinishedGameData = async () => {
		try {
			console.log('Fetching game results for gameId:', gameId)
			const response = await gameSessionService.getResultDetails(gameId!)
			setFinishedGameData(response)
		} catch (error) {
			console.error('Error fetching game results:', error)
		}
	}

	const returnToMenu = () => {
		if (!userId) {
			navigate(MENU_ROUTE)
			return
		}
		navigate(`${USER_ROUTE}/${userId}`)
	}

	return (
		<div className={styles.results_container}>
			<div className={styles.map_container}>
				{finishedGameData && <GameSummaryMap finishedGameData={finishedGameData} />}
			</div>
			<div className={styles.summary_container}>
				{!finishedGameData ? (
					<div>Loading...</div>
				) : (
					<>
						<h2>Game Results:</h2>
						<p>
							Score: <b>{finishedGameData?.finalScore.toFixed(2)}</b>
						</p>
						<p>
							On <b>{finishedGameData?.difficulty}</b> difficulty
						</p>

						<GameSummaryTable finishedGameData={finishedGameData} />
					</>
				)}

				<button onClick={returnToMenu}>Back to {userId ? 'profile' : 'menu'}</button>
			</div>
		</div>
	)
}

export default GameResults
