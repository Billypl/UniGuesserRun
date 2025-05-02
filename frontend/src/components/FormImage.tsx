import React, { useCallback, useRef, useState } from "react";
import { UseFormRegister, FieldValues } from "react-hook-form";
import Webcam from "react-webcam";
import styles from "../styles/FormImage.module.scss";
import FormField from "./FormField";

interface FormImageProps<T extends FieldValues> {
  setImage: (image: string | null) => void;
  image: string | null;
  register: UseFormRegister<T>;
}

const FormImage = <T extends FieldValues> ({setImage, image, register} : FormImageProps<T>) => {
  const [isOpen, setIsOpen] = useState<boolean>(false);
  const webcamRef = useRef<Webcam | null>(null);

  const capture = useCallback(() => {
    const imageSrc = webcamRef.current?.getScreenshot();
    console.log(imageSrc);
    if (imageSrc === undefined) {
      console.error("Error while capturing picture");
      return;
    }
    setImage(imageSrc);
    setIsOpen(false);
  }, [webcamRef]);

  const videoConstraints = { width: 1280, height: 720, facingMode: "environment" };

  const handleChangeImage = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];

    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        setImage(reader.result as string);
        console.log(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  }

  return (
    <>
      {isOpen ? (
        <>
          <Webcam
            audio={false}
            height={720}
            ref={webcamRef}
            screenshotFormat="image/jpeg"
            width={1280}
            videoConstraints={videoConstraints}
          />
          <button className={styles.photo_button} onClick={capture}>Capture photo</button>
          <button className={styles.photo_button} onClick={() => setIsOpen(false)}>Close camera</button>
        </>
      ) : (
        <>
          <button className={styles.button} onClick={() => setIsOpen(true)}>Open camera</button>
          {image && <img src={image} />}

          <FormField
            label="Upload image"
            name="image"
            type="file"
            register={register}
            onChange={handleChangeImage}
            accept="image/*"
          />

        </>
      )}
    </>
  );
};

export default FormImage;
