import React, { useCallback, useRef, useState } from "react";
import Webcam from "react-webcam";

const FormCamera: React.FC = () => {
  const [isOpen, setIsOpen] = useState<boolean>(false);
  const [image, setImage] = useState<string | null>(null);
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
          <button onClick={capture}>Capture photo</button>
          <button onClick={() => setIsOpen(false)}>Close camera</button>
        </>
      ) : (
        <>
          <button onClick={() => setIsOpen(true)}>Open camera</button>
          {image && <img src={image} />}
        </>
      )}
    </>
  );
};

export default FormCamera;
