import React, { useEffect, useState } from 'react'
import 'leaflet/dist/leaflet.css'
import gameService from '../services/api/gameService'
import { Coordinates } from '../models/Coordinates'
import { useGameContext } from '../hooks/useGameContext'
import { useNavigate } from 'react-router-dom'
import GameInterface from '../components/GameInterface'
import {
	GAME_GUID,
	GAME_RESULTS_ROUTE,
	GAME_TOKEN_KEY,
	SELECTED_DIFFICULTY_KEY,
	SELECTED_GAME_MODE,
	USER_NICKNAME_KEY,
} from '../Constants'

// Latitude: 54.371513, Longitude: 18.619164 <- Gmach Główny
const Game: React.FC = () => {
	const { setScore } = useGameContext()

	const [loading, setLoading] = useState<boolean>(false)
	const [currentRoundNumber, setCurrentRoundNumber] = useState<number | null>(null)
	const [error, setError] = useState<string | null>(null)

	const [imageUrl, setImage] = useState<string | null>(null)
	const [playerLatLng, setPlayerLatLng] = useState<Coordinates | null>(null)
	const [targetLatLng, setTargetLatLng] = useState<Coordinates | null>(null)
	const [guessDistance, setGuessDistance] = useState<number | null>(null)

	const ROUND_NUMBER: number = 5
	const navigate = useNavigate()

	useEffect(() => {
		const controller = new AbortController()
		const signal = controller.signal

		if (gameService.hasToken()) {
			getGame(signal)
		} else {
			startGame(signal)
		}

		return () => {
			controller.abort()
		}
	}, [])

	const startGame = async (signal: AbortSignal) => {
		setLoading(true)
		setError(null)

		try {
			const nickname = window.sessionStorage.getItem(USER_NICKNAME_KEY)
			const difficulty = window.sessionStorage.getItem(SELECTED_DIFFICULTY_KEY)
			const gameMode = window.sessionStorage.getItem(SELECTED_GAME_MODE)
			if (!difficulty) {
				throw new Error('Difficulty not selected')
			}
			if (!nickname) {
				throw new Error('User not logged in')
			}
			await gameService.startNewGameSession(nickname ?? '', difficulty ?? '', gameMode ?? '', signal)
			startRound(0)
		} catch (err: any) {
			if (err.name === 'CanceledError') {
				console.error('Request aborted by the abort conttoler (2nd fetch prevention')
			} else {
				setError('Failed to fetch data. Please try again later.')
				console.error('Error fetching data:', err)
			}
		} finally {
			setLoading(false)
		}
	}

	const getGame = async (signal: AbortSignal) => {
		setLoading(true)
		setError(null)

		try {
			const response = await gameService.checkGameState(signal)
			startRound(response.actualRoundNumber)
		} catch (err: any) {
			if (err.name === 'CanceledError') {
				console.error('Request aborted by the abort controller (2nd fetch prevention)')
				return
			} else {
				setError('Failed to fetch data. Please try again later.')
				//console.error('Error fetching data:', err)
				console.log('nieudalo sie wczytac stanu gry, tworzymy nowa gre \n', err)
				const nickname = window.sessionStorage.getItem(USER_NICKNAME_KEY)
				const newController = new AbortController()
				const newSignal = newController.signal
				if (!nickname) {
					throw new Error('User nickname is missing')
				}
				startGame(newSignal)
			}
		} finally {
			setLoading(false)
		}
	}

	const startRound = (round: number) => {
		setCurrentRoundNumber(round)
	}

	useEffect(() => {
		if (currentRoundNumber != null && gameService.hasToken()) {
			fetchGuessingPlace()
		}
	}, [currentRoundNumber])

	const fetchGuessingPlace = async () => {
		setError(null)

		try {
			const guessingPlace = await gameService.getGuessingPlace(currentRoundNumber!)
			setImage(guessingPlace.imageUrl)
		} catch (err: any) {
			setError('Failed to fetch data. Please try again later.')
			console.error('Error fetching data:', err)
		} finally {
			setLoading(false)
		}
	}

	const confirmPlayerChoice = (clickedLatLng: Coordinates) => {
		checkPlayerChoice(clickedLatLng)
	}

	const checkPlayerChoice = async (clickedLatLng: Coordinates) => {
		const roundResult = await gameService.checkGuess(clickedLatLng)
		setTargetLatLng(roundResult.originalPlace.coordinates)
		setGuessDistance(roundResult.distanceDifference)
	}

	const isLastRound = (currentRoundNumber: number): boolean => {
		return currentRoundNumber === ROUND_NUMBER - 1
	}

	const nextRound = () => {
		resetGameState()
		startRound(currentRoundNumber! + 1)
	}

	const finishGame = async () => {
		const response = await gameService.finishGame()
		setScore(response.finalScore)

		navigate(GAME_RESULTS_ROUTE)
	}

	const resetGameState = () => {
		setGuessDistance(null)
	}

	const getCoordinates = () => {
		if (!('geolocation' in navigator)) {
			setError('Geolocation is not supported by your browser.')
			return
		}
		navigator.geolocation.getCurrentPosition(
			position => {
				setPlayerLatLng(position.coords)
				setError(null)
			},
			error => {
				setError('Unable to retrieve location. Please enable location services.')
				console.error(error)
			},
			{
				enableHighAccuracy: true,
			}
		)
	}

	return (
		<div>
			{loading && <h1>Loading...</h1>}
			{imageUrl && currentRoundNumber != null && (
				<GameInterface
					error={error}
					currentRoundNumber={currentRoundNumber}
					isLastRound={isLastRound(currentRoundNumber)}
					imageUrl={imageUrl}
					guessDistance={guessDistance}
					targetLatLng={targetLatLng}
					onConfirmPlayerChoice={confirmPlayerChoice}
					onNextRound={nextRound}
					onFinishGame={finishGame}
				/>
			)}
		</div>
	)
}

export default Game
