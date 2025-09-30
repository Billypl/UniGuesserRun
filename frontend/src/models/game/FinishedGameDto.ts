import { Difficulty } from "./Difficulty";

export interface FinishedGameDto {
  id: string;
  userId: string;
  nickname: string;
  finalScore: number;
  rounds: Round[];
  difficulty: Difficulty;
}

export interface Round {
  latitude: number;
  longitude: number;
  placeToGuess: Place;
  score: number;
}

export interface Place {
  id: string;
  name: string;
  latitude: number;
  longitude: number;
}
