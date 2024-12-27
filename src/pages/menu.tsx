import React from 'react';

const Menu: React.FC = () => {
    return (
    <div>
        <h1>Main Menu</h1>
        <div>
            <a className="menu-option">Start game</a>
            <a className="menu-option">Settings</a>
            <a className="menu-option">Scoreboard</a>
        </div>
    </div>
    );
};

export default Menu;