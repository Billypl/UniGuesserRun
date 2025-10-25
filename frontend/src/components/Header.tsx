import { useNavigate } from "react-router-dom";
import Logo from "./Logo";
import { MENU_ROUTE } from '../Constants';
import styles from "../styles/Header.module.scss"
import UserMenu from "./UserMenu";

const Header = () => {
    const navigate = useNavigate();

    return (
    <header className={styles.header}>
      <nav className={styles.nav}>
        <Logo />
        <a className={styles.title} onClick={() => navigate(MENU_ROUTE)}>UniGuesser</a>
				<UserMenu/>
      </nav>
    </header> 
  );
};

export default Header;