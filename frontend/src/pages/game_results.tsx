import React from 'react';
import gameService from '../services/api/gameService';
import { useNavigate } from "react-router-dom";
import { useGameContext } from "../hooks/useGameContext";

const GameResults: React.FC = () => {
    const navigate = useNavigate();
    const { nickname, setNickname, difficulty, setDifficulty, score, setScore } = useGameContext();

    const returnToMenu = () => {
        gameService.deleteSession();
        navigate("/menu");           
    }

    return (
    <div>
        <h1>GAME RESULTS</h1>
        <p>Your score: {score.toFixed(2)}</p>
        <button onClick={returnToMenu}>Back to menu</button>
    </div>
    );
};

export default GameResults;