import React, { useState } from  'react';
import gameService from '../services/api/gameService';
import { useNavigate } from "react-router-dom";
import { useGameContext } from "../hooks/useGameContext";
import scoreboardService from '../services/api/scoreboardService';
import { MENU_ROUTE } from '../Constants';
import styles from "../styles/GameResults.module.scss";
import accountService from '../services/api/accountService';

const GameResults: React.FC = () => {
    const navigate = useNavigate();
    const { difficulty,  score } = useGameContext();
    const [scoreSaved, setScoreSaved] = useState<boolean>(false);

    const returnToMenu = () => {
        gameService.deleteSession();
        navigate(MENU_ROUTE);           
    }

    const saveGameScore = () => {
        scoreboardService.saveScore();
        setScoreSaved(true);
    }

    return (
    <div className={styles.result_container}>
        <h1>GAME RESULTS</h1>
        <h2>Congratulations {accountService.getCurrentUserNickname()}!</h2>
        <p>Your score: <b>{score.toFixed(2)}</b></p>
        <p>On <b>{difficulty}</b> difficulty</p>
        {scoreSaved && <button>Score saved!</button>}
        {!scoreSaved && <button onClick={saveGameScore}>Save score</button>}
        <br />
        <button onClick={returnToMenu}>Back to menu</button>
    </div>
    );
};

export default GameResults;