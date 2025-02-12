import React from "react";
import { UseFormRegister, FieldValues } from "react-hook-form";
import styles from "../styles/AccountForm.module.scss";

interface FormFieldProps<T extends FieldValues> {
  label: string;
  name: string;
  type?: string;
  register: UseFormRegister<T>;
  error?: string;
}

const FormField = <T extends FieldValues>({ label, name, type = "text", register, error }: FormFieldProps<T>) => {
  return (
    <div className={styles.field}>
      <label>{label}</label>
      <input
        type={type}
        {...register(name as any)}
      />
      {error && <p className={styles.error_message}>{error}</p>}
    </div>
  );
};

export default FormField;