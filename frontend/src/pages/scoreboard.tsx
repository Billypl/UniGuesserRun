import React, { useEffect, useState } from 'react';
import { useNavigate } from "react-router-dom";
import scoreboardService, { FinishedGame, ScoreboardService } from '../services/api/scoreboardService';
import styles from "../styles/Scoreboard.module.scss";
import { MENU_ROUTE } from '../Constants';

const Scoreboard: React.FC = () => {
    const navigate = useNavigate();
    const [records, setRecords] = useState<FinishedGame[]>([]);
    let requestSent = false;

    const showRecord = (record: FinishedGame) => {
        console.log("showRecord");
        return (
            <div className={styles.record} key={record.id}>
                <p>{record.nickname}</p>
                <p>{record.finalScore}</p>
                {/* <p>{record.difficultyLevel}</p> */}
            </div>
        )
    }

    const showAllRecords = () => {
        return (
            records.map((record) => (
                showRecord(record)
            ))        
        );
    }

    const getAllRecords = async () => {
        const result = await scoreboardService.getScores();
        console.log(result);
        setRecords(result);
        requestSent = true;
    }

    useEffect(() => {
        if(requestSent) return;
        getAllRecords();
    }, []);

    return (
    <div className="scoreboard">
        <h1>SCOREBOARD</h1>
        <div className="records-list">
            {showAllRecords()}
        </div>
        <button onClick={() => navigate(MENU_ROUTE)}>Back to menu</button>
    </div>
    );
};

export default Scoreboard;