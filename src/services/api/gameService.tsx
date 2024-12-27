import axios, { AxiosInstance } from 'axios';

const API_URL = 'http://localhost:3000/api/game';

// Define types for the request and response data
export interface Coordinates {
  x: number;
  y: number;
}

export interface StartGameResponse {
  token: string;
  message: string;
}

export interface GuessingPlaceDto {
  id: number;
  description: string;
  coordinates: Coordinates;
}

export class GameService {
  private axiosInstance: AxiosInstance;

  constructor() {
    this.axiosInstance = axios.create({
      baseURL: API_URL,
      headers: {
        'Content-Type': 'application/json',
      },
    });
  }

  // Start a new game
  async startGame(): Promise<StartGameResponse> {
    const response = await this.axiosInstance.get<StartGameResponse>('/start');
    return response.data;
  }

  // Check a guess
  async checkGuess(guessingCoordinates: Coordinates): Promise<any> {
    const response = await this.axiosInstance.patch('/check', guessingCoordinates);
    return response.data;
  }

  // Get the guessing place for a specific round
  async getGuessingPlace(roundNumber: number): Promise<GuessingPlaceDto> {
    const response = await this.axiosInstance.get<GuessingPlaceDto>(`/round/${roundNumber}`);
    return response.data;
  }

  // Finish the game
  async finishGame(): Promise<any> {
    const response = await this.axiosInstance.patch('/finish');
    return response.data;
  }
}

export default new GameService();
