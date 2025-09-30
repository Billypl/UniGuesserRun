import { Difficulty } from "./Difficulty";

export interface GameSessionStateDto {
    id: string;
    expirationDate: Date;
    actualRoundNumber: number;
    difficultyLevel: Difficulty;
}
