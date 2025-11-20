import { Coordinates } from "../Coordinates";
import { Difficulty } from "../game/Difficulty";

export interface NewPlaceDto {
    name: string;
    description: string;
    coordinates: Coordinates;
    imageUrl: string;
    alt: string;
    difficulty: Difficulty;
}