import { Coordinates } from "../Coordinates";

export interface Place {
    id: string; //OBJECTID
    name: string;
    description: string;
    coordinates: Coordinates;
    imageUrl: string;
    alt: string;
    difficultyLevel: string;
}
