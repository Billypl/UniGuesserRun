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
import menu_styles from '../styles/UserMenu.module.scss'
import placesIcon from '../assets/images/places.png'
import addPlaceIcon from '../assets/images/addplace.png'
import loginIcon from '../assets/images/login.png'
import logoutIcon from '../assets/images/logout.png'
import registerIcon from '../assets/images/register.png'
import placeQueueIcon from '../assets/images/placequeue.png'
import profileIcon from '../assets/images/profile.png'

const Account: React.FC = () => {
	const navigate = useNavigate()

	const handleLogout = () => {
		accountService.logout()
		navigate(MENU_ROUTE)
	}

	const displayGuestContent = () => {
		return (
			<div className={styles.account}>
				<div className={styles.separator_before}></div>
				<a className={menu_styles.menu_option} onClick={() => navigate(LOGIN_ROUTE)}>
					<img src={loginIcon} alt="Login" className={menu_styles.icon} />
					<span>Login</span>
				</a>
				<div className={styles.separator_item}></div>
				<a className={menu_styles.menu_option} onClick={() => navigate(REGISTER_ROUTE)}>
					<img src={registerIcon} alt="Register" className={menu_styles.icon} />
					<span>Register</span>
				</a>
			</div>
		)
	}

	const displayUserContent = () => {
		const role = accountService.getCurrentUser()?.role
		return (
			<div className={styles.account}>
				<div className={styles.separator_before}></div>
				{role === 'Admin' && (
					<>
						<a className={menu_styles.menu_option} onClick={() => navigate(PLACES_ROUTE)}>
							<img src={placesIcon} alt="Places" className={menu_styles.icon} />
							<span>Places</span>
						</a>
						<div className={styles.separator_item}></div>
					</>
				)}

				{(role === 'Admin' || role === 'Moderator') && (
					<>
						<a className={menu_styles.menu_option} onClick={() => navigate(PLACE_QUEUE_ROUTE)}>
							<img src={placeQueueIcon} alt="Place queue" className={menu_styles.icon} />
							<span>Place queue</span>
						</a>
						<div className={styles.separator_item}></div>
					</>
				)}

				<a className={menu_styles.menu_option} onClick={() => navigate(ADD_PLACE_ROUTE)}>
					<img src={addPlaceIcon} alt="Add place" className={menu_styles.icon} />
					<span>Add place</span>
				</a>
				<div className={styles.separator_item}></div>

				<a
					className={menu_styles.menu_option}
					onClick={() => navigate(`${USER_ROUTE}/${accountService.getCurrentUser()?.userId}`)}
				>
					<img src={profileIcon} alt="Profile" className={menu_styles.icon} />
					<span>{accountService.getCurrentUser()?.nickname}</span>
				</a>
				<div className={styles.separator_item}></div>

				<a className={`${styles.logout} ${menu_styles.menu_option}`} onClick={handleLogout}>
					<img src={logoutIcon} alt="Logout" className={menu_styles.icon} />
					<span>Logout</span>
				</a>
			</div>
		)
	}

	return accountService.isLoggedIn() ? displayUserContent() : displayGuestContent()
}

export default Account
