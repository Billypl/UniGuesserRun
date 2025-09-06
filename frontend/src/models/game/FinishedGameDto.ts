
import { Coordinates } from "../Coordinates";
import { PlaceToCheckDto } from "../place/PlaceToCheckDto";

export interface FinishedGameDto {
  id: string;
  userId: string;
  nickname: string;
  finalScore: number;
  rounds: Round[];
  difficulty: string;
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
