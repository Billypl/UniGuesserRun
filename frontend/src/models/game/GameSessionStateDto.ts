export interface GameSessionStateDto {
    id: string;
    publicId: string;
    expirationDate: Date;
    actualRoundNumber: number;
    difficultyLevel: string;
}
