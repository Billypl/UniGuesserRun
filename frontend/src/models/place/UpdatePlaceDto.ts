import { Coordinates } from "../Coordinates";

export interface UpdatePlaceDto {
    name: string;
    description: string;
    coordinates: Coordinates;
    imageUrl: string;
    alt: string;
    difficulty: string;
    authorId?: string | null;
}