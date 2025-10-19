import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import Header from '../components/Header'
import FormField from '../components/FormField'
import FormSelect from '../components/FormSelect'
import { StartGameData } from '../models/game/StartGameData'
import { GameSessionStateDto } from '../models/game/GameSessionState'
import accountService from '../services/api/accountService'
import gameService from '../services/api/gameService'
import {
	GAME_ROUTE,
	MENU_ROUTE,
	SELECTED_DIFFICULTY_KEY,
	USER_NICKNAME_KEY,
	SELECTED_GAME_MODE,
	GAME_GUID,
	GAME_TOKEN_KEY,
} from '../Constants'
import styles from '../styles/GameSettings.module.scss'

const DIFFICULTY_OPTIONS = [
	{ value: 'easy', label: 'Easy' },
	{ value: 'normal', label: 'Normal' },
	{ value: 'hard', label: 'Hard' },
	{ value: 'ultra-nightmare', label: 'Ultra-Nightmare' },
]

const GAME_MODE_OPTIONS = [
	{ value: 'classic', label: 'Classic' },
	{ value: 'geolocation', label: 'Geolocation' },
]

const GameSettings: React.FC = () => {
	const [existingGameData, setExistingGameData] = useState<GameSessionStateDto | null>(null)
	const navigate = useNavigate()

	const {
		register,
		handleSubmit,
		formState: { errors },
	} = useForm<StartGameData>()

	const clearGameSession = () => {
		window.sessionStorage.removeItem(GAME_ROUTE)
		window.sessionStorage.removeItem(GAME_GUID)
		window.sessionStorage.removeItem(GAME_TOKEN_KEY)
	}

	const saveGameSettings = (data: StartGameData) => {
		window.sessionStorage.setItem(SELECTED_GAME_MODE, data.gameMode)
		window.sessionStorage.setItem(SELECTED_DIFFICULTY_KEY, data.difficulty)

		if (!accountService.isLoggedIn()) {
			window.sessionStorage.setItem(USER_NICKNAME_KEY, data.nickname)
		}
	}

	const handleExistingGame = async (gameData: GameSessionStateDto | null) => {
		if (gameData) {
			setExistingGameData(gameData)
		} else {
			navigate(GAME_ROUTE)
		}
	}

	const handleGuestGameStart = async () => {
		try {
			const gameData = await gameService.checkGameState()
			await handleExistingGame(gameData)
		} catch (error) {
			console.error('Error checking game state:', error)
			clearGameSession()
			navigate(GAME_ROUTE)
		}
	}

	const handleLoggedInGameStart = async () => {
		try {
			const gameData = await gameService.setUpGameTokenIfUserHasGame()
			await handleExistingGame(gameData ?? null)
		} catch (error) {
			console.error('Error setting up game token:', error)
			clearGameSession()
			navigate(GAME_ROUTE)
		}
	}

	const startGame = async (data: StartGameData) => {
		saveGameSettings(data)

		if (accountService.isLoggedIn()) {
			await handleLoggedInGameStart()
		} else {
			await handleGuestGameStart()
		}
	}

	const startNewGame = async () => {
		try {
			await gameService.abandonGame()
			clearGameSession()
			navigate(GAME_ROUTE)
		} catch (error) {
			console.error('Error abandoning game:', error)
		}
	}

	const continueExistingGame = () => {
		navigate(GAME_ROUTE)
	}

	const closeModal = () => {
		setExistingGameData(null)
	}

	const renderExistingGameModal = () => {
		if (!existingGameData) return null

		return (
			<>
				<div className={styles.modal_backdrop} onClick={closeModal} />
				<div className={styles.existing_game_modal}>
					<h3>You have an ongoing game!</h3>
					<div className={styles.game_info}>
						<p>
							Game Difficulty: <strong>{existingGameData.difficulty}</strong>
						</p>
						<p>
							Current Round: <strong>{existingGameData.actualRoundNumber + 1}</strong>
						</p>
						<p>
							Time left:{' '}
							<strong>
								{Math.ceil(
									(new Date(existingGameData.expirationDate).getTime() - Date.now()) / 1000
								)}{' '}
								seconds
							</strong>
						</p>
						<p>
							Game Mode: <strong>{existingGameData.gameMode}</strong>
						</p>
					</div>
					<p className={styles.modal_question}>
						Would you like to continue your previous game or start a new one?
					</p>
					<div className={styles.modal_actions}>
						<button className={styles.continue_game} onClick={continueExistingGame}>
							Continue
						</button>
						<button className={styles.start_new_game} onClick={startNewGame}>
							Start New Game
						</button>
					</div>
				</div>
			</>
		)
	}

	return (
		<>
			<Header />
			<div className={styles.settings}>
				<h2 className={styles.header}>Game settings</h2>

				<form onSubmit={handleSubmit(startGame)} className={styles.form}>
					{!accountService.isLoggedIn() && (
						<FormField
							label="Nickname"
							name="nickname"
							type="text"
							register={register}
							error={errors.nickname?.message}
						/>
					)}

					<FormSelect
						label="Difficulty"
						name="difficulty"
						options={DIFFICULTY_OPTIONS}
						defaultValue="normal"
						register={register}
						error={errors.difficulty?.message}
					/>

					<FormSelect
						label="Game mode"
						name="gameMode"
						options={GAME_MODE_OPTIONS}
						defaultValue="classic"
						register={register}
						error={errors.gameMode?.message}
					/>

					<button type="submit" className={styles.start_game}>
						Start game
					</button>
				</form>

				<button className={styles.go_back} onClick={() => navigate(MENU_ROUTE)}>
					Go back
				</button>
			</div>

			{renderExistingGameModal()}
		</>
	)
}

export default GameSettings
