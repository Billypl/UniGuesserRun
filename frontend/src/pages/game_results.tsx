import React from 'react'
import { useNavigate } from 'react-router-dom'
import { useGameContext } from '../hooks/useGameContext'
import { MAP_CENTER, MENU_ROUTE, SELECTED_DIFFICULTY_KEY } from '../Constants'
import styles from '../styles/GameResults.module.scss'
import accountService from '../services/api/accountService'
import { MapContainer, TileLayer } from 'react-leaflet'
import { LocationMarker } from '../components/LocationMarker'
import { TargetMarker } from '../components/TargetMarker'
import { ClickedIcon, TargetIcon } from '../components/MarkerIcons'

const GameResults: React.FC = () => {
	const navigate = useNavigate()
	const { finishedGameData } = useGameContext()

	const returnToMenu = () => {
		navigate(MENU_ROUTE)
	}

	const getMapBounds = (): [[number, number], [number, number]] => {
		if (!finishedGameData)
			return [
				[0, 0],
				[0, 0],
			]

		// clicked positions
		const clickedMaxLat: number =
			finishedGameData?.rounds.reduce((max, round) => {
				return Math.max(max, round.latitude)
			}, -Infinity) || 0
		const clickedMinLat: number =
			finishedGameData?.rounds.reduce((min, round) => {
				return Math.min(min, round.latitude)
			}, Infinity) || 0
		const clickedMaxLng: number =
			finishedGameData?.rounds.reduce((max, round) => {
				return Math.max(max, round.longitude)
			}, -Infinity) || 0
		const clickedMinLng: number =
			finishedGameData?.rounds.reduce((min, round) => {
				return Math.min(min, round.longitude)
			}, Infinity) || 0

		// target positions
		const targetMaxLat: number =
			finishedGameData?.rounds.reduce((max, round) => {
				return Math.max(max, round.placeToGuess.latitude)
			}, -Infinity) || 0
		const targetMinLat: number =
			finishedGameData?.rounds.reduce((min, round) => {
				return Math.min(min, round.placeToGuess.latitude)
			}, Infinity) || 0
		const targetMaxLng: number =
			finishedGameData?.rounds.reduce((max, round) => {
				return Math.max(max, round.placeToGuess.longitude)
			}, -Infinity) || 0
		const targetMinLng: number =
			finishedGameData?.rounds.reduce((min, round) => {
				return Math.min(min, round.placeToGuess.longitude)
			}, Infinity) || 0

		const maxLat = Math.max(clickedMaxLat, targetMaxLat)
		const minLat = Math.min(clickedMinLat, targetMinLat)
		const maxLng = Math.max(clickedMaxLng, targetMaxLng)
		const minLng = Math.min(clickedMinLng, targetMinLng)

		return [
			[minLat, minLng],
			[maxLat, maxLng],
		]
	}

	const showRoundsTargets = () => {
		return finishedGameData?.rounds.map((round, index) => {
			const clickedCoords = { latitude: round.latitude, longitude: round.longitude }
			const targetCoords = {
				latitude: round.placeToGuess.latitude,
				longitude: round.placeToGuess.longitude,
			}
			return (
				<>
					<LocationMarker coords={clickedCoords} icon={ClickedIcon} label="Clicked location:" />
					<TargetMarker
						key={index}
						clickedLatLng={clickedCoords}
						targetLatLng={targetCoords}
						icon={TargetIcon}
					/>
				</>
			)
		})
	}

	const showRoundsTable = () => {
		return (
			<table>
				<thead>
					<tr>
						<th>Round</th>
						<th>Score</th>
					</tr>
				</thead>
				<tbody>
					{finishedGameData?.rounds.map((round, index) => {
						return (
							<tr key={index}>
								<td>{index + 1}</td>
								<td>{round.score.toFixed(2)}</td>
							</tr>
						)
					})}
				</tbody>
			</table>
		)
	}

	return (
		<div className={styles.results_container}>
			<div className={styles.map_container}>
				<MapContainer
					center={finishedGameData?.rounds ? undefined : MAP_CENTER} // default map center
					bounds={finishedGameData?.rounds ? getMapBounds() : undefined}
					zoom={13}
					scrollWheelZoom={true}
					style={{ height: '100%', width: '100%' }}
				>
					<TileLayer
						attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
						url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
					/>

					{showRoundsTargets()}
				</MapContainer>
			</div>
			<div className={styles.summary_container}>
				{!finishedGameData ? (
					<div>Loading...</div>
				) : (
					<>
						<h2>Congratulations {accountService.getCurrentUser()?.nickname}!</h2>
						<p>
							Your score: <b>{finishedGameData?.finalScore.toFixed(2)}</b>
						</p>
						<p>
							On <b>{finishedGameData?.difficulty}</b> difficulty
						</p>

						{showRoundsTable()}
					</>
				)}

				<button onClick={returnToMenu}>Back to menu</button>
			</div>
		</div>
	)
}

export default GameResults
