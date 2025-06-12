import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import FormField from "../components/FormField";
import Logo from "../components/Logo";
import { MAP_CENTER, MENU_ROUTE, USER_ROLE_ADMIN, USER_ROLE_MODERATOR } from "../Constants";
import { useUserContext } from "../hooks/useUserContext";
import styles from "../styles/AddPlace.module.scss";
import placeService from "../services/api/placeService";
import { useGeolocation } from "../hooks/useGeolocation";
import accountService from "../services/api/accountService";
import { MapContainer, TileLayer } from "react-leaflet";
import { LocationMarker } from "../components/LocationMarker";
import { ClickedIcon } from "../components/MarkerIcons";
import { SelectMapLocation } from "../components/SelectMapLocation";
import { RecenterMap } from "../components/RecenterMap";
import FormImage from "../components/FormImage";
import FormSelect from "../components/FormSelect";

interface AddPlaceFormInputs {
  name: string;
  description: string;
  imageUrl: string;
  alt: string;
  difficulty: string;
}

const AddPlace: React.FC = () => {
  const navigate = useNavigate();
  const { setUsername } = useUserContext();
  const [error, setError] = useState<string | null>(null);
  const { coordinates, setCoordinates, readCoordinates, geolocationError } = useGeolocation();
  const [placeAdded, setPlaceAdded] = useState<boolean>(false);
  const [image, setImage] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<AddPlaceFormInputs>();

  const addNewPlace = async (data: AddPlaceFormInputs, event?: React.BaseSyntheticEvent) => {
    if (!coordinates) {
      setError("Coordinates not ready yet. Please try again.");
      return;
    }

    const submitter = (event?.nativeEvent as SubmitEvent).submitter as HTMLButtonElement;
    const skipQueue = submitter && submitter.name === "skipQueue";

    const errorMessage = await placeService.addNewPlace(
      data.name,
      data.description,
      coordinates,
      data.imageUrl,
      data.alt,
      data.difficulty,
      skipQueue
    );

    setError(errorMessage);

    if (!errorMessage) {
      // TODO: ok and return to menu OR add another place
      setPlaceAdded(true);

      clearStates();
      reset();
    }
  };

  const clearStates = () => {
    setUsername("");
    setCoordinates(null);
    setError(null);
    setImage(null);
  };

  const canSkipQueue = (): boolean => {
    const userRole = accountService.getCurrentUser()?.role;
    return userRole === USER_ROLE_ADMIN || userRole === USER_ROLE_MODERATOR;
  };

  return placeAdded ? (
    <>
      <p className={styles.success}>Place added successfully!</p>
      <a className={styles.option} onClick={() => navigate(MENU_ROUTE)}>
        <button>Go back to menu</button>
      </a>
      <button className={styles.option} onClick={() => setPlaceAdded(false)}>
        Add another place
      </button>
    </>
  ) : (
    <div className={styles.form_container}>
      <div className={styles.logo_container}>
        <Logo />
      </div>
      <h2 className={styles.header}>Add new place to UniGuesser</h2>

      <div className={styles.map}>
        <MapContainer center={MAP_CENTER} zoom={13} scrollWheelZoom={true} style={{ height: "100%", width: "100%" }}>
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
        Read GPS coordinates
      </button>

      <p className={styles.error}>{geolocationError && "Geolocation error: " + geolocationError}</p>
      <p className={styles.coordinates}>
        {coordinates && `Coordinates: ${coordinates.latitude}, ${coordinates.longitude}`}
      </p>

      <form onSubmit={handleSubmit((data, event) => addNewPlace(data, event))} className={styles.form}>
        <div className={styles.camera_container}>
          <FormImage setImage={setImage} image={image} register={register} />
        </div>

        <FormField label="Name" name="name" type="text" register={register} error={errors.name?.message} />

        <FormField
          label="Description"
          name="description"
          type="text"
          register={register}
          error={errors.description?.message}
        />

        <FormField label="imageUrl" name="imageUrl" type="text" register={register} error={errors.imageUrl?.message} />

        <FormField label="alt" name="alt" type="text" register={register} error={errors.alt?.message} />

        <FormSelect
          label="Difficulty"
          name="difficulty"
          options={[
            { value: "easy", label: "Easy" },
            { value: "normal", label: "Normal" },
            { value: "hard", label: "Hard" },
            { value: "ultra-nightmare", label: "Ultra-Nightmare" },
          ]}
          register={register}
          error={errors.difficulty?.message}
        />

        {error && <p className={styles.error}>{error}</p>}

        <button type="submit" name="addToQueue" className={styles.button}>
          Add place to queue
        </button>
        {canSkipQueue() && (
          <button type="submit" name="skipQueue" className={styles.button}>
            Add place skipping queue
          </button>
        )}
      </form>
    </div>
  );
};

export default AddPlace;
