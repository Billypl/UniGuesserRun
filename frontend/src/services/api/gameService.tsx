import axios, { AxiosInstance } from "axios";
import { GAME_API_URL, GAME_TOKEN_KEY } from "../../Constants";

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
      baseURL: GAME_API_URL,
      headers: {
        "Content-Type": "application/json",
      },
    });
  }

  async startGame(nickname: string, difficulty: string){
    const startData: StartGameData = {
      nickname: nickname,
      difficulty: difficulty,
    };
    const response = await this.axiosInstance.post<StartGameResponse>("/start", startData);
    window.sessionStorage.setItem(GAME_TOKEN_KEY, response.data.token);
  }

  async checkGuess(guessingCoordinates: Coordinates): Promise<RoundResultDto> {
    const response = await this.axiosInstance.patch<RoundResultDto>("/check", guessingCoordinates, {
      headers: {
        Authorization: `Bearer ${sessionStorage.getItem(GAME_TOKEN_KEY)}`,
      },
    });
    return response.data;
  }

  async getGuessingPlace(roundNumber: number): Promise<GuessingPlaceDto> {
    const response = await this.axiosInstance.get<GuessingPlaceDto>(`/round/${roundNumber}`, {
      headers: {
        Authorization: `Bearer ${sessionStorage.getItem(GAME_TOKEN_KEY)}`,
      },
    });
    return response.data;
  }

  async finishGame(): Promise<SummarizeGameDto> {
    const response = await this.axiosInstance.patch<SummarizeGameDto>("/finish", "", {
      headers: {
        Authorization: `Bearer ${sessionStorage.getItem(GAME_TOKEN_KEY)}`,
      },
    });
    return response.data;
  }

  deleteSession() {
    this.axiosInstance.delete("/delete_session", {
      headers: {
        Authorization: `Bearer ${sessionStorage.getItem(GAME_TOKEN_KEY)}`,
      },
    });
    window.sessionStorage.removeItem(GAME_TOKEN_KEY);
  }

  hasToken(): boolean {
    return !!window.sessionStorage.getItem(GAME_TOKEN_KEY);
  }
}

export default new GameService();
