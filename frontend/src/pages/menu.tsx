import React from 'react'
import { useNavigate } from 'react-router-dom'
import { useGameContext } from '../hooks/useGameContext'
import Header from '../components/Header'
import { GAME_ROUTE, GAME_TOKEN_KEY, LOGIN_ROUTE, REGISTER_ROUTE, SCOREBOARD_ROUTE } from '../Constants'
import styles from '../styles/Menu.module.scss'
import accountService from '../services/api/accountService'
import gameService from '../services/api/gameService'

const Menu: React.FC = () => {
	const navigate = useNavigate()

	const startGame = async () => {
		if (!accountService.isLoggedIn()) {
			return
		}
		await gameService.setUpGameTokenIfUserHasGame()

		navigate(GAME_ROUTE)
	}

	return (
		<>
			<div className={styles.background}></div>
			<Header />

			<div className={styles.main_menu}>
				<div className={styles.catchy_phrase}>
					CZY ZNASZ TERENY SWOJEJ UCZELNI? <br />
					Sprawdź i się przekonaj!
				</div>
				<div className={styles.options}>
					{accountService.isLoggedIn() ? (
						<a className={styles.menu_option} onClick={startGame}>
							Start game
						</a>
					) : (
						<a className={styles.menu_option} onClick={() => navigate(LOGIN_ROUTE)}>
							Login to play
						</a>
					)}

					<a className={styles.menu_option} onClick={() => navigate(SCOREBOARD_ROUTE)}>
						Scoreboard
					</a>
				</div>
			</div>
		</>
	)
}

export default Menu
