import React, { useState } from "react";
import { MapContainer, TileLayer } from "react-leaflet";
import "leaflet/dist/leaflet.css";

import { SelectMapLocation } from "./SelectMapLocation";
import { LocationMarker } from "./LocationMarker";

import { TargetIcon, ClickedIcon } from "./MarkerIcons";
import { TargetMarker } from "./TargetMarker";

import styles from "../styles/Game.module.scss";
import { MAP_CENTER } from "../Constants";
import { Coordinates } from "../models/Coordinates";

interface GameInterfaceProps {
  error: string | null;
  currentRoundNumber: number;
  isLastRound: boolean;
  imageUrl: string;
  guessDistance: number | null;
  targetLatLng: Coordinates | null;
  onConfirmPlayerChoice: (latlng: Coordinates) => void;
  onNextRound: () => void;
  onFinishGame: () => void;
}

const GameInterface: React.FC<GameInterfaceProps> = (props) => {
  const [clickedLatLng, setClickedLatLng] = useState<Coordinates | null>(null);
  const [playerChoiceConfirmed, setPlayerChoiceConfirmed] = useState<boolean>(false);

  const selectLocation = (coords: Coordinates | null) => {
    if (playerChoiceConfirmed) return; // cant move the marker after confirming your choice
    setClickedLatLng(coords);
  };

  const confirmPlayerChoice = () => {
    setPlayerChoiceConfirmed(true);
    props.onConfirmPlayerChoice(clickedLatLng!);
  }

  const endRoundButton = () => {

    return props.isLastRound ? (
      <button className={styles.end_round_button} onClick={props.onFinishGame}>Finish game</button>
    ) : (
      <button className={styles.end_round_button} onClick={nextRound}>Next round</button>
    );
  };

  const nextRound = () => {
    setClickedLatLng(null);
    setPlayerChoiceConfirmed(false);
    props.onNextRound();
  }

  return (
    <div className={styles.game_interface}>
      <div className={styles.game_header}>
        <h1>Round {props.currentRoundNumber + 1}</h1>
      </div>

      <div className={styles.image_container}>
        <img src={props.imageUrl!} />
      </div>

      {props.error && <p style={{ color: "red" }}>{props.error}</p>}

      {clickedLatLng && !playerChoiceConfirmed && (
        <button className={styles.button} onClick={confirmPlayerChoice}>Confirm</button>
      )}
      {clickedLatLng && playerChoiceConfirmed &&
      <div className={styles.game_controls}>
        {props.guessDistance && <h1 className={styles.distance}>Guess distance: {props.guessDistance.toFixed(2)}</h1>}
        {endRoundButton()}
      </div>}

      {/* Map Section */}
      <div className={styles.map_container}>
        <MapContainer center={MAP_CENTER} zoom={13} scrollWheelZoom={true} style={{ height: "100%", width: "100%" }}>
          <TileLayer
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />

          {clickedLatLng && <LocationMarker coords={clickedLatLng} icon={ClickedIcon} label="Clicked location:" />}
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
