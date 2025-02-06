import React from "react";
import Account from "./Account";
import Logo from "./Logo";
import styles from "../styles/Header.module.scss"

const Header = () => {
  return (
    <header className={styles.header}>
      <nav className={styles.nav}>
        <Logo />
        <Account />
      </nav>
    </header>
  );
};

export default Header;