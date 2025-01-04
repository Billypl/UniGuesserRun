import React from 'react';
import gameService from '../services/api/gameService';
import { useNavigate } from "react-router-dom";
import { useGameContext } from "../hooks/useGameContext";

const GameResults: React.FC = () => {
    const navigate = useNavigate();
    const { nickname, difficulty,  score } = useGameContext();

    const returnToMenu = () => {
        gameService.deleteSession();
        navigate("/menu");           
    }

    return (
    <div>
        <h1>GAME RESULTS</h1>
        <p>Congratulations {nickname}!</p>
        <p>Your score: {score.toFixed(2)}</p>
        <p>On {difficulty} difficulty</p>
        <button onClick={returnToMenu}>Back to menu</button>
    </div>
    );
};

export default GameResults;