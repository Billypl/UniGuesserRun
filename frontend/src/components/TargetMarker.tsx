import L from "leaflet";
import { LocationMarker } from "./LocationMarker";
import React from "react";
import { Polyline } from "react-leaflet";
import { Coordinates } from "../models/Coordinates";

interface TargetMarkerProps {
  clickedLatLng: Coordinates | null;
  targetLatLng: Coordinates | null;
  icon: L.Icon;
}

export const TargetMarker: React.FC<TargetMarkerProps> = ({ clickedLatLng, targetLatLng, icon }) => {
  return (
    clickedLatLng &&
    targetLatLng && (
      <>
        <LocationMarker coords={targetLatLng} icon={icon} label="Target location:" />
        <Polyline
          pathOptions={{ color: "black", dashArray: "1 5", weight: 2 }}
          positions={[
            [clickedLatLng.latitude, clickedLatLng.longitude],
            [targetLatLng.latitude, targetLatLng.longitude],
          ]}
        />
      </>
    )
  );
};
