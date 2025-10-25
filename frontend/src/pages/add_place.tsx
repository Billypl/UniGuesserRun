import React, { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import FormField from '../components/FormField'
import Header from '../components/Header'
import { MAP_CENTER, MENU_ROUTE, USER_ROLE_ADMIN, USER_ROLE_MODERATOR } from '../Constants'
import { useUserContext } from '../hooks/useUserContext'
import styles from '../styles/AddPlace.module.scss'
import placeService from '../services/api/placeService'
import { useGeolocation } from '../hooks/useGeolocation'
import accountService from '../services/api/accountService'
import { MapContainer, TileLayer } from 'react-leaflet'
import { LocationMarker } from '../components/LocationMarker'
import { ClickedIcon } from '../components/MarkerIcons'
import { SelectMapLocation } from '../components/SelectMapLocation'
import { RecenterMap } from '../components/RecenterMap'
import FormImage from '../components/FormImage'
import FormSelect from '../components/FormSelect'
import { UserRole } from '../models/account/UserRole'
import { Difficulty } from '../models/game/Difficulty'

interface AddPlaceFormInputs {
	name: string
	description: string
	imageUrl: string
	alt: string
	difficulty: Difficulty
	latitude: number
	longitude: number
}

type ImageInputMode = 'url' | 'file'

const AddPlace: React.FC = () => {
	const navigate = useNavigate()
	const { setUsername } = useUserContext()
	const [error, setError] = useState<string | null>(null)
	const { coordinates, setCoordinates, readCoordinates, geolocationError } = useGeolocation()
	const [placeAdded, setPlaceAdded] = useState<boolean>(false)
	const [image, setImage] = useState<string | null>(null)
	const [imageFile, setImageFile] = useState<File | null>(null)
	const [imageInputMode, setImageInputMode] = useState<ImageInputMode>('url')

	const {
		register,
		handleSubmit,
		setValue,
		reset,
		formState: { errors },
	} = useForm<AddPlaceFormInputs>()

	useEffect(() => {
		setValue('latitude', coordinates?.latitude || 0)
		setValue('longitude', coordinates?.longitude || 0)
	}, [coordinates, setValue])

	const handleChangeCoordinateField = (field: 'latitude' | 'longitude', value: string) => {
		const parsedValue = parseFloat(value)
		if (isNaN(parsedValue)) {
			return
		}
		if (coordinates) {
			setCoordinates({ ...coordinates, [field]: parsedValue })
		}
	}

	const addNewPlace = async (data: AddPlaceFormInputs, event?: React.BaseSyntheticEvent) => {
		if (!coordinates) {
			setError('Coordinates not ready yet. Please try again.')
			return
		}

		console.log(data,event)

		// Validate that user provided either URL or file
		if (imageInputMode === 'url' && !data.imageUrl) {
			setError('Please provide an image URL.')
			return
		}

		if (imageInputMode === 'file' && !imageFile && !image) {
			setError('Please upload or capture an image.')
			return
		}

		const submitter = (event?.nativeEvent as SubmitEvent).submitter as HTMLButtonElement
		const skipQueue = submitter && submitter.name === 'skipQueue'

		// Use imageFile if in file mode, otherwise use imageUrl from form
		const finalImageUrl = imageInputMode === 'file' ? '' : data.imageUrl

		const errorMessage = await placeService.addNewPlace(
			data.name,
			data.description,
			coordinates,
			finalImageUrl,
			data.alt,
			data.difficulty,
			skipQueue,
			imageInputMode === 'file' ? imageFile : null
		)

		setError(errorMessage)

		if (!errorMessage) {
			// TODO: ok and return to menu OR add another place
			setPlaceAdded(true)

			clearStates()
			reset()
		}
	}

	const clearStates = () => {
		setUsername('')
		setCoordinates(null)
		setError(null)
		setImage(null)
		setImageFile(null)
	}

	const canSkipQueue = (): boolean => {
		const userRole = accountService.getCurrentUser()?.role
		return userRole === UserRole.ADMIN || userRole === UserRole.MODERATOR
	}

	return placeAdded ? (
		<>
			<Header />
			<div className={styles.page}>
				<div className={styles.container}>
					<div className={styles.form_container}>
						<div className={styles.success_container}>
							<p className={styles.success}>✓ Place added successfully!</p>
							<div className={styles.buttons}>
								<button className={styles.button} onClick={() => navigate(MENU_ROUTE)}>
									Go back to menu
								</button>
								<button className={styles.button} onClick={() => setPlaceAdded(false)}>
									Add another place
								</button>
							</div>
						</div>
					</div>
				</div>
			</div>
		</>
	) : (
		<>
			<Header />
			<div className={styles.page}>
				<div className={styles.container}>
					<div className={styles.form_container}>
						<h2 className={styles.header}>Add New Place to UniGuesser</h2>

						<div className={styles.map}>
							<MapContainer
								center={MAP_CENTER}
								zoom={13}
								scrollWheelZoom={true}
								style={{ height: '100%', width: '100%' }}
							>
								<TileLayer
									attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
									url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
								/>

								{coordinates && (
									<>
										<LocationMarker
											coords={coordinates}
											icon={ClickedIcon}
											label="Clicked location:"
										/>
										<RecenterMap location={[coordinates.latitude, coordinates.longitude]} />
									</>
								)}

								<SelectMapLocation selectLocationFunction={setCoordinates} />
							</MapContainer>
						</div>

						<button className={styles.button} onClick={readCoordinates}>
							📍 Read GPS coordinates
						</button>

						{geolocationError && (
							<p className={styles.error}>Geolocation error: {geolocationError}</p>
						)}

						<form
							onSubmit={handleSubmit((data, event) => addNewPlace(data, event))}
							className={styles.form}
						>
							<div className={styles.coordinates_section}>
								<FormField
									label="Latitude"
									name="latitude"
									type="number"
									onBlur={(e) => handleChangeCoordinateField('latitude', e.target.value)}
									defaultValue={coordinates?.latitude.toString() || ''}
									register={register}
									error={errors.latitude?.message}
								/>
								<FormField
									label="Longitude"
									name="longitude"
									type="number"
									onBlur={(e) => handleChangeCoordinateField('longitude', e.target.value)}
									defaultValue={coordinates?.longitude.toString() || ''}
									register={register}
									error={errors.longitude?.message}
								/>
							</div>

							<FormField
								label="Name"
								name="name"
								type="text"
								register={register}
								error={errors.name?.message}
							/>

							<FormField
								label="Description"
								name="description"
								type="text"
								register={register}
								error={errors.description?.message}
							/>

							<div className={styles.image_mode_toggle}>
								<button
									type="button"
									className={`${styles.toggle_button} ${
										imageInputMode === 'url' ? styles.active : ''
									}`}
									onClick={() => setImageInputMode('url')}
								>
									URL
								</button>
								<button
									type="button"
									className={`${styles.toggle_button} ${
										imageInputMode === 'file' ? styles.active : ''
									}`}
									onClick={() => setImageInputMode('file')}
								>
									Upload File
								</button>
							</div>

							{imageInputMode === 'url' ? (
								<FormField
									label="Image URL"
									name="imageUrl"
									type="text"
									register={register}
									error={errors.imageUrl?.message}
								/>
							) : (
								<div className={styles.camera_container}>
									<FormImage
										setImage={setImage}
										image={image}
										register={register}
										setImageFile={setImageFile}
									/>
								</div>
							)}

							<FormField
								label="Alt Text"
								name="alt"
								type="text"
								register={register}
								error={errors.alt?.message}
							/>

							<FormSelect
								label="Difficulty"
								name="difficulty"
								options={[
									{ value: 'easy', label: 'Easy' },
									{ value: 'normal', label: 'Normal' },
									{ value: 'hard', label: 'Hard' },
									{ value: 'ultra-nightmare', label: 'Ultra-Nightmare' },
								]}
								register={register}
								error={errors.difficulty?.message}
							/>

							{error && <p className={styles.error}>{error}</p>}

							<button type="submit" name="addToQueue" className={styles.button}>
								Add place to queue
							</button>
							{canSkipQueue() && (
								<button
									type="submit"
									name="skipQueue"
									className={`${styles.button} ${styles.secondary}`}
								>
									Add place skipping queue
								</button>
							)}
						</form>
					</div>
				</div>
			</div>
		</>
	)
}

export default AddPlace
