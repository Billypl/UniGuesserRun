export interface GameSessionStateDto {
    publicId: string;
    expirationDate: Date;
    actualRoundNumber: number;
    difficultyLevel: string;
}
