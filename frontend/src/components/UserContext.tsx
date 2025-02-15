import React, { createContext, useState, ReactNode } from "react";

export interface UserContextType {
  username: string;
  setUsername: (username: string) => void;
  email: string;
  setEmail: (email: string) => void;
}

export const UserContext = createContext<UserContextType | undefined>(undefined);

interface UserContextProviderProps {
  children: ReactNode;
}

export const UserContextProvider: React.FC<UserContextProviderProps> = ({ children }) => {
  const [username, setUsername] = useState<string>("");
  const [email, setEmail] = useState<string>("");

  const value: UserContextType = {
    username: username,
    setUsername: setUsername,
    email: email,
    setEmail: setEmail,
  };

  return <UserContext.Provider value={value}>{children}</UserContext.Provider>;
};