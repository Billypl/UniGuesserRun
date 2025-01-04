import L from "leaflet";
import { LocationMarker } from "./LocationMarker";
import React from "react";
import { Polyline } from "react-leaflet";

interface TargetMarkerProps {
  clickedLatLng: [number, number] | null;
  targetLatLng: [number, number] | null;
  icon: L.Icon;
}

export const TargetMarker: React.FC<TargetMarkerProps> = ({
  clickedLatLng,
  targetLatLng,
  icon
}) => {
  return (
    clickedLatLng &&
    targetLatLng && (
      <>
        <LocationMarker latlng={targetLatLng} icon={icon} label="Target location:" />
        <Polyline
          pathOptions={{ color: "black", dashArray: "1 5", weight: 2 }}
          positions={[clickedLatLng, targetLatLng]}
        />
      </>
    )
  );
};
