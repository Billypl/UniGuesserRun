# UniguesserRun
![](frontend/src/assets/promotion/main-menu.png)
UniguesserRun (UGR) is a geoguessing game where players guess a location based on a random photo. It features two gameplay modes:
- **Map Guessing Mode**: Players are given a random image of a location and must place a marker on an interactive map to indicate where they think the photo was taken from. The accuracy of their guess is scored based on the distance from the actual location.

- **Running Mode**: Instead of placing a marker, players must physically reach the correct location using real-world navigation. The game provides feedback in a hot/cold-style system, indicating whether the player is moving closer or farther away. GPS tracking verifies if the player has reached the correct spot.

The game is fully dockerized using `docker-compose` for easy deployment.

## Features
- **Two gameplay modes**: Guess on a map or reach the location physically.
- **Real-world exploration**: Players navigate using GPS.
- **Live tracking**: Uses GPS to validate movement in running mode.
- **Custom location creation**: Players can submit their own locations, which must be approved by an admin.
- **Multiplayer support** (Planned): Play with friends in real-time.
- **PWA integration**: Play directly in a web browser and on mobile!

## Tech Stack
- **Frontend**: React (TypeScript, PWA)
- **Backend**: ASP.NET Core (C#)
- **Database**: MongoDB
- **Containerization**: Docker (via `docker-compose`)
- **Mapping & Localization**: Google Maps API / OpenStreetMap

### Running with Docker
```sh
git clone https://github.com/YourUsername/UniguesserRun.git
cd UniguesserRun/docker
# full app
docker-compose up
# without backend
docker-compose.dev.yaml
```

## Contact
For any inquiries, feel free to open an issue or reach out to us via email.

