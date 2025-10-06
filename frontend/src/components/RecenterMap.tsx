import React, { useEffect } from "react";
import { useMap } from "react-leaflet";

// Component to recenter map automatically on coords change
export const RecenterMap: React.FC<{ location: [number, number] }> = ({ location }) => {
  const map = useMap();
  useEffect(() => {
    map.setView([location[0], location[1]]);
  }, location);
  return null; // No visual output; marker functionality only
};
