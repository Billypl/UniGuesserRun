import L from "leaflet";
import React from "react";
import { Marker, Popup } from "react-leaflet";
import { Coordinates } from "../models/Coordinates";

interface LocationMarkerProps {
  coords: Coordinates;
  icon: L.Icon;
  label: string;
}

export const LocationMarker: React.FC<LocationMarkerProps> = ({ coords, icon, label }) => {
  return (
    <Marker position={[coords.latitude, coords.longitude]} icon={icon}>
      <Popup>
        {label} <br />
        Latitude: {coords.latitude.toFixed(6)}, Longitude:{coords.longitude.toFixed(6)}
      </Popup>
    </Marker>
  );
};
