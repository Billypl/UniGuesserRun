import React, { useEffect } from "react";
import { useMap, useMapEvents } from "react-leaflet";
import { MAP_CENTER } from "../Constants";

// Component to recenter map automatically on coords change
export const RecenterMap: React.FC<{ location: [number, number] }> = ({ location }) => {
  const map = useMap();
  useEffect(() => {
    map.setView([location[0], location[1]]);
  }, location);
  return null; // No visual output; marker functionality only
};
