import { UseFormRegister, FieldValues } from "react-hook-form";
import styles from "../styles/FormField.module.scss";

interface FormFieldProps<T extends FieldValues> {
    label: string;
    name: string;
    type?: string;
    register: UseFormRegister<T>;
    onChange?: (event: React.ChangeEvent<HTMLInputElement>) => void;
    error?: string;
    accept?: string;
}

const FormField = <T extends FieldValues>({ label, name, type = "text", register, onChange, error, accept }: FormFieldProps<T>) => {
    return (
        <div className={styles.field}>
            <label>{label}</label>
            <input type={type} {...register(name as any, {onChange: onChange})} accept={accept}/>
            {error && <p className={styles.hint}>{error}</p>}
        </div>
    );
};

export default FormField;
