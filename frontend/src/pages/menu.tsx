import React from "react";
import { useNavigate } from "react-router-dom";
import Header from "../components/Header";
import { GAME_SETTINGS_ROUTE, GAME_TOKEN_KEY, LOGIN_ROUTE, SCOREBOARD_ROUTE } from "../Constants";
import styles from "../styles/Menu.module.scss"

const Menu: React.FC = () => {
  const navigate = useNavigate();

  const openGameSettings = () => {
    window.sessionStorage.removeItem(GAME_TOKEN_KEY);
    navigate(GAME_SETTINGS_ROUTE);
  };

  return (
    <>
    <div className={styles.background}></div>
    <Header />

    <div className={styles.main_menu}>
      <div className={styles.catchy_phrase}>
        CZY ZNASZ TERENY SWOJEJ UCZELNI? <br/>
        Sprawdź i się przekonaj!
      </div>
      <div className={styles.options}>
        <a className={styles.menu_option} onClick={openGameSettings}>
          Start game
        </a>
        <a className={styles.menu_option} onClick={() => navigate(SCOREBOARD_ROUTE)}>
          Scoreboard
        </a>
      </div>
    </div>
    </>
  );
};

export default Menu;
