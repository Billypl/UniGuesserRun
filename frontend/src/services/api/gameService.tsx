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

interface Place {
  coordinates: Coordinates;
  //imageUrl: string;
  //alt: string;
}

export interface RoundResultDto {
  originalPlace: Place;
  distanceDifference: number;
  roundNumber: number;
}

export interface SummarizeGameDto {
  // TODO: round list
  score: number;
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

  async startGame(nickname: string, difficulty: string): Promise<StartGameResponse> {
    const startData: StartGameData = {
      nickname: nickname,
      difficulty: difficulty,
    };
    const response = await this.axiosInstance.post<StartGameResponse>("/start", startData);
    return response.data;
  }

  async checkGuess(guessingCoordinates: Coordinates): Promise<RoundResultDto> {
    const response = await this.axiosInstance.patch<RoundResultDto>("/check", guessingCoordinates, {
      headers: {
        Authorization: `Bearer ${sessionStorage.getItem("token")}`,
      },
    });
    return response.data;
  }

  async getGuessingPlace(roundNumber: number): Promise<GuessingPlaceDto> {
    const response = await this.axiosInstance.get<GuessingPlaceDto>(`/round/${roundNumber}`, {
      headers: {
        Authorization: `Bearer ${sessionStorage.getItem("token")}`,
      },
    });
    return response.data;
  }

  async finishGame(): Promise<SummarizeGameDto> {
    const response = await this.axiosInstance.patch<SummarizeGameDto>("/finish", "", {
      headers: {
        Authorization: `Bearer ${sessionStorage.getItem("token")}`,
      },
    });
    return response.data;
  }

  deleteSession() {
    this.axiosInstance.delete("/delete_session", {
      headers: {
        Authorization: `Bearer ${sessionStorage.getItem("token")}`,
      },
    });
  }
}

export default new GameService();
