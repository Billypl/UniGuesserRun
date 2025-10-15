import React from 'react'
import { useNavigate } from 'react-router-dom'
import Header from '../components/Header'
import { GAME_SETTINGS_ROUTE, GAME_TOKEN_KEY, SCOREBOARD_ROUTE } from '../Constants'
import styles from '../styles/Menu.module.scss'

const Menu: React.FC = () => {
	const navigate = useNavigate()

	const openGameSettings = () => {
		window.sessionStorage.removeItem(GAME_TOKEN_KEY)
		navigate(GAME_SETTINGS_ROUTE)
	}

	return (
		<>
			<div className={styles.background}></div>
			<Header />

			{/* Floating decorative elements */}
			<div className={styles.decorations}>
				<div className={`${styles.decoration} ${styles.decoration1}`}>📍</div>
				<div className={`${styles.decoration} ${styles.decoration2}`}>🗺️</div>
				<div className={`${styles.decoration} ${styles.decoration3}`}>🎓</div>
				<div className={`${styles.decoration} ${styles.decoration4}`}>📌</div>
			</div>

			<div className={styles.main_menu}>
				<div className={styles.title_container}>
					<div className={styles.catchy_phrase}>
						<span className={styles.highlight}>CZY ZNASZ TERENY</span>
						<br />
						<span className={styles.highlight}>SWOJEJ UCZELNI?</span>
						<div className={styles.subtitle}>Sprawdź i się przekonaj!</div>
					</div>
				</div>

				<div className={styles.options}>
					<button className={`${styles.menu_option} ${styles.primary}`} onClick={openGameSettings}>
						<span className={styles.button_icon}>▶</span>
						<span className={styles.button_text}>Zagraj teraz</span>
					</button>
					<button
						className={`${styles.menu_option} ${styles.secondary}`}
						onClick={() => navigate(SCOREBOARD_ROUTE)}
					>
						<span className={styles.button_icon}>★</span>
						<span className={styles.button_text}>Ranking</span>
					</button>
				</div>
			</div>
		</>
	)
}

export default Menu
