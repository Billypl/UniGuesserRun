import React from "react";
import { useNavigate } from "react-router-dom";
import Account from "./Account";
import Logo from "./Logo";
import { MENU_ROUTE } from '../Constants';
import styles from "../styles/Header.module.scss"

const Header = () => {
    const navigate = useNavigate();

    return (
    <header className={styles.header}>
      <nav className={styles.nav}>
        <Logo />
        <div className={styles.title} onClick={() => navigate(MENU_ROUTE)}>UniGuesser</div>
        <Account />
      </nav>
    </header>
  );
};

export default Header;