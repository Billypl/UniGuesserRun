import React, { useEffect, useState } from "react";
import "leaflet/dist/leaflet.css";
import gameService, { Coordinates } from "../services/api/gameService";
import { useGameContext } from "../hooks/useGameContext";
import { useNavigate } from "react-router-dom";
import GameInterface from "../components/GameInterface";
import { GAME_RESULTS_ROUTE } from "../Constants";

// Latitude: 54.371513, Longitude: 18.619164 <- Gmach Główny
const Game: React.FC = () => {
  const { nickname, difficulty, setScore } = useGameContext();

  const [loading, setLoading] = useState<boolean>(false);
  const [currentRoundNumber, setCurrentRoundNumber] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);

  const [imageUrl, setImage] = useState<string | null>(null);
  const [playerLatLng, setPlayerLatLng] = useState<[number, number] | null>(null);
  const [targetLatLng, setTargetLatLng] = useState<[number, number] | null>(null);
  const [guessDistance, setGuessDistance] = useState<number | null>(null);

  const ROUND_NUMBER: number = 5;
  const navigate = useNavigate();

  useEffect(() => {
    if (!gameService.hasToken()) {
      startGame();
    }
  }, []);

  // Function to make a GET request
  const startGame = async () => {
    setLoading(true);
    setError(null);

    try {
      await gameService.startGame(nickname, difficulty);
      //window.sessionStorage.setItem("token", startData.token);
    } catch (err: any) {
      setError("Failed to fetch data. Please try again later.");
      console.error("Error fetching data:", err);
    } finally {
      setLoading(false);
      startRound(0);
    }
  };

  const startRound = (round: number) => {
    setCurrentRoundNumber(round);
  };

  useEffect(() => {
    if (currentRoundNumber != null && gameService.hasToken()) {
      fetchGuessingPlace();
    }
  }, [currentRoundNumber]);

  const fetchGuessingPlace = async () => {
    setError(null);

    try {
      const guessingPlace = await gameService.getGuessingPlace(currentRoundNumber!);
      setImage(guessingPlace.imageUrl);
    } catch (err: any) {
      setError("Failed to fetch data. Please try again later.");
      console.error("Error fetching data:", err);
    } finally {
      setLoading(false);
    }
  };

  const confirmPlayerChoice = (clickedLatLng: [number, number]) => {
    checkPlayerChoice(clickedLatLng);
  };

  const checkPlayerChoice = async (clickedLatLng: [number, number]) => {
    const coords: Coordinates = {
      latitude: clickedLatLng[0],
      longitude: clickedLatLng[1],
    };
    const roundResult = await gameService.checkGuess(coords);
    setTargetLatLng([roundResult.originalPlace.coordinates.latitude, roundResult.originalPlace.coordinates.longitude]);
    setGuessDistance(roundResult.distanceDifference);
  };

  const isLastRound = (currentRoundNumber: number): boolean => {
    return currentRoundNumber === ROUND_NUMBER - 1;
  };

  const nextRound = () => {
    resetGameState();
    startRound(currentRoundNumber! + 1);
  };

  const finishGame = async () => {
    const response = await gameService.finishGame();
    setScore(response.score);
    navigate(GAME_RESULTS_ROUTE);
  };

  const resetGameState = () => {
    setGuessDistance(null);
  };

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

  return (
    <div style={{ textAlign: "center", marginTop: "20px" }}>
      {loading && <h1>Loading...</h1>}
      {imageUrl && currentRoundNumber != null && (
        <GameInterface
          error={error}
          currentRoundNumber={currentRoundNumber}
          isLastRound={isLastRound(currentRoundNumber)}
          imageUrl={imageUrl}
          guessDistance={guessDistance}
          targetLatLng={targetLatLng}
          onConfirmPlayerChoice={confirmPlayerChoice}
          onNextRound={nextRound}
          onFinishGame={finishGame}
        />
      )}
    </div>
  );
};

export default Game;
