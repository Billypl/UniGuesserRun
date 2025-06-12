import { ShowPlaceDto } from "../place/ShowPlaceDto";

export interface RoundResultDto {
  originalPlace: ShowPlaceDto;
  distanceDifference: number;
  roundNumber: number;
}
