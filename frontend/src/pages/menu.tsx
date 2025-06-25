import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Header from "../components/Header";
import { GAME_SETTINGS_ROUTE, GAME_TOKEN_KEY, GAME_ROUTE, SCOREBOARD_ROUTE } from "../Constants";
import styles from "../styles/Menu.module.scss";
import gameService from "../services/api/gameService";
import { startGameManually } from "../utils/gameStartHelper";
import { GameSession } from "../models/game/GameSession";

const Menu: React.FC = () => {
  const navigate = useNavigate();
  const [gameExist, setGameExist] = useState<boolean>(false);
  const [gameState, setGameState] = useState<GameSession | null>(null);

  useEffect(() => {
    checkActiveGameState();
  }, []);

  const checkActiveGameState = async () => {
    try {
      
      const game = await gameService.checkActiveGameState();
      if (game) {
        setGameExist(true)
        setGameState(game);
      }
      else {
        setGameExist(false);
      }
    } catch (error) {
      setGameExist(false);
    }
  }

  const openGameSettings = () => {
    window.sessionStorage.removeItem(GAME_TOKEN_KEY);
    navigate(GAME_SETTINGS_ROUTE);
  };

  const continueGame = async () => {
    if (!gameState) {
      console.error("No active game state found.");
      return;
    }
    console.log("Continuing game with state:", gameState);
    await startGameManually({
      gameMode: gameState.gameMode,
      difficulty: gameState.difficulty,
      nickname: window.sessionStorage.getItem('USER_NICKNAME_KEY') || '',
    }, navigate);
  };

  return (
    <>
      <div className={styles.background}></div>
      <Header />
      <div className={styles.main_menu}>
        <div className={styles.catchy_phrase}>
          CZY ZNASZ TERENY SWOJEJ UCZELNI? <br />
          Sprawdź i się przekonaj!
        </div>
        <div className={styles.options}>
          {gameExist &&
            (<a className={styles.menu_option} onClick={continueGame}>
              Continue game
            </a>)
          }
          <a className={styles.menu_option} onClick={openGameSettings}>
            Start game
          </a>
          <a className={styles.menu_option} onClick={() => navigate(SCOREBOARD_ROUTE)}>
            Scoreboard
          </a>
        </div>
      </div >
    </>
  );
};

export default Menu;
