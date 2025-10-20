import {StartGameData} from '../models/game/StartGameData';

import { SELECTED_DIFFICULTY_KEY, SELECTED_GAME_MODE, USER_NICKNAME_KEY, GAME_ROUTE } from '../Constants';
import accountService from '../services/api/accountService';
import gameService from '../services/api/gameService';

export const startGameManually = async (data: StartGameData, navigate: (route: string) => void) => {
  window.sessionStorage.setItem(SELECTED_GAME_MODE, data.gameMode);
  window.sessionStorage.setItem(SELECTED_DIFFICULTY_KEY, data.difficulty);
  window.sessionStorage.setItem(SELECTED_GAME_MODE, data.gameMode);

  if (!accountService.isLoggedIn()) {
    console.log('niezalogowany')
    window.sessionStorage.setItem(USER_NICKNAME_KEY, data.nickname);
  } else {
    console.log('zalogowany')
    await gameService.setUpGameTokenIfUserHasGame();
  }

  navigate(GAME_ROUTE);
};