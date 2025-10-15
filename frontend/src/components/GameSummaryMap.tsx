import { FinishedGameDto } from '../models/game/FinishedGameDto'
import { MapContainer, TileLayer, Marker, Popup, Polyline, Tooltip } from 'react-leaflet'
import { MAP_CENTER } from '../Constants'
import React from 'react'
import L from 'leaflet'

interface GameSummaryMapProps {
	finishedGameData: FinishedGameDto
	highlightedRound: number | null
}

const GameSummaryMap = ({ finishedGameData, highlightedRound }: GameSummaryMapProps) => {
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

	// Kolory dla różnych rund - stałe kolory dla każdej rundy
	const getRoundColors = (index: number, isHighlighted: boolean) => {
		// Używamy stałego koloru dla wszystkich rund
		const guessColor = '#e74c3c' // Czerwony dla wyboru użytkownika
		const targetColor = '#2ecc71' // Zielony dla celu

		return {
			guess: isHighlighted ? guessColor : guessColor + 'BB', // BB = ~73% opacity
			target: isHighlighted ? targetColor : targetColor + 'BB',
		}
	}

	const createNumberedIcon = (number: number, color: string, isHighlighted: boolean) => {
		return L.divIcon({
			className: 'custom-numbered-marker',
			html: `
				<div class="marker-content ${
					isHighlighted ? 'highlighted' : ''
				}" style="background-color: ${color}; position:relative; bottom:-40px; right: -10px;">
					${number}
				</div>
			`,
			iconSize: [isHighlighted ? 40 : 32, isHighlighted ? 40 : 32],
			iconAnchor: [isHighlighted ? 20 : 16, isHighlighted ? 20 : 16],
		})
	}

	const calculateDistance = (lat1: number, lon1: number, lat2: number, lon2: number): number => {
		const R = 6371
		const dLat = ((lat2 - lat1) * Math.PI) / 180
		const dLon = ((lon2 - lon1) * Math.PI) / 180
		const a =
			Math.sin(dLat / 2) * Math.sin(dLat / 2) +
			Math.cos((lat1 * Math.PI) / 180) *
				Math.cos((lat2 * Math.PI) / 180) *
				Math.sin(dLon / 2) *
				Math.sin(dLon / 2)
		const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a))
		return R * c
	}

	const showRoundsTargets = () => {
		return finishedGameData.rounds.map((round, index) => {
			const clickedCoords: [number, number] = [round.latitude, round.longitude]
			const targetCoords: [number, number] = [
				round.placeToGuess.latitude,
				round.placeToGuess.longitude,
			]
			const isHighlighted = highlightedRound === index
			const colors = getRoundColors(index, isHighlighted)
			const distance = calculateDistance(
				round.latitude,
				round.longitude,
				round.placeToGuess.latitude,
				round.placeToGuess.longitude
			)

			return (
				<React.Fragment key={index}>
					{/* Linia łącząca punkty - NIE ZMIENIAJ TEJ LINII */}
					<Polyline
						pathOptions={{
							color: 'black',
							dashArray: '1 5',
							weight: isHighlighted ? 3 : 2,
							opacity: isHighlighted ? 0.8 : 0.5,
						}}
						positions={[clickedCoords, targetCoords]}
					/>

					{/* Marker Twojego wyboru */}
					<Marker
						position={clickedCoords}
						icon={createNumberedIcon(index + 1, colors.guess, isHighlighted)}
					>
						<Tooltip direction="top" offset={[0, -15]} opacity={0.95} permanent={false}>
							<div style={{ textAlign: 'center', fontSize: '12px' }}>
								<strong>Runda {index + 1} - Twój wybór</strong>
							</div>
						</Tooltip>
						<Popup>
							<div style={{ textAlign: 'center', minWidth: '200px' }}>
								<strong style={{ fontSize: '16px', color: colors.guess }}>
									Runda {index + 1} - Twój wybór
								</strong>
								<hr style={{ margin: '8px 0', border: 'none', borderTop: '1px solid #ddd' }} />
								<p style={{ margin: '6px 0', fontSize: '13px' }}>📍 Współrzędne:</p>
								<p style={{ margin: '4px 0', fontSize: '12px', color: '#666' }}>
									{round.latitude.toFixed(6)}, {round.longitude.toFixed(6)}
								</p>
								<p style={{ margin: '8px 0', fontSize: '14px' }}>
									<strong>Odległość:</strong>{' '}
									<span style={{ color: '#e74c3c' }}>{distance.toFixed(2)} km</span>
								</p>
								<p style={{ margin: '4px 0', fontSize: '14px' }}>
									<strong>Punkty:</strong>{' '}
									<span style={{ color: '#2ecc71' }}>{round.score.toFixed(2)}</span>
								</p>
							</div>
						</Popup>
					</Marker>

					{/* Marker celu */}
					<Marker
						position={targetCoords}
						icon={createNumberedIcon(index + 1, colors.target, isHighlighted)}
					>
						<Tooltip direction="top" offset={[0, -15]} opacity={0.95} permanent={false}>
							<div style={{ textAlign: 'center', fontSize: '12px' }}>
								<strong>
									Runda {index + 1} - {round.placeToGuess.name}
								</strong>
							</div>
						</Tooltip>
						<Popup>
							<div style={{ textAlign: 'center', minWidth: '200px' }}>
								<strong style={{ fontSize: '16px', color: colors.target }}>
									Runda {index + 1} - Cel
								</strong>
								<hr style={{ margin: '8px 0', border: 'none', borderTop: '1px solid #ddd' }} />
								<p style={{ margin: '6px 0', fontSize: '15px' }}>
									<strong>{round.placeToGuess.name}</strong>
								</p>
								<p style={{ margin: '6px 0', fontSize: '13px' }}>📍 Prawidłowa lokalizacja</p>
								<p style={{ margin: '4px 0', fontSize: '12px', color: '#666' }}>
									{round.placeToGuess.latitude.toFixed(6)},{' '}
									{round.placeToGuess.longitude.toFixed(6)}
								</p>
								<p style={{ margin: '8px 0', fontSize: '14px' }}>
									<strong>Twoja odległość:</strong>{' '}
									<span style={{ color: '#e74c3c' }}>{distance.toFixed(2)} km</span>
								</p>
								<p style={{ margin: '4px 0', fontSize: '14px' }}>
									<strong>Punkty:</strong>{' '}
									<span style={{ color: '#2ecc71' }}>{round.score.toFixed(2)}</span>
								</p>
							</div>
						</Popup>
					</Marker>
				</React.Fragment>
			)
		})
	}

	return (
		<MapContainer
			center={finishedGameData.rounds ? undefined : MAP_CENTER} // default map center
			bounds={finishedGameData.rounds ? getMapBounds() : undefined}
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
	)
}

export default GameSummaryMap
