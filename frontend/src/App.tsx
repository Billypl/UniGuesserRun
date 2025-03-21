import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import Menu from './pages/menu';
import Game from './pages/game';
import Scoreboard from './pages/scoreboard';
import GameResults from './pages/game_results';
import { GameContextProvider } from './components/GameContext';
import { UserContextProvider } from './components/UserContext';
import Register from './pages/register';
import Login from './pages/login';

const App = () => {
    return (
        <UserContextProvider><Router basename="/PartyGame">
            <Routes>
                <Route path="/" element={<GameContextProvider><Menu /></GameContextProvider>}/>
                <Route path="/game" element={<GameContextProvider><Game /></GameContextProvider>} />
                <Route path="/game_results" element={<GameContextProvider><GameResults /></GameContextProvider>} />

                <Route path="/scoreboard" element={<Scoreboard />} />
                <Route path="/register" element={<Register />} />
                <Route path="/login" element={<Login />} />
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </Router></UserContextProvider>
    );
};

export default App;