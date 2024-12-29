import { useContext } from "react";
import { GameContext, GameContextType } from "../components/GameContext";

export const useGameContext = (): GameContextType => {
  const context = useContext(GameContext);
  if (!context) {
    throw new Error("useGameContext must be used within a GameContextProvider");
  }
  return context;
};
