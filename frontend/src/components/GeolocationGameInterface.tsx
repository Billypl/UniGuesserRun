import React, { useEffect, useState } from 'react'
import { MapContainer, TileLayer } from 'react-leaflet'
import 'leaflet/dist/leaflet.css'

import { LocationMarker } from './LocationMarker'
import { TargetIcon, ClickedIcon } from './MarkerIcons'
import { TargetMarker } from './TargetMarker'
import { RecenterMap } from './RecenterMap'

import styles from '../styles/GameInterface.module.scss'
import { MAP_CENTER } from '../Constants'
import { Coordinates } from '../models/Coordinates'

import ExitIcon from '../assets/images/x-lg.svg?react'

interface GeolocationGameInterfaceProps {
	error: string | null
	currentRoundNumber: number
	isLastRound: boolean
	imageUrl: string
	guessDistance: number | null
	targetLatLng: Coordinates | null
	onConfirmPlayerChoice: (latlng: Coordinates) => void
	onNextRound: () => void
	onFinishGame: () => void
}

const GeolocationGameInterface: React.FC<GeolocationGameInterfaceProps> = (props) => {
	const [playerPosition, setPlayerPosition] = useState<Coordinates | null>(null)
	const [isChoiceConfirmed, setIsChoiceConfirmed] = useState<boolean>(false)
	const [isImageFullScreen, setIsImageFullScreen] = useState<boolean>(false)

	useEffect(() => {
		// Always use geolocation in this component
		forceUpdateGeolocation()
		const watchId = watchGeolocation()
		return () => {
			if (watchId) {
				navigator.geolocation.clearWatch(watchId)
			}
		}
	}, [])

	const selectLocation = (coords: Coordinates | null) => {
		if (isChoiceConfirmed) return // can't move the marker after confirming your choice
		setPlayerPosition(coords)
	}

	const watchGeolocation = (): number | null => {
		if (!('geolocation' in navigator)) {
			console.error('Geolocation is not supported by your browser.')
			return null
		}
		return navigator.geolocation.watchPosition(
			(position) => {
				const coords: Coordinates = {
					latitude: position.coords.latitude,
					longitude: position.coords.longitude,
				}
				selectLocation(coords)
			},
			(error) => {
				console.error('Unable to retrieve location. Please enable location services.', error)
			},
			{
				enableHighAccuracy: true,
				maximumAge: 3000,
				timeout: 5000,
			}
		)
	}

	const forceUpdateGeolocation = () => {
		if (!('geolocation' in navigator)) {
			console.error('Geolocation is not supported by your browser.')
			return
		}

		navigator.geolocation.getCurrentPosition(
			(position) => {
				const coords: Coordinates = {
					latitude: position.coords.latitude,
					longitude: position.coords.longitude,
				}
				selectLocation(coords)
			},
			(error) => {
				console.error('Unable to retrieve location. Please enable location services.', error)
			},
			{
				enableHighAccuracy: true,
				maximumAge: 3000,
				timeout: 5000,
			}
		)
	}

	const confirmPlayerChoice = () => {
		setIsChoiceConfirmed(true)
		props.onConfirmPlayerChoice(playerPosition!)
	}

	const endRoundButton = () => {
		return props.isLastRound ? (
			<button className={styles.end_round_button} onClick={props.onFinishGame}>
				Finish game
			</button>
		) : (
			<button className={styles.end_round_button} onClick={nextRound}>
				Next round
			</button>
		)
	}

	const nextRound = () => {
		setIsChoiceConfirmed(false)
		setPlayerPosition(null)
		// In geolocation mode, update position for next round
		forceUpdateGeolocation()
		props.onNextRound()
	}

	return (
		<div className={styles.game_interface}>
			{!isImageFullScreen && (
				<div className={styles.game_header}>
					<h1>Round {props.currentRoundNumber + 1}</h1>
				</div>
			)}

			<div
				className={`${styles.image_container} ${isImageFullScreen ? styles.fullscreen : ''}`}
				onClick={() => setIsImageFullScreen(!isImageFullScreen)}
			>
				<img src={props.imageUrl!} alt="Round location" />
				{isImageFullScreen && (
					<div className={styles.fullscreen_exit}>
						<ExitIcon />
					</div>
				)}
			</div>

			{props.error && <p style={{ color: 'red' }}>{props.error}</p>}

			{playerPosition && !isChoiceConfirmed && !isImageFullScreen && (
				<button className={styles.confirm_button} onClick={confirmPlayerChoice}>
					Confirm
				</button>
			)}

			{playerPosition && isChoiceConfirmed && props.guessDistance && (
				<div className={styles.round_result}>
					<h1 className={styles.distance}>Guess distance: {props.guessDistance.toFixed(2)}</h1>
					{endRoundButton()}
				</div>
			)}

			{/* Map Section */}
			<div className={styles.map_container}>
				<MapContainer
					center={MAP_CENTER}
					zoom={13}
					scrollWheelZoom={true}
					style={{ height: '100%', width: '100%' }}
				>
					<TileLayer
						attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
						url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
					/>
					{playerPosition && (
						<>
							<LocationMarker coords={playerPosition} icon={ClickedIcon} label="Your location:" />
							<RecenterMap location={[playerPosition.latitude, playerPosition.longitude]} />
						</>
					)}{' '}
					{isChoiceConfirmed && (
						<TargetMarker
							clickedLatLng={playerPosition}
							targetLatLng={props.targetLatLng}
							icon={TargetIcon}
						/>
					)}
					{/* No manual location selection in GEOLOCATION mode - position is tracked automatically */}
				</MapContainer>
			</div>
		</div>
	)
}

export default GeolocationGameInterface
