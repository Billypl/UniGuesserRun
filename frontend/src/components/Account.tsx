import React from 'react'
import { useNavigate } from 'react-router-dom'
import accountService from '../services/api/accountService'
import {
	ADD_PLACE_ROUTE,
	LOGIN_ROUTE,
	MENU_ROUTE,
	PLACE_QUEUE_ROUTE,
	PLACES_ROUTE,
	REGISTER_ROUTE,
	USER_ROUTE,
} from '../Constants'
import styles from '../styles/Account.module.scss'

const Account: React.FC = () => {
	const navigate = useNavigate()

	const handleLogout = () => {
		accountService.logout()
		navigate(MENU_ROUTE)
	}

	const displayGuestContent = () => {
		return (
			<div className={styles.account}>
				<a className={styles.nav_item} onClick={() => navigate(LOGIN_ROUTE)}>
					Login
				</a>
				<a className={styles.nav_item} onClick={() => navigate(REGISTER_ROUTE)}>
					Register
				</a>
			</div>
		)
	}

	const displayUserContent = () => {
		const role = accountService.getCurrentUser()?.role
		return (
			<div className={styles.account}>
				{role === 'Admin' && (
					<a className={styles.nav_item} onClick={() => navigate(PLACES_ROUTE)}>
						Places
					</a>
				)}
				
				{(role === 'Admin' || role === 'Moderator') && (
					<a className={styles.nav_item} onClick={() => navigate(PLACE_QUEUE_ROUTE)}>
						Place queue
					</a>
				)}

				<a className={styles.nav_item} onClick={() => navigate(ADD_PLACE_ROUTE)}>
					Add place
				</a>

				<a className={styles.nav_item} onClick={() => navigate(`${USER_ROUTE}/${accountService.getCurrentUser()?.userId}`)}>
					{accountService.getCurrentUser()?.nickname}
				</a>
				<a className={`${styles.logout} ${styles.nav_item} `} onClick={handleLogout}>
					Logout
				</a>
			</div>
		)
	}

	return accountService.isLoggedIn() ? displayUserContent() : displayGuestContent()
}

export default Account
