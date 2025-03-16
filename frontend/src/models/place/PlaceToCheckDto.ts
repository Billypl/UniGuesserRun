import { Coordinates } from "../Coordinates";
import { NewPlaceDto } from "./NewPlaceDto";

export interface PlaceToCheckDto {
    id: string;
    newPlace: NewPlaceDto;
    authorId: string;
    createdAt: string; //DATETIME
}
