import { Coordinates } from "../Coordinates";

export interface NewPlaceDto {
    name: string;
    description: string;
    coordinates: Coordinates;
    imageUrl: string;
    alt: string;
    difficulty: string;
}