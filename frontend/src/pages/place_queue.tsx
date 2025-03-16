import React, { useEffect, useRef, useState } from "react";
import Header from "../components/Header";
import styles from "../styles/PlaceQueue.module.scss";
import placeService from "../services/api/placeService";
import { PlaceToCheckDto } from "../models/place/PlaceToCheckDto";

const PlaceQueue: React.FC = () => {
    const [places, setPlaces] = useState<PlaceToCheckDto[]>([]);
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
            <div className={styles.record} key={place.id}>
                <p>{place.newPlace.name}</p>
                <p>{place.newPlace.coordinates.latitude}, {place.newPlace.coordinates.longitude}</p>
                {/* <p>{record.difficultyLevel}</p> */}
            </div>
        );
    };

    const showAllPlaces = () => {
        return places.map((place) => showPlace(place));
    };

    useEffect(() => {
        if (requestSent.current) return;
        getAllPlaces();
    }, []);
    
    return (
        <>
            <Header />
            <div className={styles.place_queue}>{showAllPlaces()}</div>
        </>
    );
};

export default PlaceQueue;
