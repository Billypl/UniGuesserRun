import React from "react";
import { useMapEvents } from "react-leaflet";

// Component to handle map click events
export const SelectMapLocation: React.FC<{
  selectLocationFunction: (location: [number, number]) => void;
}> = ({ selectLocationFunction: selectLocation }) => {
  useMapEvents({
    click(e) {
      const { lat, lng } = e.latlng;
      selectLocation([lat, lng]); // Call your custom function
    },
  });
  return null; // No visual output; marker functionality only
};
