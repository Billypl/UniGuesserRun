export interface GameSession {
    id: string;
    rounds: number;
    expirationDate: Date;
    userId?: string;
    player?: string;
    difficulty: string;
    gameMode: string;
}
