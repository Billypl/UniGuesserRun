import { ShowPlaceDto } from "../place/ShowPlaceDto";

export interface RoundResultDto {
  originalPlace: ShowPlaceDto;
  distanceDifference: number;
  score: number;
  roundNumber: number;
}
