import React from 'react';
import { useNavigate } from 'react-router-dom';
import accountService from '../services/api/accountService';
import { LOGIN_ROUTE, MENU_ROUTE, REGISTER_ROUTE } from '../Constants';
import styles from '../styles/Account.module.scss';

const Account: React.FC = () => {
    const navigate = useNavigate();

    const handleLogout = () => {
        accountService.logout();
        navigate(MENU_ROUTE);
    }

    const displayGuestContent = () => {
        return (
            <div className={styles.account}>
                <a className={styles.nav_item} onClick={() => navigate(LOGIN_ROUTE)}>
                    Login
                </a>
                <a className={styles.nav_item} onClick={() => navigate(REGISTER_ROUTE)}>
                    Register
                </a>
            </div>
        )
    }

    const displayUserContent = () => {
        return (
            <div className={styles.account}>
                <a className={styles.nav_item}>
                    {accountService.getCurrentUserNickname()}
                </a>
                <a className={styles.logout} onClick={handleLogout}>
                   Logout
                </a>
            </div>
        )
    }

    return accountService.isLoggedIn() ? displayUserContent() : displayGuestContent();
}

export default Account;