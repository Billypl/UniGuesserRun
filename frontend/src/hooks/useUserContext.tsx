import { useContext } from "react";
import { UserContext, UserContextType } from "../components/UserContext";

export const useUserContext = (): UserContextType => {
  const context = useContext(UserContext);
  if (!context) {
    throw new Error("useUserContext must be used within a UserContextProvider");
  }
  return context;
};
