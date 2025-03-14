import { Place } from "./Place";

export interface RoundResultDto {
  originalPlace: Place;
  distanceDifference: number;
  roundNumber: number;
}
