import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import accountService from "../services/api/accountService";
import FormField from "../components/FormField";
import Logo from "../components/Logo";
import { MENU_ROUTE, REGISTER_ROUTE } from "../Constants";
import { useUserContext } from "../hooks/useUserContext";
import styles from "../styles/AccountForm.module.scss";

interface LoginFormInputs {
  nicknameOrEmail: string;
  password: string;
}

const Login: React.FC = () => {
  const navigate = useNavigate();
  const { setUsername } = useUserContext();
  const [error, setError] = useState<string | null>(null);

  const {
    register: login,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormInputs>();

  const loginUser = async(data: LoginFormInputs) => {
    //TODO: sprawdzenie w przypadku błędnych danych
    const errorMessage = await accountService.login(data.nicknameOrEmail, data.password);
    setError(errorMessage);
    if (!errorMessage) {
      const user = await accountService.getLoggedInUser();
      setUsername(user.nickname);
      navigate(MENU_ROUTE);
    }
  }

  return (
    <div className={styles.form_container}>
      <Logo />
      <h2 className={styles.header}>Sign in to UniGuesser</h2>
      <form onSubmit={handleSubmit(loginUser)} 
    className={styles.form}>

      <FormField
        label="Nickname or Email"
        name="nicknameOrEmail"
        type="text"
        register={login}
        error={errors.nicknameOrEmail?.message}
      />
      
      <FormField
        label="Password"
        name="password"
        type="password"
        register={login}
        error={errors.password?.message}
      />

      {error && <p className={styles.error}>{error}</p>}

      <button type="submit" className={styles.submit_button}>Login</button>
    </form>
    <p onClick={() => navigate(REGISTER_ROUTE)} className={styles.link}>New to UniGuesser? Sign up</p> 
    </div>
  );
};

export default Login;
