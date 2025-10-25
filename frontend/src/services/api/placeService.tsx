import axios, { AxiosError, AxiosInstance } from 'axios'
import { ACCOUNT_TOKEN_KEY, PLACE_API_URL } from '../../Constants'
import { NewPlaceDto } from '../../models/place/NewPlaceDto'
import { PlaceToCheckDto } from '../../models/place/PlaceToCheckDto'
import { ShowPlaceDto } from '../../models/place/ShowPlaceDto'
import { Coordinates } from '../../models/Coordinates'
import { UpdatePlaceDto } from '../../models/place/UpdatePlaceDto'
import { Difficulty } from '../../models/game/Difficulty'

export class PlaceService {
	private axiosInstance: AxiosInstance

	constructor() {
		this.axiosInstance = axios.create({
			baseURL: PLACE_API_URL,
			headers: {
				'Content-Type': 'application/json',
			},
		})
	}

	async getAllPlaces(): Promise<ShowPlaceDto[]> {
		const result = await this.axiosInstance.get('')
		return result.data
	}

	async getPlace(id: string): Promise<ShowPlaceDto> {
		const result = await this.axiosInstance.get(`/${id}`)
		return result.data
	}

	async getAllPlacesInQueue(): Promise<ShowPlaceDto[]> {
		const result = await this.axiosInstance.get<ShowPlaceDto[]>('/to_check', {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
			},
		})
		return result.data
	}

	async addNewPlace(
		name: string,
		description: string,
		coordinates: Coordinates,
		imageUrl: string,
		alt: string,
		difficulty: Difficulty,
		skipQueue: boolean,
		imageFile?: File | null
	): Promise<string | null> {
		try {
			// Both endpoints now use multipart/form-data
			const formData = new FormData()
			formData.append('Name', name)
			formData.append('Description', description)
			formData.append('Coordinates.Latitude', coordinates.latitude.toString())
			formData.append('Coordinates.Longitude', coordinates.longitude.toString())
			formData.append('ImageUrl', imageUrl || '')
			formData.append('Alt', alt)
			formData.append('Difficulty', difficulty)
			formData.append('ImageType', imageFile ? 'file' : 'url')

			if (imageFile) {
				formData.append('imageFile', imageFile)
			}

			const endpoint = skipQueue ? '' : '/to_check'

			await this.axiosInstance.post(endpoint, formData, {
				headers: {
					Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
					'Content-Type': 'multipart/form-data',
				},
			})
		} catch (err) {
			const error = err as AxiosError
			console.error('Error adding place:', error.response?.data || error.message)
			if (!error.response) {
				return 'Network error. Please check your connection.'
			}
			return 'Invalid place.'
		}

		return null
	}

	private convertFileToBase64(file: File): Promise<string> {
		return new Promise((resolve, reject) => {
			const reader = new FileReader()
			reader.onloadend = () => {
				resolve(reader.result as string)
			}
			reader.onerror = reject
			reader.readAsDataURL(file)
		})
	}

	async rejectPlaceToCheck(placeId: string) {
		await this.axiosInstance.delete(`/to_check/reject/${placeId}`, {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
			},
		})
	}

	async acceptPlaceToCheck(placeId: string) {
		await this.axiosInstance.post(`/to_check/approve/${placeId}`, '', {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
			},
		})
	}

	async updatePlace(
		placeId: string,
		name: string,
		description: string,
		coordinates: Coordinates,
		imageUrl: string,
		alt: string,
		difficulty: Difficulty
	) {
		const updateDto: UpdatePlaceDto = {
			name: name,
			description: description,
			coordinates: coordinates,
			imageUrl: imageUrl,
			alt: alt,
			difficulty: difficulty,
		}
		await this.axiosInstance.put(`/${placeId}`, updateDto, {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
			},
		})
	}

	async deletePlace(placeId: string) {
		await this.axiosInstance.delete(`/${placeId}`, {
			headers: {
				Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
			},
		})
	}
}

export default new PlaceService()
