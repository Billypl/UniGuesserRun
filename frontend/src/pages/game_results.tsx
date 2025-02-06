import React, { useState } from  'react';
import gameService from '../services/api/gameService';
import { useNavigate } from "react-router-dom";
import { useGameContext } from "../hooks/useGameContext";
import scoreboardService from '../services/api/scoreboardService';
import { MENU_ROUTE } from '../Constants';

const GameResults: React.FC = () => {
    const navigate = useNavigate();
    const { nickname, difficulty,  score } = useGameContext();
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
    <div>
        <h1>GAME RESULTS</h1>
        <p>Congratulations {nickname}!</p>
        <p>Your score: {score.toFixed(2)}</p>
        <p>On {difficulty} difficulty</p>
        {scoreSaved && <button>Score saved!</button>}
        {!scoreSaved && <button onClick={saveGameScore}>Save score</button>}
        <br />
        <button onClick={returnToMenu}>Back to menu</button>
    </div>
    );
};

export default GameResults;