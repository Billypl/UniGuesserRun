import React from 'react'
import { useNavigate } from 'react-router-dom'
import Header from '../components/Header'
import styles from '../styles/User.module.scss'
import accountService from '../services/api/accountService'

const User: React.FC = () => {
	const navigate = useNavigate()

	return (
		<>
			<div className={styles.background}></div>
			<Header />
			<div className={styles.profile_container}>
				<div className={styles.account_info}>
					<h2>{accountService.getCurrentUser()?.nickname}</h2>
					<p>Email: {accountService.getCurrentUser()?.email}</p>
					<p>Role: {accountService.getCurrentUser()?.role}</p>
				</div>
			</div>
		</>
	)
}

export default User
