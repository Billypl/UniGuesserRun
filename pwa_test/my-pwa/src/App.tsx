import React, { useState } from 'react';

const App: React.FC = () => {
  const [latitude, setLatitude] = useState<string | null>(null);
  const [longitude, setLongitude] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const getCoordinates = () => {
    if ('geolocation' in navigator) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          setLatitude(position.coords.latitude.toFixed(6));
          setLongitude(position.coords.longitude.toFixed(6));
          setError(null);
        },
        (error) => {
          setError('Unable to retrieve location. Please enable location services.');
          console.error(error);
        }
      );
    } else {
      setError('Geolocation is not supported by your browser.');
    }
  };

  return (
    <div style={{ textAlign: 'center', marginTop: '20px' }}>
      <h1>Device Coordinates</h1>
      <div>
        <p>Latitude: <span>{latitude ?? '-'}</span></p>
        <p>Longitude: <span>{longitude ?? '-'}</span></p>
        {error && <p style={{ color: 'red' }}>{error}</p>}
      </div>
      <button onClick={getCoordinates}>Get Coordinates</button>
    </div>
  );
};

export default App;