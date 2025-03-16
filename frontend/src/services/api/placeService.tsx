import axios, { AxiosError, AxiosInstance } from "axios";
import { PLACE_API_URL  } from "../../Constants";
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

    async addNewPlaceToQueue (
        name: string,
        description: string,
        coordinates: Coordinates,
        imageUrl: string,
        alt: string,
        difficulty: string
    ) {
        const newPlaceDto: NewPlaceDto = {
            name: name,
            description: description,
            coordinates: coordinates,
            imageUrl: imageUrl,
            alt: alt,
            difficulty: difficulty,
        };
        
        await this.axiosInstance.post("/to_check", newPlaceDto);
    }

    async getAllPlacesInQueue(): Promise<PlaceToCheckDto> {
        return await this.axiosInstance.get("/to_check");
    }

    async addNewPlace (
        name: string,
        description: string,
        coordinates: Coordinates,
        imageUrl: string,
        alt: string,
        difficulty: string
    ): Promise<string | null> 
    {
        try{
            const newPlaceDto: NewPlaceDto = {
                name: name,
                description: description,
                coordinates: coordinates,
                imageUrl: imageUrl,
                alt: alt,
                difficulty: difficulty,
            };

            await this.axiosInstance.post("", newPlaceDto);
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
        await this.axiosInstance.delete("/to_check/reject", {params: {placeId}});
    }

    async acceptPlaceToCheck(placeId: string) {
        await this.axiosInstance.post("/to_check/approve", placeId);
    }
}

export default new PlaceService();
