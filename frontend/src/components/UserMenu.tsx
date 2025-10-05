import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import Account from "./Account";
import styles from "../styles/UserMenu.module.scss";
import { PROFILE_ROUTE, SCOREBOARD_ROUTE } from "../Constants";

const UserMenu = () => {
	const [isOpen, setIsOpen] = useState(false);
	const navigate = useNavigate();

	const toggleMenu = () => {
		setIsOpen(!isOpen);
	}

	return (
		<div className={styles.user_menu_container}>
			<button
        className={styles.hamburger_toggle}
        onClick={toggleMenu}
        aria-expanded={isOpen}
      >
        <span className={isOpen ? styles.bar_open : styles.bar}></span>
        <span className={isOpen ? styles.bar_open : styles.bar}></span>
        <span className={isOpen ? styles.bar_open : styles.bar}></span>
      </button>
			
      {isOpen && (
        <div className={styles.user_menu}>
          <Account />
          <button
            className={styles.menu_option}
            onClick={() => {
              navigate(PROFILE_ROUTE);
              setIsOpen(false);
            }}
          >
            Profile
          </button>
          <button
            className={styles.menu_option}
            onClick={() => {
              navigate(SCOREBOARD_ROUTE);
              setIsOpen(false);
            }}
          >
            Scoreboard
          </button>
        </div>
      )}
		</div>
	)
};


export default UserMenu;