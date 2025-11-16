# UniGuesser - Setup Guide

## Quick Start

### 1. Find Your Local IP Address

**Windows:**

```powershell
ipconfig
```

Look for "IPv4 Address" under your active network adapter (usually starts with 192.168.x.x)

**Linux/Mac:**

```bash
ifconfig
```

### 2. Configure Environment

**Step 2a:** Edit the **root `.env` file** (in `UniGuesserRun/.env`):

```env
HOST_IP=YOUR_IP_HERE  # e.g., 192.168.0.10
BACKEND_PORT=5223
FRONTEND_PORT=3000
```

**Step 2b:** Edit `frontend/.env` to match:

```env
VITE_HOST_IP=YOUR_IP_HERE  # MUST match HOST_IP above
VITE_API_TARGET=https://YOUR_IP_HERE:5223
VITE_BACKEND_PORT=5223
```

**Example for both files:**

Root `.env`:

```env
HOST_IP=192.168.0.10
BACKEND_PORT=5223
FRONTEND_PORT=3000
```

Frontend `.env`:

```env
VITE_HOST_IP=192.168.0.10
VITE_API_TARGET=https://192.168.0.10:5223
VITE_BACKEND_PORT=5223
```

**⚠️ Important:** Keep these two files synchronized! When you change IP in root `.env`, update `frontend/.env` too.

**Docker users:** When running with Docker Compose from root directory, only the root `.env` is needed (frontend/.env can be deleted).

### 3. Optional: Additional CORS Origins

If you need to allow additional origins (e.g., another developer's IP), you can add them to `backend/UniGuesser.API/appsettings.Development.json`:

```json
"CorsSettings": {
  "AllowedOrigins": "https://192.168.0.99:3000,https://other-domain.com"
}
```

But for most cases, the `.env` file is all you need!

### 4. Run the Application

**Frontend:**

```powershell
cd frontend
npm run dev
```

**Backend:**
Run the backend project from Visual Studio or:

```powershell
cd backend/UniGuesser.API
dotnet run
```

## Access URLs

After starting both servers:

### Localhost Access:

- Frontend: `https://localhost:3000`
- Backend: `https://localhost:5223`

### Network Access (from other devices):

- Frontend: `https://YOUR_IP_HERE:3000` (e.g., `https://192.168.0.10:3000`)
- Backend: `https://YOUR_IP_HERE:5223` (e.g., `https://192.168.0.10:5223`)

## How It Works

The application now **automatically detects** which URL you're using:

- If you open `https://localhost:3000` → API calls go to `https://localhost:5223`
- If you open `https://YOUR_IP_HERE:3000` → API calls go to `https://YOUR_IP_HERE:5223`

No manual switching needed! The frontend automatically uses the same hostname as your browser.

## Certificate Warnings

When accessing via HTTPS for the first time, your browser will show a security warning because the certificate is self-signed. This is normal for development:

- Click "Advanced" → "Proceed anyway" (or similar)
- On mobile: Accept the certificate warning

## Troubleshooting

### Mixed Content Errors

Make sure both frontend and backend use HTTPS (not HTTP).

### Can't Access from Phone

1. Make sure your phone is on the **same WiFi network**
2. Check if firewall is blocking ports 3000 and 5223
3. Try disabling Windows Firewall temporarily to test

### IP Changed

If your IP address changes (common with DHCP):

1. Run `ipconfig` again to get new IP
2. Update `frontend/.env` with new IP
3. Update `backend/UniGuesser.API/appsettings.Development.json` CORS and ServerIp
4. Restart both frontend and backend

## Production Build

For production, update environment variables accordingly and build:

```powershell
cd frontend
npm run build
```
