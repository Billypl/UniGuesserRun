import axios, { AxiosInstance } from "axios";

const API_URL = "https://localhost:7157/api/game";

// Define types for the request and response data
export interface Coordinates {
  longitude: number;
  latitude: number;
}

export interface StartGameData {
  nickname: string;
  difficulty: string;
}

export interface StartGameResponse {
  token: string;
  message: string;
}

export interface GuessingPlaceDto {
  imageUrl: string;
}

export interface RoundResultDto {
  // TODO: original place
  distanceDifference: number;
  roundNumber: number;
}

export class GameService {
  private axiosInstance: AxiosInstance;

  constructor() {
    this.axiosInstance = axios.create({
      baseURL: API_URL,
      headers: {
        "Content-Type": "application/json",
      },
    });
  }

  // Start a new game
  async startGame(nickname: string, difficulty: string): Promise<StartGameResponse> {
    const startData: StartGameData = {
      nickname: nickname,
      difficulty: difficulty,
    };
    const response = await this.axiosInstance.post<StartGameResponse>("/start", startData);
    return response.data;
  }

  // Check a guess
  async checkGuess(guessingCoordinates: Coordinates): Promise<RoundResultDto> {
    const response = await this.axiosInstance.patch<RoundResultDto>("/check", guessingCoordinates, {
      headers: {
        Authorization: `Bearer ${sessionStorage.getItem("token")}`,
      },
    });
    return response.data;
  }

  // Get the guessing place for a specific round
  async getGuessingPlace(roundNumber: number): Promise<GuessingPlaceDto> {
    const response = await this.axiosInstance.get<GuessingPlaceDto>(`/round/${roundNumber}`, {
      headers: {
        Authorization: `Bearer ${sessionStorage.getItem("token")}`,
      },
    });
    return response.data;
  }

  // Finish the game
  async finishGame(): Promise<any> {
    const response = await this.axiosInstance.patch("/finish");
    return response.data;
  }
}

export default new GameService();
