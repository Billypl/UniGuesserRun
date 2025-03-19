import { Place } from "../place/Place";

export interface RoundResultDto {
  originalPlace: Place;
  distanceDifference: number;
  roundNumber: number;
}
