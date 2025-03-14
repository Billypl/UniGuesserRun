import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import FormField from "../components/FormField";
import Logo from "../components/Logo";
import { MENU_ROUTE } from "../Constants";
import { useUserContext } from "../hooks/useUserContext";
import styles from "../styles/AccountForm.module.scss";
import placeService from "../services/api/placeService";

interface AddPlaceFormInputs {
  name: string;
  description: string;
  coordinates: string;
  imageUrl: string;
  alt: string;
  difficulty: string;
}

const AddPlace: React.FC = () => {
  const navigate = useNavigate();
  const { setUsername } = useUserContext();
  const [error, setError] = useState<string | null>(null);

  const {
    register: login,
    handleSubmit,
    formState: { errors },
  } = useForm<AddPlaceFormInputs>();

  const addNewPlace = async (data: AddPlaceFormInputs) => {
    const errorMessage = await placeService.addNewPlace(
      data.name,
      data.description,
      data.coordinates,
      data.imageUrl,
      data.alt,
      data.difficulty
    );

    setError(errorMessage);

    if (!errorMessage) {
      // TODO: ok and return to menu OR add another place

      //const user = await accountService.getLoggedInUser();
      //setUsername(user.nickname);
      navigate(MENU_ROUTE);
    }
  };

  return (
    <div className={styles.form_container}>
      <Logo />
      <h2 className={styles.header}>Add new place to UniGuesser</h2>
      <form onSubmit={handleSubmit(addNewPlace)} className={styles.form}>
        <FormField
          label="Name"
          name="name"
          type="text"
          register={login}
          error={errors.name?.message}
        />

        <FormField
          label="Description"
          name="description"
          type="text"
          register={login}
          error={errors.description?.message}
        />

        <FormField
          label="Coordinates"
          name="coordinates"
          type="text"
          register={login}
          error={errors.coordinates?.message}
        />

        <FormField
          label="imageUrl"
          name="imageUrl"
          type="text"
          register={login}
          error={errors.imageUrl?.message}
        />

        <FormField
          label="alt"
          name="alt"
          type="text"
          register={login}
          error={errors.alt?.message}
        />

        <FormField
          label="difficulty"
          name="difficulty"
          type="text"
          register={login}
          error={errors.difficulty?.message}
        />

        {error && <p className={styles.error}>{error}</p>}

        <button type="submit" className={styles.submit_button}>
          Submit
        </button>
      </form>
    </div>
  );
};

export default AddPlace;
