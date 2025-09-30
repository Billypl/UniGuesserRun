import { Coordinates } from "../Coordinates";
import { Difficulty } from "../game/Difficulty";

export interface ShowPlaceDto {
    id: string; //OBJECTID
    name: string;
    description: string;
    coordinates: Coordinates;
    imageUrl: string;
    alt: string;
    difficultyLevel: Difficulty;
    authorId?: string | null; //OBJECTID
}
