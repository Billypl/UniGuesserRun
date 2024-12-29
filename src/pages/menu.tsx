import { URL } from 'node:url';
import React from 'react';
import { GameService } from '../services/api/gameService';
import { useNavigate } from 'react-router-dom';

const Menu: React.FC = () => {
    const navigate = useNavigate();

    const startGame = () => {
        navigate("/game");
    };

    const showScoreboard = () => {
        navigate("/scoreboard");
    };

    return (
    <div>
        <h1>Main Menu</h1>
        <div>
            <ul>
                <li>Username: <input type="text"></input></li>
                <li className="menu-option" onClick={startGame}>Start game</li>
                {/* <li className="menu-option">Settings</li> */}
                <li className="menu-option" onClick={showScoreboard}>Scoreboard</li>
            </ul>
        </div>
    </div>
    );
};

export default Menu;