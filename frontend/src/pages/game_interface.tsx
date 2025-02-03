import React, { useState } from "react";
import { MapContainer, TileLayer } from "react-leaflet";
import "leaflet/dist/leaflet.css";

import { SelectMapLocation } from "../components/SelectMapLocation";
import { LocationMarker } from "../components/LocationMarker";

import { TargetIcon, ClickedIcon } from "../components/MarkerIcons";
import { TargetMarker } from "../components/TargetMarker";

import styles from "../styles/Game.module.scss";

interface GameInterfaceProps {
  error: string | null;
  currentRoundNumber: number;
  isLastRound: boolean;
  imageUrl: string;
  guessDistance: number | null;
  targetLatLng: [number, number] | null;
  onConfirmPlayerChoice: (latlng: [number, number]) => void;
  onNextRound: () => void;
  onFinishGame: () => void;
}

const GameInterface: React.FC<GameInterfaceProps> = (props) => {
  const [clickedLatLng, setClickedLatLng] = useState<[number, number] | null>(null);
  const [playerChoiceConfirmed, setPlayerChoiceConfirmed] = useState<boolean>(false);

  const MAP_CENTER: [number, number] = [54.371513, 18.619164];

  const selectLocation = (latlng: [number, number] | null) => {
    if (playerChoiceConfirmed) return; // cant move the marker after confirming your choice
    setClickedLatLng(latlng);
  };

  const confirmPlayerChoice = () => {
    setPlayerChoiceConfirmed(true);
    props.onConfirmPlayerChoice(clickedLatLng!);
  }

  const endRoundButton = () => {
    
    return props.isLastRound ? (
      <button onClick={props.onFinishGame}>Finish game</button>
    ) : (
      <button onClick={nextRound}>Next round</button>
    );
  };

  const nextRound = () => {
    setClickedLatLng(null);
    setPlayerChoiceConfirmed(false);
    props.onNextRound();
  }

  return (
    <div>
      <h1>Round {props.currentRoundNumber + 1}</h1>
      <div className={styles.image_container}>
        <img src={props.imageUrl!} />
      </div>

      {props.error && <p style={{ color: "red" }}>{props.error}</p>}

      {clickedLatLng && !playerChoiceConfirmed && (
        <button onClick={confirmPlayerChoice}>Confirm choice</button>
      )}
      {clickedLatLng && playerChoiceConfirmed && endRoundButton()}
      {props.guessDistance && <h1>Guess distance: {props.guessDistance.toFixed(2)}</h1>}

      {/* Map Section */}
      <div style={{ height: "500px", marginTop: "20px" }}>
        <MapContainer center={MAP_CENTER} zoom={13} scrollWheelZoom={true} style={{ height: "100%", width: "100%" }}>
          <TileLayer
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />

          {clickedLatLng && <LocationMarker latlng={clickedLatLng} icon={ClickedIcon} label="Clicked location:" />}
          {playerChoiceConfirmed && (
            <TargetMarker clickedLatLng={clickedLatLng} targetLatLng={props.targetLatLng} icon={TargetIcon} />
          )}

          <SelectMapLocation selectLocationFunction={selectLocation} />
        </MapContainer>
      </div>
    </div>
  );
};

export default GameInterface;
