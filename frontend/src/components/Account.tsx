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
                <div className={styles.nav_item}>
                    <a onClick={() => navigate(LOGIN_ROUTE)}>Login</a>
                </div>
                <div className={styles.nav_item}>
                    <a onClick={() => navigate(REGISTER_ROUTE)}>Register</a>
                </div>
            </div>
        )
    }

    const displayUserContent = () => {
        return (
            <div className={styles.account}>
                <div className={styles.nav_item}>
                    <a onClick={handleLogout}>Logout</a>
                </div>
            </div>
        )
    }

    return accountService.isLoggedIn() ? displayUserContent() : displayGuestContent();
}

export default Account;