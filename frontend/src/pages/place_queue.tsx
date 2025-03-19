import React, { useEffect, useRef, useState } from "react";
import Header from "../components/Header";
import styles from "../styles/PlaceQueue.module.scss";
import placeService from "../services/api/placeService";
import { PlaceToCheckDto } from "../models/place/PlaceToCheckDto";
import { useNavigate } from "react-router-dom";

const PlaceQueue: React.FC = () => {
  const navigate = useNavigate();
  const [places, setPlaces] = useState<PlaceToCheckDto[]>([]);
  const [selectedPlace, setSelectedPlace] = useState<PlaceToCheckDto | null>(null);
  let requestSent = useRef(false);

  const getAllPlaces = async () => {
    const result = await placeService.getAllPlacesInQueue();
    console.log(JSON.stringify(result));
    setPlaces(result);
    requestSent.current = true;
  };

  const showPlace = (place: PlaceToCheckDto) => {
    console.log("showPlace");
    return (
      <div className={styles.place_entry} key={place.id}>
        <p>{place.newPlace.name}</p>
        <p>
          {place.newPlace.coordinates.latitude}, {place.newPlace.coordinates.longitude}
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
        <img src={selectedPlace?.newPlace.imageUrl} alt={selectedPlace?.newPlace.alt} />
        <p>{selectedPlace?.newPlace.name}</p>
        <p>{selectedPlace?.newPlace.description}</p>
        <p>
          {selectedPlace?.newPlace.coordinates.latitude}, {selectedPlace?.newPlace.coordinates.longitude}
        </p>
        <button onClick={() => setSelectedPlace(null)}>Go back</button>
        <button onClick={() => acceptPlace()}>Accept</button>
        <button onClick={() => rejectPlace()}>Reject</button>
      </>
    );
  };

  const acceptPlace = async () => {
    if (!selectedPlace) return;
    await placeService.acceptPlaceToCheck(selectedPlace.id);
    showUpdatedPlaces();
  };
  
  const rejectPlace = async () => {
    if (!selectedPlace) return;
    await placeService.rejectPlaceToCheck(selectedPlace.id);
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

export default PlaceQueue;
