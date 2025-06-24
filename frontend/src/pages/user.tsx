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
					<p>Average score: 1320,23</p>
					<h3>Games played:</h3>
					<table className={styles.games_table}>
            <thead>
							<tr>
								<th>Date</th>
								<th>Score</th>
							</tr>
            </thead>
						<tbody>
							{/* {gamesPlayed.map((game) => (
								<tr key={game.id}>
									<td>{game.date}</td>
									<td>{game.score}</td>
								</tr>
							))} */}
							<tr>
								<td>24.06.2025 16:42</td>
								<td>2137,69</td>
							</tr>
							<tr>
								<td>22.06.2025 13:37</td>
								<td>420,37</td>
							</tr>
							<tr>
								<td>20.04.2025 4:32</td>
								<td>1519,12</td>
							</tr>
						</tbody>
					</table>
				</div>
			</div>
		</>
	)
}

export default User
