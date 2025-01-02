import React, { createContext, useState, ReactNode } from "react";

export interface GameContextType {
  nickname: string;
  setNickname: (nickname: string) => void;
  difficulty: string;
  setDifficulty: (difficulty: string) => void;
  score: number;
  setScore: (score: number) => void;
}

export const GameContext = createContext<GameContextType | undefined>(undefined);

interface GameContextProviderProps {
  children: ReactNode;
}

export const GameContextProvider: React.FC<GameContextProviderProps> = ({ children }) => {
  const [nickname, setNickname] = useState<string>("");
  const [difficulty, setDifficulty] = useState<string>("easy");
  const [score, setScore] = useState<number>(0);

  const value: GameContextType = {
    nickname,
    setNickname,
    difficulty,
    setDifficulty,
    score,
    setScore
  };

  return <GameContext.Provider value={value}>{children}</GameContext.Provider>;
};