import React from "react";
import { useNavigate } from "react-router-dom";
import { useGameContext } from "../hooks/useGameContext";
import Header from "../components/Header";
import { GAME_ROUTE, GAME_TOKEN_KEY, LOGIN_ROUTE, REGISTER_ROUTE, SCOREBOARD_ROUTE } from "../Constants";
import styles from "../styles/Menu.module.scss"

const Menu: React.FC = () => {
  const navigate = useNavigate();
  const { nickname, setNickname, difficulty, setDifficulty } = useGameContext();

  const startGame = () => {
    window.sessionStorage.removeItem(GAME_TOKEN_KEY);
    navigate(GAME_ROUTE);
  };

  return (
    <>
    <Header />
    <div className={styles.main_menu}>
      <div className={styles.menu_option}>
        Username: <input type="text" onChange={(e) => setNickname(e.target.value)}></input>
      </div>
      <div className={styles.menu_option} onClick={startGame}>
        Start game
      </div>
      <div className={styles.menu_option} onClick={() => navigate(SCOREBOARD_ROUTE)}>
        Scoreboard
      </div>
    </div>
    </>
  );
};

export default Menu;
