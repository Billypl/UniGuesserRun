import React from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import accountService from "../services/api/accountService";
import FormField from "../components/FormField";
import Logo from "../components/Logo";
import { MENU_ROUTE, REGISTER_ROUTE } from "../Constants";

interface LoginFormInputs {
  nicknameOrEmail: string;
  password: string;
}

const Login: React.FC = () => {
  const navigate = useNavigate();

  const {
    register: login,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormInputs>();

  const loginUser = async(data: LoginFormInputs) => {
    await accountService.login(data.nicknameOrEmail, data.password);
    navigate(MENU_ROUTE);
  }

  return (
    <>
    <Logo />
    <form onSubmit={handleSubmit(loginUser)} className="login-form">
      <h2 className="form-header">Login</h2>

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

      <button type="submit" className="login-form-button">Login</button>
    </form>
    <button onClick={() => navigate(REGISTER_ROUTE)}>New to UniGuesser? Sign up</button> 
    </>
  );
};

export default Login;
