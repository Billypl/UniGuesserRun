import React, { createContext, useState, ReactNode } from 'react'
import { FinishedGameDto } from '../models/game/FinishedGameDto'

export interface GameContextType {
	nickname: string
	setNickname: (nickname: string) => void
	difficulty: string
	setDifficulty: (difficulty: string) => void
	finishedGameData: FinishedGameDto | null
	setFinishedGameData: (data: FinishedGameDto | null) => void
}

export const GameContext = createContext<GameContextType | undefined>(undefined)

interface GameContextProviderProps {
	children: ReactNode
}

export const GameContextProvider: React.FC<GameContextProviderProps> = ({ children }) => {
	const [nickname, setNickname] = useState<string>('')
	const [difficulty, setDifficulty] = useState<string>('easy')
	const [finishedGameData, setFinishedGameData] = useState<FinishedGameDto | null>(null)

	const value: GameContextType = {
		nickname,
		setNickname,
		difficulty,
		setDifficulty,
		finishedGameData,
		setFinishedGameData,
	}

	return <GameContext.Provider value={value}>{children}</GameContext.Provider>
}
