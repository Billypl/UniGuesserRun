import React from "react";
import { UseFormRegister, FieldValues } from "react-hook-form";
import styles from "../styles/FormSelect.module.scss";

interface FormSelectProps<T extends FieldValues> {
  label: string;
  name: string;
  options: { value: string; label: string }[];
  register: UseFormRegister<T>;
  error?: string;
}

const FormSelect = <T extends FieldValues>({ label, name, options, register, error }: FormSelectProps<T>) => {
  return (
    <div className={styles.field}>
      <label>{label}</label>
      <select {...register(name as any)}>
        {options.map(({ value, label }) => (
          <option key={value} value={value}>
            {label}
          </option>
        ))}
      </select>
      {error && <p className={styles.hint}>{error}</p>}
    </div>
  );
};

export default FormSelect;
