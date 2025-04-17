import axios, { AxiosError, AxiosInstance } from "axios";
import { ACCOUNT_TOKEN_KEY, PLACE_API_URL } from "../../Constants";
import { NewPlaceDto } from "../../models/place/NewPlaceDto";
import { PlaceToCheckDto } from "../../models/place/PlaceToCheckDto";
import { Place } from "../../models/place/Place";
import { Coordinates } from "../../models/Coordinates";
import { UpdatePlaceDto } from "../../models/place/UpdatePlaceDto";

export class PlaceService {
  private axiosInstance: AxiosInstance;

  constructor() {
    this.axiosInstance = axios.create({
      baseURL: PLACE_API_URL,
      headers: {
        "Content-Type": "application/json",
      },
    });
  }

  async getAllPlaces(): Promise<Place[]> {
    const result = await this.axiosInstance.get("");
    return result.data;
  }

  async getPlace(id: string): Promise<Place> {
    const result = await this.axiosInstance.get(`/${id}`);
    return result.data;
  }

  async getAllPlacesInQueue(): Promise<PlaceToCheckDto[]> {
    const result = await this.axiosInstance.get<PlaceToCheckDto[]>("/to_check", {
      headers: {
        Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
      },
    });
    return result.data;
  }

  async addNewPlace(
    name: string,
    description: string,
    coordinates: Coordinates,
    imageUrl: string,
    alt: string,
    difficulty: string,
    skipQueue: boolean
  ): Promise<string | null> {
    try {
      const newPlaceDto: NewPlaceDto = {
        name: name,
        description: description,
        coordinates: coordinates,
        imageUrl: imageUrl,
        alt: alt,
        difficulty: difficulty,
      };

      if (skipQueue) {
        await this.axiosInstance.post("", newPlaceDto, {
          headers: {
            Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
          },
        });
      } else {
        await this.axiosInstance.post("/to_check", newPlaceDto, {
          headers: {
            Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
          },
        });
      }
    } catch (err) {
      const error = err as AxiosError;
      if (!error.response) {
        return "Network error. Please check your connection.";
      }
      return "Invalid place.";
    }

    return null;
  }

  async rejectPlaceToCheck(placeId: string) {
    await this.axiosInstance.delete(`/to_check/reject/${placeId}`, {
      headers: {
        Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
      },
    });
  }

  async acceptPlaceToCheck(placeId: string) {
    await this.axiosInstance.post(`/to_check/approve/${placeId}`, "", {
      headers: {
        Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
      },
    });
  }

  async updatePlace(
    placeId: string,
    name: string,
    description: string,
    coordinates: Coordinates,
    imageUrl: string,
    alt: string,
    difficulty: string,
    authorId: string | null
  ) {
    const updateDto: UpdatePlaceDto = {
      name: name,
      description: description,
      coordinates: coordinates,
      imageUrl: imageUrl,
      alt: alt,
      difficulty: difficulty,
      authorId: authorId,
    };
    await this.axiosInstance.put(`/to_check/${placeId}`, updateDto, {
      headers: {
        Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
      },
    });
  }
}

export default new PlaceService();
