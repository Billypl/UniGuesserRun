import React, { useState } from  'react';
import gameService from '../services/api/gameService';
import { useNavigate } from "react-router-dom";
import { useGameContext } from "../hooks/useGameContext";
import scoreboardService from '../services/api/scoreboardService';
import { MENU_ROUTE, SELECTED_DIFFICULTY_KEY } from '../Constants';
import styles from "../styles/GameResults.module.scss";
import accountService from '../services/api/accountService';

const GameResults: React.FC = () => {
    const navigate = useNavigate();
    const { score } = useGameContext();

    const returnToMenu = () => {
        navigate(MENU_ROUTE);           
    }

    return (
    <div className={styles.result_container}>
        <h1>GAME RESULTS</h1>
        <h2>Congratulations {accountService.getCurrentUser()?.nickname}!</h2>
        <p>Your score: <b>{score.toFixed(2)}</b></p>
        <p>On <b>{window.sessionStorage.getItem(SELECTED_DIFFICULTY_KEY)}</b> difficulty</p>
        <br />
        <button onClick={returnToMenu}>Back to menu</button>
    </div>
    );
};

export default GameResults;