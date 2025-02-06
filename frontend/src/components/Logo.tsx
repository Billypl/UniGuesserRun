import React from 'react';
import { useNavigate } from "react-router-dom";
import { MENU_ROUTE } from '../Constants';
import logo from '../assets/images/logo_UGR.png';
import styles from "../styles/Logo.module.scss"

const Logo: React.FC = () => {
    const navigate = useNavigate();

    return(
        <div className={styles.logo}>
            <a onClick={() => navigate(MENU_ROUTE)}>
                <img src={logo}></img>
            </a>
        </div>
    )

};

export default Logo;