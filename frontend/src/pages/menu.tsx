import React from "react";
import { useNavigate } from "react-router-dom";
import { useGameContext } from "../hooks/useGameContext";

const Menu: React.FC = () => {
  const navigate = useNavigate();
  const { nickname, setNickname, difficulty, setDifficulty } = useGameContext();

  const startGame = () => {
    window.sessionStorage.removeItem("token");
    navigate("/game");
  };

  const showScoreboard = () => {
    navigate("/scoreboard");
  };

  const showLoginForm = () => {
    navigate("/login");
  };

  const showRegisterForm = () => {
    navigate("/register");
  };

  return (
    <div>
      <h1>Main Menu</h1>
      <div className="account-option" onClick={showLoginForm}>Login</div>
      <div className="account-option" onClick={showRegisterForm}>Sign in</div>
      <div>
        <ul>
          <li>
            Username: <input type="text" onChange={(e) => setNickname(e.target.value)}></input>
          </li>
          <li className="menu-option" onClick={startGame}>
            Start game
          </li>
          {/* <li className="menu-option">Settings</li> */}
          <li className="menu-option" onClick={showScoreboard}>
            Scoreboard
          </li>
        </ul>
      </div>
    </div>
  );
};

export default Menu;
