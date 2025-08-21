
import { Coordinates } from "../Coordinates";

export interface FinishedGameDto {
  id: string;
  userId: string;
  nickname: string;
  finalScore: number;
  rounds: Round[];
  difficulty: string;
}

export interface Round {
  idPlaceToGuess: string;
  guessedCoordinates: Coordinates;
  score: number;
}
