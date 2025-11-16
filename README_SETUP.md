# UniGuesser

A location guessing game built with React (Vite) frontend and ASP.NET Core backend.

## Quick Setup

1. **Copy the environment template:**

   ```bash
   cp .env.example .env
   ```

2. **Set your IP address** in `.env`:

   ```env
   HOST_IP=192.168.0.10  # Replace with your IP (use 'ipconfig' or 'ifconfig')
   ```

3. **Run the application:**
   - Frontend: `cd frontend && npm install && npm run dev`
   - Backend: Open `backend/UniGuesser.sln` in Visual Studio or run `dotnet run`

📖 **Full setup guide:** See [SETUP.md](./SETUP.md) for detailed instructions.

## Project Structure

```
├── .env                    # Shared environment config (not in git)
├── .env.example            # Template for .env
├── frontend/               # React + Vite frontend
├── backend/                # ASP.NET Core backend
├── docker/                 # Docker configurations
└── SETUP.md               # Detailed setup guide
```

## Access URLs

- Frontend: `https://localhost:3000` or `https://YOUR_IP:3000`
- Backend API: `https://localhost:5223` or `https://YOUR_IP:5223`

## Requirements

- Node.js 18+
- .NET 8 SDK
- PostgreSQL
