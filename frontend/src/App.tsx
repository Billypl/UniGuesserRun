import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import Menu from './pages/menu';
import Game from './pages/game';
import Scoreboard from './pages/scoreboard';
import GameResults from './pages/game_results';
import { GameContextProvider } from './components/GameContext';
import { UserContextProvider } from './components/UserContext';
import Register from './pages/register';
import Login from './pages/login';
import { ADD_PLACE_ROUTE, GAME_RESULTS_ROUTE, GAME_ROUTE, LOGIN_ROUTE, REGISTER_ROUTE, SCOREBOARD_ROUTE } from './Constants';
import AddPlace from './pages/add_place';

const App = () => {
    return (
        <UserContextProvider><Router basename="/PartyGame">
            <Routes>
                <Route path="/" element={<GameContextProvider><Menu /></GameContextProvider>}/>
                <Route path={GAME_ROUTE} element={<GameContextProvider><Game /></GameContextProvider>} />
                <Route path={GAME_RESULTS_ROUTE} element={<GameContextProvider><GameResults /></GameContextProvider>} />

                <Route path={SCOREBOARD_ROUTE} element={<Scoreboard />} />
                <Route path={REGISTER_ROUTE} element={<Register />} />
                <Route path={LOGIN_ROUTE} element={<Login />} />
                <Route path={ADD_PLACE_ROUTE} element={<AddPlace />} />
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </Router></UserContextProvider>
    );
};

export default App;