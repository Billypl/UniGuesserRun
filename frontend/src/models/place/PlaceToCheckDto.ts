import { NewPlaceDto } from "./NewPlaceDto";

export interface PlaceToCheckDto {
    id: string;
    newPlace: NewPlaceDto;
    coordinates: string;
    authorId: string;
    createdAt: string; //DATETIME
}
