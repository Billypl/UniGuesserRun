import React, { useEffect, useRef, useState } from "react";
import Header from "../components/Header";
import styles from "../styles/Places.module.scss";
import placeService from "../services/api/placeService";
import { Navigate } from "react-router-dom";
import accountService from "../services/api/accountService";
import { MENU_ROUTE, USER_ROLE_ADMIN } from "../Constants";
import { Place } from "../models/place/Place";

const Places: React.FC = () => {
  const [places, setPlaces] = useState<Place[]>([]);
  const [selectedPlace, setSelectedPlace] = useState<Place | null>(null);
  let requestSent = useRef(false);

  const getAllPlaces = async () => {
    const result = await placeService.getAllPlaces();
    setPlaces(result);
    requestSent.current = true;
  };

  const showPlace = (place: Place) => {
    return (
      <div className={styles.place_entry} key={place.name}>
        <p>{place.name}</p>
        <p>
          {place.coordinates.latitude}, {place.coordinates.longitude}
        </p>

        <button className={styles.review_button} onClick={() => setSelectedPlace(place)}>
          Review
        </button>
      </div>
    );
  };

  const showAllPlaces = () => {
    return places.map((place) => showPlace(place));
  };

  const showPlaceDetails = () => {
    return (
      <>
        <img src={selectedPlace?.imageUrl} alt={selectedPlace?.alt} />
        <p>{selectedPlace?.name}</p>
        <p>{selectedPlace?.description}</p>
        <p>
          {selectedPlace?.coordinates.latitude}, {selectedPlace?.coordinates.longitude}
        </p>
        <button onClick={() => setSelectedPlace(null)}>Go back</button>
        <button onClick={() => deletePlace()}>Delete</button>
      </>
    );
  };

  const deletePlace = async () => {
    if (!selectedPlace) return;
    // TODO: Implement deleting place
    // await placeService.rejectPlaceToCheck(selectedPlace.id);
    showUpdatedPlaces();
  }

  const showUpdatedPlaces = async () => {
    setSelectedPlace(null);
    await getAllPlaces();
  }

  useEffect(() => {
    if (requestSent.current) return;
    getAllPlaces();
  }, []);

  const currentUser = accountService.getCurrentUser();
  if (currentUser === null || currentUser.role !== USER_ROLE_ADMIN) {
    return <Navigate to={MENU_ROUTE} />;
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
  );
};

export default Places;
