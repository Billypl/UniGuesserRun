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
import hamburger_styles from '../styles/UserMenu.module.scss'
import placesIcon from '../assets/images/places.png';
import addPlaceIcon from '../assets/images/addplace.png';
import loginIcon from '../assets/images/login.png';
import logoutIcon from '../assets/images/logout.png';
import registerIcon from '../assets/images/register.png';
import placeQueueIcon from '../assets/images/placequeue.png';
import profileIcon from '../assets/images/profile.png';

const Account: React.FC = () => {
	const navigate = useNavigate()

	const handleLogout = () => {
		accountService.logout()
		
		navigate(MENU_ROUTE)
	}

	const displayGuestContent = () => {
		return (
			<div className={styles.account}>
				<a className={hamburger_styles.menu_option} onClick={() => navigate(LOGIN_ROUTE)}>
					<img src={loginIcon} alt="Login icon" className={hamburger_styles.icon} />
					Login
				</a>
				<a className={hamburger_styles.menu_option} onClick={() => navigate(REGISTER_ROUTE)}>
					<img src={registerIcon} alt="Register icon" className={hamburger_styles.icon} />
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
					<a className={hamburger_styles.menu_option} onClick={() => navigate(PLACES_ROUTE)}>
						<img src={placesIcon} alt="Places icon" className={hamburger_styles.icon} />
						Places
					</a>
				)}
				
				{(role === 'Admin' || role === 'Moderator') && (
					<a className={hamburger_styles.menu_option} onClick={() => navigate(PLACE_QUEUE_ROUTE)}>
						<img src={placeQueueIcon} alt="Place queue icon" className={hamburger_styles.icon} />
						Place queue
					</a>
				)}

				<a className={hamburger_styles.menu_option} onClick={() => navigate(ADD_PLACE_ROUTE)}>
					<img src={addPlaceIcon} alt="Add place icon" className={hamburger_styles.icon} />
					Add place
				</a>

				<a className={hamburger_styles.menu_option} onClick={() => navigate(`${USER_ROUTE}/${accountService.getCurrentUser()?.userId}`)}>
					<img src={profileIcon} alt="Profile icon" className={hamburger_styles.icon} />
					{accountService.getCurrentUser()?.nickname}
				</a>

				<a className={`${styles.logout} ${hamburger_styles.menu_option} `} onClick={handleLogout}>
					<img src={logoutIcon} alt="Logout icon" className={hamburger_styles.icon} />
					Logout
				</a>
			</div>
		)
	}

	return accountService.isLoggedIn() ? displayUserContent() : displayGuestContent()
}

export default Account
