import markerIcon from "leaflet/dist/images/marker-icon.png";
import markerShadow from "leaflet/dist/images/marker-shadow.png";
import userMarkerIcon from "../assets/images/user-marker-icon.png";
import targetMarkerIcon from "../assets/images/target-marker-icon.png";
import L from "leaflet";

export const PlayerIcon = L.icon({
  iconUrl: markerIcon,
  shadowUrl: markerShadow,
});

export const ClickedIcon = L.icon({
  iconUrl: userMarkerIcon,
  shadowUrl: markerShadow,
});

export const TargetIcon = L.icon({
  iconUrl: targetMarkerIcon,
  shadowUrl: markerShadow,
});

L.Marker.prototype.options.icon = PlayerIcon;
