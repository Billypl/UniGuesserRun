import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import Account from "./Account";
import styles from "../styles/UserMenu.module.scss";
import { MENU_ROUTE, PROFILE_ROUTE, SCOREBOARD_ROUTE } from "../Constants";
import homeIcon from '../assets/images/home.png';
import scoreboardIcon from '../assets/images/scoreboard.png';

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
					<button
            className={styles.menu_option}
            onClick={() => {
              navigate(MENU_ROUTE);
              setIsOpen(false);
            }}
          >
						<img src={homeIcon} alt="Home icon" className={styles.icon} />
            Home
          </button>
          <Account />
          <button
            className={styles.menu_option}
            onClick={() => {
              navigate(SCOREBOARD_ROUTE);
              setIsOpen(false);
            }}
          >
						<img src={scoreboardIcon} alt="SCoreboard icon" className={styles.icon} />
            Scoreboard
          </button>
        </div>
      )}
		</div>
	)
};


export default UserMenu;