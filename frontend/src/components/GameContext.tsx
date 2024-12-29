import React, { createContext, useState, ReactNode } from "react";

// Define the shape of the context
export interface GameContextType {
  nickname: string;
  setNickname: (nickname: string) => void;
  difficulty: string;
  setDifficulty: (difficulty: string) => void;
}

// Create the context with a default value of `undefined`
export const GameContext = createContext<GameContextType | undefined>(undefined);

// Define the provider's props
interface GameContextProviderProps {
  children: ReactNode;
}

// Create the provider component
export const GameContextProvider: React.FC<GameContextProviderProps> = ({ children }) => {
  const [nickname, setNickname] = useState<string>("");
  const [difficulty, setDifficulty] = useState<string>("easy");

  const value: GameContextType = {
    nickname,
    setNickname,
    difficulty,
    setDifficulty,
  };

  return <GameContext.Provider value={value}>{children}</GameContext.Provider>;
};