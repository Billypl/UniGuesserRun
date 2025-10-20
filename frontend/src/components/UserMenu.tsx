import React, { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import Account from './Account'
import styles from '../styles/UserMenu.module.scss'
import { SCOREBOARD_ROUTE } from '../Constants'
import scoreboardIcon from '../assets/images/scoreboard.png'

const UserMenu = () => {
	const [isOpen, setIsOpen] = useState(false)
	const navigate = useNavigate()

	const toggleMenu = () => {
		setIsOpen(!isOpen)
	}

	const handleNavigation = (route: string) => {
		navigate(route)
		setIsOpen(false)
	}

	return (
		<div className={styles.user_menu_container}>
			{/* Burger menu button - visible only on mobile */}
			<button
				className={styles.hamburger_toggle}
				onClick={toggleMenu}
				aria-expanded={isOpen}
				aria-label="Toggle navigation menu"
			>
				<span className={isOpen ? styles.bar_open : styles.bar}></span>
				<span className={isOpen ? styles.bar_open : styles.bar}></span>
				<span className={isOpen ? styles.bar_open : styles.bar}></span>
			</button>

			{/* Desktop navigation - always visible on desktop */}
			<nav className={styles.desktop_nav}>
				<button className={styles.nav_button} onClick={() => handleNavigation(SCOREBOARD_ROUTE)}>
					<img src={scoreboardIcon} alt="Scoreboard" className={styles.icon} />
					<span>Scoreboard</span>
				</button>
				<div className={styles.separator}></div>
				<div className={styles.account_section}>
					<Account />
				</div>
			</nav>

			{/* Mobile dropdown menu */}
			{isOpen && (
				<div className={styles.mobile_menu}>
					<button className={styles.menu_option} onClick={() => handleNavigation(SCOREBOARD_ROUTE)}>
						<img src={scoreboardIcon} alt="Scoreboard" className={styles.icon} />
						Scoreboard
					</button>
					<Account />
				</div>
			)}
		</div>
	)
}

export default UserMenu
