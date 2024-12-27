import React, { useState } from 'react';
import { MapContainer, TileLayer, Marker, Popup, Polyline, useMapEvents } from 'react-leaflet';
import 'leaflet/dist/leaflet.css';
import L from 'leaflet';

// Fix for missing default icon in Leaflet
import markerIcon from 'leaflet/dist/images/marker-icon.png';
import markerShadow from 'leaflet/dist/images/marker-shadow.png';
import userMarkerIcon from '../assets/images/user-marker-icon.png';
import targetMarkerIcon from '../assets/images/target-marker-icon.png';

const PlayerIcon = L.icon({
  iconUrl: markerIcon,
  shadowUrl: markerShadow,
});

const ClickedIcon = L.icon({
  iconUrl: userMarkerIcon,
  shadowUrl: markerShadow,
});

const TargetIcon = L.icon({
  iconUrl: targetMarkerIcon,
  shadowUrl: markerShadow,
});

L.Marker.prototype.options.icon = PlayerIcon;

// Component to handle map click events
const LocationMarker: React.FC<{
  selectLocation: (location: [number, number]) => void;
}> = ({ selectLocation }) => {
  useMapEvents({
    click(e) {
      const { lat, lng } = e.latlng;
      selectLocation([lat, lng]); // Call your custom function
    },
  });
  return null; // No visual output; marker functionality only
};

const DisplayMarker = ({ latlng, icon, label }: { latlng: [number, number] | null, icon: L.Icon, label: string }) => {
  return latlng && (
    <Marker position={latlng} icon={icon}>
      <Popup>
        {label} <br />
        Latitude: {latlng[0].toFixed(6)}, Longitude:{latlng[1].toFixed(6)}
      </Popup>
    </Marker>
  );
}

// Latitude: 54.371513, Longitude: 18.619164 <- GG
const Game: React.FC = () => {
  const [playerLatLng, setPlayerLatLng] = useState<[number, number] | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [clickedLatLng, setClickedLatLng] = useState<[number, number] | null>(null);
  const [playerChoiceConfirmed, setPlayerChoiceConfirmed] = useState<boolean>(false);
  const [guessDistance, setGuessDistance] = useState<number | null>(null);

  const getCoordinates = () => {
    if (!('geolocation' in navigator)) {
      setError('Geolocation is not supported by your browser.');
      return;
    }
    navigator.geolocation.getCurrentPosition(
      (position) => {
        setPlayerLatLng([position.coords.latitude, position.coords.longitude]);
        setError(null);
      },
      (error) => {
        setError('Unable to retrieve location. Please enable location services.');
        console.error(error);
      },
      {
        enableHighAccuracy: true
      }
    );
  };

  const mapCenter: [number, number] = [54.371513, 18.619164];

  const confirmPlayerChoice = () => {
    setPlayerChoiceConfirmed(true);
    setGuessDistance(L.latLng(clickedLatLng!).distanceTo(getTargetLocation()));
  };

  const getTargetLocation = (): [number, number] => {
    return [54.371513, 18.619164]; // TODO: replace with GET request
  };

  const selectLocation = (latlng: [number, number] | null) => {
    if (playerChoiceConfirmed) return; // cant move the marker after confirming your choice
    setClickedLatLng(latlng);
  };

  const resetGameState = () => {
    setClickedLatLng(null);
    setPlayerChoiceConfirmed(false);
    setGuessDistance(null);
  };

  return (
    <div style={{ textAlign: 'center', marginTop: '20px' }}>
      <h1>Device Coordinates</h1>
      {playerLatLng && (
        <div>
          <p>Latitude: <span>{playerLatLng[0].toFixed(6)}</span></p>
          <p>Longitude: <span>{playerLatLng[1].toFixed(6)}</span></p>
        </div>
      )}
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <button onClick={getCoordinates}>Get Coordinates</button>
      {clickedLatLng && !playerChoiceConfirmed && (
        <button onClick={confirmPlayerChoice}>Confirm choice</button>
      )}
      {clickedLatLng && playerChoiceConfirmed && (
        <button onClick={resetGameState}>Reset</button>
      )}
      {guessDistance && (
        <h1>Guess distance: {guessDistance.toFixed(2)}</h1>
      )}

      {/* Map Section */}
      <div style={{ height: '500px', marginTop: '20px' }}>
        <MapContainer
          center={mapCenter}
          zoom={13}
          scrollWheelZoom={true}
          style={{ height: '100%', width: '100%' }}
        >
          <TileLayer
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />

          <DisplayMarker latlng={playerLatLng} icon={PlayerIcon} label="Your location:" />
          <DisplayMarker latlng={clickedLatLng} icon={ClickedIcon} label="Clicked location:" />
          {playerChoiceConfirmed && clickedLatLng && (
            <>
              <DisplayMarker latlng={getTargetLocation()} icon={TargetIcon} label="Target location:" />
              <Polyline
                pathOptions={{ color: 'black', dashArray: '1 5', weight: 2 }}
                positions={[clickedLatLng, getTargetLocation()]} />
            </>
          )}

          <LocationMarker selectLocation={selectLocation} />
        </MapContainer>

      </div>
    </div>
  );
};

export default Game;