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

        const watchId = navigator.geolocation.watchPosition(
            (position) => {
                setCoordinates({
                    latitude: position.coords.latitude,
                    longitude: position.coords.longitude,
                });
                setError(null);
            },
            (err) => {
                setError(err.message);
            },
            {
                enableHighAccuracy: true,
                maximumAge: 0,
                timeout: 5000,
            }
        );

        return () => navigator.geolocation.clearWatch(watchId);
    }, []);

    return { coordinates, geolocationError };
};
