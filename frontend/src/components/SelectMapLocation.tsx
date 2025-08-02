import React from "react";
import { useMapEvents } from "react-leaflet";
import { Coordinates } from "../models/Coordinates";

// Component to handle map click events
export const SelectMapLocation: React.FC<{
  selectLocationFunction: (location: Coordinates) => void;
}> = ({ selectLocationFunction: selectLocation }) => {
  useMapEvents({
    click(e) {
      const coords: Coordinates = {
        latitude: e.latlng.lat,
        longitude: e.latlng.lng
      }
      selectLocation(coords); // Call your custom function
    },
  });
  return null; // No visual output; marker functionality only
};
