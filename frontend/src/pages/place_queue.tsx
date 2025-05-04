import React, { useEffect, useRef, useState } from 'react'
import Header from '../components/Header'
import styles from '../styles/PlaceQueue.module.scss'
import placeService from '../services/api/placeService'
import { PlaceToCheckDto } from '../models/place/PlaceToCheckDto'
import { Navigate, useNavigate } from 'react-router-dom'
import accountService from '../services/api/accountService'
import { MAP_CENTER, MENU_ROUTE, USER_ROLE_ADMIN, USER_ROLE_MODERATOR } from '../Constants'
import FormField from '../components/FormField'
import { useForm } from 'react-hook-form'
import FormSelect from '../components/FormSelect'
import { MapContainer, TileLayer } from 'react-leaflet'
import { LocationMarker } from '../components/LocationMarker'
import { RecenterMap } from '../components/RecenterMap'
import { SelectMapLocation } from '../components/SelectMapLocation'
import { Coordinates } from '../models/Coordinates'
import { ClickedIcon } from '../components/MarkerIcons'
import { ShowPlaceDto } from '../models/place/ShowPlaceDto'

interface UpdatePlaceFormInputs {
	name: string
	description: string
	alt: string
	difficulty: string
}

const PlaceQueue: React.FC = () => {
	const navigate = useNavigate()
	const [places, setPlaces] = useState<ShowPlaceDto[]>([])
	const [selectedPlace, setSelectedPlace] = useState<ShowPlaceDto | null>(null)
	const [coordinates, setCoordinates] = useState<Coordinates | null>(null)
	const [isEditing, setIsEditing] = useState<boolean>(false)
	let requestSent = useRef(false)

	const {
		register,
		handleSubmit,
		reset,
		formState: { errors },
	} = useForm<UpdatePlaceFormInputs>()

	const getAllPlaces = async () => {
		const result = await placeService.getAllPlacesInQueue()
		setPlaces(result)
		requestSent.current = true
	}

	const showPlace = (place: ShowPlaceDto) => {
		return (
			<div className={styles.place_entry} key={place.id}>
				<p>{place.id}</p>
				<p>
					{place.coordinates.latitude}, {place.coordinates.longitude}
				</p>

				<button className={styles.review_button} onClick={() => setSelectedPlace(place)}>
					Review
				</button>
			</div>
		)
	}

	const showAllPlaces = () => {
		return places.map(place => showPlace(place))
	}

	const showPlaceDetails = () => {
		return (
			<>
				<img className={styles.image} src={selectedPlace?.imageUrl} alt={selectedPlace?.alt} />
				<div className={styles.map}>
					<MapContainer center={MAP_CENTER} zoom={13} scrollWheelZoom={true} style={{ height: '100%', width: '100%' }}>
						<TileLayer
							attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
							url='https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png'
						/>

						{isEditing && coordinates ? (
							<>
								<LocationMarker coords={coordinates} icon={ClickedIcon} label='Place location:' />
								<RecenterMap location={[coordinates.latitude, coordinates.longitude]} />
							</>
						) : (
							selectedPlace && (
								<>
									<LocationMarker
										coords={selectedPlace.coordinates}
										icon={ClickedIcon}
										label='Place location:'
									/>
									<RecenterMap
										location={[
											selectedPlace.coordinates.latitude,
											selectedPlace.coordinates.longitude,
										]}
									/>
								</>
							)
						)}

						{isEditing && <SelectMapLocation selectLocationFunction={setCoordinates} />}
					</MapContainer>
				</div>

				{isEditing && coordinates ? (
					<p>
						{coordinates.latitude}, {coordinates.longitude}
					</p>
				) : (
					<p>
						{selectedPlace?.coordinates.latitude}, {selectedPlace?.coordinates.longitude}
					</p>
				)}

				{isEditing && selectedPlace ? (
					<>
						<form onSubmit={handleSubmit(saveChanges)} className={styles.form}>
							<FormField
								label='Name'
								name='name'
								type='text'
								defaultValue={selectedPlace.name}
								register={register}
								error={errors.name?.message}
							/>

							<FormField
								label='Description'
								name='description'
								type='text'
								defaultValue={selectedPlace.description}
								register={register}
								error={errors.description?.message}
							/>

							<FormField
								label='alt'
								name='alt'
								type='text'
								defaultValue={selectedPlace.alt}
								register={register}
								error={errors.alt?.message}
							/>

							<FormSelect
								label='Difficulty'
								name='difficulty'
								options={[
									{ value: 'easy', label: 'Easy' },
									{ value: 'normal', label: 'Normal' },
									{ value: 'hard', label: 'Hard' },
									{ value: 'ultra-nightmare', label: 'Ultra-Nightmare' },
								]}
								defaultValue={selectedPlace.difficultyLevel}
								register={register}
								error={errors.difficulty?.message}
							/>

							<button type='submit'>Save</button>
							<button onClick={() => cancelChanges()}>Cancel</button>
						</form>
					</>
				) : (
					<>
						<p>{selectedPlace?.name}</p>
						<p>{selectedPlace?.description}</p>
						<button onClick={() => setIsEditing(true)}>Edit</button>
						<button onClick={() => acceptPlace()}>Accept</button>
						<button onClick={() => rejectPlace()}>Reject</button>
					</>
				)}
				<button onClick={goBack}>Go back</button>
			</>
		)
	}

	const acceptPlace = async () => {
		if (!selectedPlace) return
		await placeService.acceptPlaceToCheck(selectedPlace.id)
		showUpdatedPlaces()
	}

	const rejectPlace = async () => {
		if (!selectedPlace) return
		await placeService.rejectPlaceToCheck(selectedPlace.id)
		showUpdatedPlaces()
	}

	const saveChanges = (data: UpdatePlaceFormInputs) => {
		if (!selectedPlace) return

		placeService.updatePlace(
			selectedPlace?.id,
			data.name,
			data.description,
			coordinates ?? selectedPlace.coordinates,
			selectedPlace?.imageUrl,
			data.alt,
			data.difficulty,
			selectedPlace?.authorId
		)

		setIsEditing(false)
	}

	const cancelChanges = () => {
		setIsEditing(false)
		setCoordinates(null)
		reset()
	}

	const goBack = () => {
		setSelectedPlace(null)
		setIsEditing(false)
		setCoordinates(null)
	}

	const showUpdatedPlaces = async () => {
		setSelectedPlace(null)
		await getAllPlaces()
	}

	useEffect(() => {
		if (requestSent.current) return
		getAllPlaces()
	}, [])

	const currentUser = accountService.getCurrentUser()
	if (currentUser === null || (currentUser.role !== USER_ROLE_ADMIN && currentUser.role !== USER_ROLE_MODERATOR)) {
		return <Navigate to={MENU_ROUTE} />
	}

	return (
		<>
			<Header />
			<div className={styles.container}>
				{selectedPlace ? (
					<div className={styles.place_details}>{showPlaceDetails()}</div>
				) : (
					<div className={styles.place_queue}>{showAllPlaces()}</div>
				)}
			</div>
		</>
	)
}

export default PlaceQueue
