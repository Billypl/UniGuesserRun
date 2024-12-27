import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import Menu from './pages/menu';
import Game from './pages/game';
import Scoreboard from './pages/scoreboard';

const App = () => {
    return (
        <Router basename="/PartyGame">
            <Routes>
                <Route path="/" element={<Menu />} />
                <Route path="/game" element={<Game />} />
                <Route path="/scoreboard" element={<Scoreboard />} />
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </Router>
    );
};

export default App;