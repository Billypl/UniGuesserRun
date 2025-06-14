import { useEffect, useState } from "react";
import { Coordinates } from "../models/Coordinates";

export const useGeolocation = () => {
    const [coordinates, setCoordinates] = useState<Coordinates | null>(null);
    const [geolocationError, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!navigator.geolocation) {
            setError("Geolocation is not supported by your browser.");
            return;
        }

        readCoordinates();
    }, []);

    const readCoordinates = () => {
        navigator.geolocation.getCurrentPosition(
            (position) => {
                setCoordinates({
                    latitude: position.coords.latitude,
                    longitude: position.coords.longitude,
                });
                setError(null);
            },
            (err) => {
                if (err.code === 3) {
                    console.warn("Geolocation timeout, retrying...");
                } else {
                    setError(err.message);
                }
            },
            {
                enableHighAccuracy: true,
                maximumAge: 0,
                timeout: 5000,
            }
        );
    }

    return { coordinates, setCoordinates, readCoordinates, geolocationError };
};
