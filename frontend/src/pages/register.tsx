import React from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import accountService from "../services/api/accountService";
import FormField from "../components/FormField";
import Logo from "../components/Logo";
import { LOGIN_ROUTE } from "../Constants";

interface RegisterFormInputs {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
}

const Register: React.FC = () => {
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterFormInputs>();

  const registerUser = (data: RegisterFormInputs) => {
    accountService.addNewUser(data.username, data.email, data.password, data.confirmPassword);
    navigate(LOGIN_ROUTE);
  }

  return (
    <>
    <Logo />
    <form onSubmit={handleSubmit(registerUser)} className="registration-form">
      <h2 className="form-header">Register</h2>

      <FormField
        label="Username"
        name="username"
        type="text"
        register={register}
        error={errors.username?.message}
      />

      <FormField
        label="Email"
        name="email"
        type="email"
        register={register}
        error={errors.email?.message}
      />
      
      <FormField
        label="Password"
        name="password"
        type="password"
        register={register}
        error={errors.password?.message}
      />

      <FormField
        label="Confirm password"
        name="confirmPassword"
        type="password"
        register={register}
        error={errors.confirmPassword?.message}
      />

      <button type="submit" className="register-form-button">Register</button>
    </form>
    <button onClick={() => navigate(LOGIN_ROUTE)}>Already have an account</button> 
    </>
  );
};

export default Register;
