import React, { useEffect, useState } from 'react'
import { MapContainer, TileLayer } from 'react-leaflet'
import 'leaflet/dist/leaflet.css'

import { SelectMapLocation } from './SelectMapLocation'
import { LocationMarker } from './LocationMarker'

import { TargetIcon, ClickedIcon } from './MarkerIcons'
import { TargetMarker } from './TargetMarker'

import styles from '../styles/GameInterface.module.scss'
import { MAP_CENTER } from '../Constants'
import { Coordinates } from '../models/Coordinates'

import { ReactComponent as ExitIcon } from '../assets/images/x-lg.svg'
import { GameMode } from '../models/game/GameMode'

interface GameInterfaceProps {
	error: string | null
	gameMode: GameMode
	currentRoundNumber: number
	isLastRound: boolean
	imageUrl: string
	guessDistance: number | null
	targetLatLng: Coordinates | null
	onConfirmPlayerChoice: (latlng: Coordinates) => void
	onNextRound: () => void
	onFinishGame: () => void
}

const GameInterface: React.FC<GameInterfaceProps> = (props) => {
	const [clickedLatLng, setClickedLatLng] = useState<Coordinates | null>(null)
	const [playerChoiceConfirmed, setPlayerChoiceConfirmed] = useState<boolean>(false)

	const [fullScreenImage, setFullScreenImage] = useState<boolean>(false)
	const [zoomClass, setZoomClass] = useState<string>('')

	useEffect(() => {
		console.log(props)

		if (props.gameMode === GameMode.GEOLOCATION) {
			forceUpdateGeolocation()
			const watchId = watchGeolocation()
			return () => {
				if (watchId) {
					navigator.geolocation.clearWatch(watchId)
				}
			}
		}
	}, [props.gameMode])

	const selectLocation = (coords: Coordinates | null) => {
		if (playerChoiceConfirmed) return // cant move the marker after confirming your choice
		setClickedLatLng(coords)
	}

	const watchGeolocation = (): number | null => {
		if (!('geolocation' in navigator)) {
			console.error('Geolocation is not supported by your browser.')
			return null
		}
		return navigator.geolocation.watchPosition(
			(position) => {
				console.log('Geolocation position obtained:', position)
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
				console.log('Geolocation position obtained:', position)
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
				maximumAge: 3000,
				timeout: 5000,
			}
		)
	}

	const confirmPlayerChoice = () => {
		setPlayerChoiceConfirmed(true)
		props.onConfirmPlayerChoice(clickedLatLng!)
	}

	const showFullScreenImage = () => {
		setZoomClass('zoomIn')
		setFullScreenImage(true)
	}

	const hideFullScreenImage = () => {
		setZoomClass('zoomOut')
		setTimeout(() => {
			setFullScreenImage(false)
		}, 400) //time spent zooming out
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
		setPlayerChoiceConfirmed(false)
		if (props.gameMode === GameMode.CLASSIC) {
			setClickedLatLng(null)
		} else if (props.gameMode === GameMode.GEOLOCATION) {
			console.log('Forcing geolocation update for next round')
			forceUpdateGeolocation()
		}
		props.onNextRound()
	}

	return (
		<div className={styles.game_interface}>
			{!fullScreenImage && (
				<div className={styles.game_header}>
					<h1>Round {props.currentRoundNumber + 1}</h1>
				</div>
			)}

			<div
				className={`${styles.image_container} ${fullScreenImage ? styles.fullscreen : ''}`}
				onClick={() => setFullScreenImage(!fullScreenImage)}
			>
				<img src={props.imageUrl!} alt="Round location" />
				{fullScreenImage && (
					<div className={styles.fullscreen_exit}>
						<ExitIcon />
					</div>
				)}
			</div>

			{props.error && <p style={{ color: 'red' }}>{props.error}</p>}

			{clickedLatLng && !playerChoiceConfirmed && !fullScreenImage && (
				<button className={styles.confirm_button} onClick={confirmPlayerChoice}>
					Confirm
				</button>
			)}

			{clickedLatLng && playerChoiceConfirmed && props.guessDistance && (
				<div className={styles.round_result}>
					{<h1 className={styles.distance}>Guess distance: {props.guessDistance.toFixed(2)}</h1>}
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

					{clickedLatLng && (
						<LocationMarker coords={clickedLatLng} icon={ClickedIcon} label="Clicked location:" />
					)}
					{playerChoiceConfirmed && (
						<TargetMarker
							clickedLatLng={clickedLatLng}
							targetLatLng={props.targetLatLng}
							icon={TargetIcon}
						/>
					)}

					{/* Manual location selection is disabled in GEOLOCATION mode */}
					{props.gameMode === GameMode.CLASSIC && (
						<SelectMapLocation selectLocationFunction={selectLocation} />
					)}
				</MapContainer>
			</div>
		</div>
	)
}

export default GameInterface
