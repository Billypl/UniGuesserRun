import React from 'react';
import { useNavigate } from "react-router-dom";
import { MENU_ROUTE } from '../Constants';
import logo from '../assets/images/logo_UGR.png';
import styles from "../styles/Logo.module.scss";

const Logo: React.FC = () => {
    const navigate = useNavigate();

    return(
        <a className={styles.logo} onClick={() => navigate(MENU_ROUTE)}>
            <img src={logo}></img>
        </a>
    )
};

export default Logo;