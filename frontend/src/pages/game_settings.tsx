import { useNavigate } from "react-router-dom";
import Header from "../components/Header";
import styles from "../styles/GameSettings.module.scss";
import { useForm } from "react-hook-form";
import { StartGameData } from "../models/game/StartGameData";
import FormField from "../components/FormField";
import FormSelect from "../components/FormSelect";

import { GAME_ROUTE, MENU_ROUTE, SELECTED_DIFFICULTY_KEY, USER_NICKNAME_KEY, SELECTED_GAME_MODE} from "../Constants";
import accountService from "../services/api/accountService";
import gameService from "../services/api/gameService";
import { startGameManually } from "../utils/gameStartHelper";

const GameSettings: React.FC = () => {
	const navigate = useNavigate()

	const {
		register,
		handleSubmit,
		reset,
		formState: { errors },
	} = useForm<StartGameData>()

	const startGame = async (data: StartGameData) => {
		await startGameManually(data, navigate);
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
						label='Difficulty'
						name='difficulty'
						options={[
							{ value: 'easy', label: 'Easy' },
							{ value: 'normal', label: 'Normal' },
							{ value: 'hard', label: 'Hard' },
							{ value: 'ultra-nightmare', label: 'Ultra-Nightmare' },
						]}
						defaultValue={'normal'}
						register={register}
						error={errors.difficulty?.message}
					/>

					<FormSelect
						label="Game mode"
						name="gameMode"
						options={[
							{ value: 'classic', label: 'Classic' },
							{ value: 'geolocation', label: 'Geolocation' },
						]}
						defaultValue="classic"
						register={register}
						error={errors.gameMode?.message}
					/>

					<button type="submit" className={styles.start_game}>Start game</button>
				</form>
				<button className={styles.go_back} onClick={() => navigate(MENU_ROUTE)}>Go back</button>
			</div>
		</>
	);
}
export default GameSettings;
