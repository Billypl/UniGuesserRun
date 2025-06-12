import { Coordinates } from "../Coordinates";

export interface ShowPlaceDto {
    id: string; //OBJECTID
    name: string;
    description: string;
    coordinates: Coordinates;
    imageUrl: string;
    alt: string;
    difficultyLevel: string;
    authorId?: string | null; //OBJECTID
}
