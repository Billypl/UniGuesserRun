import axios, { AxiosError, AxiosInstance } from "axios";
import { ACCOUNT_TOKEN_KEY, PLACE_API_URL } from "../../Constants";
import { NewPlaceDto } from "../../models/place/NewPlaceDto";
import { PlaceToCheckDto } from "../../models/place/PlaceToCheckDto";
import { Place } from "../../models/place/Place";
import { Coordinates } from "../../models/Coordinates";

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
    return await this.axiosInstance.get("");
  }

  async getPlace(id: string): Promise<Place> {
    return await this.axiosInstance.get(`/${id}`);
  }

  async getAllPlacesInQueue(): Promise<PlaceToCheckDto[]> {
    const result = await this.axiosInstance.get<PlaceToCheckDto[]>("/to_check");
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
    await this.axiosInstance.delete("/to_check/reject", { params: { placeId } });
  }

  async acceptPlaceToCheck(placeId: string) {
    await this.axiosInstance.post("/to_check/approve", '', { params: { placeId } });
  }
}

export default new PlaceService();
