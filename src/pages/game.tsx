import React, { useEffect, useState } from "react";
import { MapContainer, TileLayer, Polyline } from "react-leaflet";
import "leaflet/dist/leaflet.css";
import L from "leaflet";

import markerIcon from "leaflet/dist/images/marker-icon.png";
import markerShadow from "leaflet/dist/images/marker-shadow.png";
import userMarkerIcon from "../assets/images/user-marker-icon.png";
import targetMarkerIcon from "../assets/images/target-marker-icon.png";
import gameService, { Coordinates, StartGameResponse } from "../services/api/gameService";
import { SelectMapLocation } from "../components/SelectMapLocation";
import { LocationMarker } from "../components/LocationMarker";
import { useGameContext } from "../hooks/useGameContext";

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

// Latitude: 54.371513, Longitude: 18.619164 <- Gmach Główny
const Game: React.FC = () => {
  const { nickname, setNickname, difficulty, setDifficulty } = useGameContext();

  const [loading, setLoading] = useState<boolean>(false);
  const [roundNumber, setRoundNumber] = useState<number | null>(null);
  const [imageUrl, setImage] = useState<string | null>(null);

  const [playerLatLng, setPlayerLatLng] = useState<[number, number] | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [clickedLatLng, setClickedLatLng] = useState<[number, number] | null>(null);
  const [playerChoiceConfirmed, setPlayerChoiceConfirmed] = useState<boolean>(false);
  const [guessDistance, setGuessDistance] = useState<number | null>(null);

  const mapCenter: [number, number] = [54.371513, 18.619164];

  // Function to make a GET request
  const startGame = async () => {
    setLoading(true);
    setError(null);

    try {
      const startData = await gameService.startGame(nickname, difficulty);
      window.sessionStorage.setItem("token", startData.token);
    } catch (err: any) {
      setError("Failed to fetch data. Please try again later.");
      console.error("Error fetching data:", err);
    } finally {
      setLoading(false);
      startRound(0);
    }
  };

  const fetchGuessingPlace = async () => {
    setError(null);

    try {
      const guessingPlace = await gameService.getGuessingPlace(roundNumber!);
      setImage(guessingPlace.imageUrl);
    } catch (err: any) {
      setError("Failed to fetch data. Please try again later.");
      console.error("Error fetching data:", err);
    } finally {
      setLoading(false);
    }
  };

  const startRound = (round: number) => {
    console.log("setting round number: " + round);
    setRoundNumber(round);
  };

  useEffect(() => {
    console.log("starting round: " + roundNumber + ", " + sessionStorage.getItem("token"));
    if (roundNumber != null && sessionStorage.getItem("token")) {
      fetchGuessingPlace();
    }
  }, [roundNumber]);

  const nextRound = () => {
    resetGameState();
    startRound(roundNumber! + 1);
  };

  useEffect(() => {
    console.log(sessionStorage.getItem("token"));
    if (!sessionStorage.getItem("token")) {
      console.log("starting game");
      startGame();
    }
  }, []);

  const getCoordinates = () => {
    if (!("geolocation" in navigator)) {
      setError("Geolocation is not supported by your browser.");
      return;
    }
    navigator.geolocation.getCurrentPosition(
      (position) => {
        setPlayerLatLng([position.coords.latitude, position.coords.longitude]);
        setError(null);
      },
      (error) => {
        setError("Unable to retrieve location. Please enable location services.");
        console.error(error);
      },
      {
        enableHighAccuracy: true,
      }
    );
  };

  const confirmPlayerChoice = () => {
    setPlayerChoiceConfirmed(true);
    checkPlayerChoice();
  };

  const checkPlayerChoice = async () => {
    const coords: Coordinates = {
      latitude: clickedLatLng![0],
      longitude: clickedLatLng![1],
    };
    const roundResult = await gameService.checkGuess(coords);
    setGuessDistance(roundResult.distanceDifference);
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

  const displayGame = () => {
    return (
      <div>
        <h1>Round {roundNumber! + 1}</h1>
        <img src={imageUrl!} />
        {error && <p style={{ color: "red" }}>{error}</p>}
        <button onClick={getCoordinates}>Get Coordinates</button>
        {clickedLatLng && !playerChoiceConfirmed && <button onClick={confirmPlayerChoice}>Confirm choice</button>}
        {clickedLatLng && playerChoiceConfirmed && <button onClick={nextRound}>Next round</button>}
        {guessDistance && <h1>Guess distance: {guessDistance.toFixed(2)}</h1>}

        {/* Map Section */}
        <div style={{ height: "500px", marginTop: "20px" }}>
          <MapContainer center={mapCenter} zoom={13} scrollWheelZoom={true} style={{ height: "100%", width: "100%" }}>
            <TileLayer
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />

            <LocationMarker latlng={playerLatLng} icon={PlayerIcon} label="Your location:" />
            <LocationMarker latlng={clickedLatLng} icon={ClickedIcon} label="Clicked location:" />
            {playerChoiceConfirmed && clickedLatLng && (
              <>
                <LocationMarker latlng={getTargetLocation()} icon={TargetIcon} label="Target location:" />
                <Polyline
                  pathOptions={{ color: "black", dashArray: "1 5", weight: 2 }}
                  positions={[clickedLatLng, getTargetLocation()]}
                />
              </>
            )}

            <SelectMapLocation selectLocationFunction={selectLocation} />
          </MapContainer>
        </div>
      </div>
    );
  };

  return (
    <div style={{ textAlign: "center", marginTop: "20px" }}>
      {loading && <h1>Loading...</h1>}
      {imageUrl && displayGame()}
    </div>
  );
};

export default Game;
