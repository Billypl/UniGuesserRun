import L from "leaflet";
import React from "react";
import { Marker, Popup } from "react-leaflet";

interface LocationMarkerProps {
  latlng: [number, number];
  icon: L.Icon;
  label: string;
}

export const LocationMarker: React.FC<LocationMarkerProps> = ({ latlng, icon, label }) => {
  return (
    <Marker position={latlng} icon={icon}>
      <Popup>
        {label} <br />
        Latitude: {latlng[0].toFixed(6)}, Longitude:{latlng[1].toFixed(6)}
      </Popup>
    </Marker>
  );
};
