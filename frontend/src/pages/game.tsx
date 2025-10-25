import React, { useEffect, useState } from 'react'
import 'leaflet/dist/leaflet.css'
import gameService from '../services/api/gameService'
import { Coordinates } from '../models/Coordinates'
import { useNavigate } from 'react-router-dom'
import GameInterface from '../components/GameInterface'
import {
	GAME_RESULTS_ROUTE,
	SELECTED_DIFFICULTY_KEY,
	SELECTED_GAME_MODE,
	USER_NICKNAME_KEY,
	GAME_GUID,
} from '../Constants'
import { Difficulty } from '../models/game/Difficulty'
import { GameMode } from '../models/game/GameMode'

// Latitude: 54.371513, Longitude: 18.619164 <- Gmach Główny
const Game: React.FC = () => {
	const [loading, setLoading] = useState<boolean>(false)
	const [currentRoundNumber, setCurrentRoundNumber] = useState<number | null>(null)
	const [error, setError] = useState<string | null>(null)

	const [imageUrl, setImage] = useState<string | null>(null)
	const [targetLatLng, setTargetLatLng] = useState<Coordinates | null>(null)
	const [guessDistance, setGuessDistance] = useState<number | null>(null)

	const ROUND_NUMBER: number = 5
	const navigate = useNavigate()


	useEffect(() => {
		const controller = new AbortController()
		const signal = controller.signal

		if (gameService.hasToken()) {
			console.log('Game token exists, fetching game state...')
			getGame(signal)
		} else {
			console.log('Calling startGame')
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
			const difficulty = window.sessionStorage.getItem(SELECTED_DIFFICULTY_KEY) as Difficulty | null
			const gameMode = window.sessionStorage.getItem(SELECTED_GAME_MODE) as GameMode | null
			// Reset game GUID before starting a new game
			if (!difficulty) {
				throw new Error('Difficulty not selected')
			}
			if (!gameMode) {
				throw new Error('Game mode not selected')
			}
			if (!nickname) {
				throw new Error('User not logged in')
			}
			await gameService.startNewGameSession(nickname, difficulty, gameMode, signal)
			startRound(0)
		} catch (err: any) {
			if (err.name === 'CanceledError') {
				console.error('Request aborted by the abort controller (2nd fetch prevention)')
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

		console.log('GETOWANIE GRY PO SPRAWDZENIU CZY ISTNIEJE')
		try {
			const response = await gameService.checkGameState(signal)
			console.log('Game state fetched successfully:', response)
			startRound(response.actualRoundNumber)
		} catch (err: any) {
			if (err.name === 'CanceledError') {
				console.error('Request aborted by the abort controller (2nd fetch prevention)')
				return
			} else {
				setError('Failed to fetch data. Please try again later.')
				//console.error('Error fetching data:', err)
				console.log('nie udalo sie wczytac stanu gry, tworzymy nowa gre \n', err)
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
		console.log('Finishing game...')
		const gameGuid = sessionStorage.getItem(GAME_GUID)
		const response = await gameService.finishGame()
		navigate(`${GAME_RESULTS_ROUTE}/${gameGuid}`)
	}

	const resetGameState = () => {
		setGuessDistance(null)
		setTargetLatLng(null)
	}

	return (
		<div>
			{loading && <h1>Loading...</h1>}
			{imageUrl && currentRoundNumber != null && (
				<GameInterface
					error={error}
					gameMode={GameMode.GEOLOCATION}
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
