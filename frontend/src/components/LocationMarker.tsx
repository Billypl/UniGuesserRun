import L from "leaflet";
import React from "react";
import { Marker, Popup } from "react-leaflet";

export const LocationMarker = ({ latlng, icon, label }: { latlng: [number, number] | null; icon: L.Icon; label: string; }) => {
  return (
    latlng && (
      <Marker position={latlng} icon={icon}>
        <Popup>
          {label} <br />
          Latitude: {latlng[0].toFixed(6)}, Longitude:{latlng[1].toFixed(6)}
        </Popup>
      </Marker>
    )
  );
};
