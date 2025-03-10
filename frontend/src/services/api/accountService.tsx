import axios, { AxiosInstance } from 'axios'
import { ACCOUNT_API_URL, ACCOUNT_TOKEN_KEY, REFRESH_TOKEN_KEY, USER_NICKNAME } from '../../Constants'
import axios, { AxiosError, AxiosInstance } from "axios";
import { ACCOUNT_API_URL, ACCOUNT_TOKEN_KEY } from "../../Constants";

export interface RegisterUserDto {
	nickname: string
	email: string
	password: string
	confirmPassword: string
}

export interface LoginUserDto {
	nicknameOrEmail: string
	password: string
}

export interface AccountDetailsFromTokenDto {
	userId: string
	nickname: string
	email: string
	role: string
}

export interface LoginResultDto {
	token: string
	refreshToken: string
	nickname: string
}

export class AccountService {
    private axiosInstance: AxiosInstance;

    constructor() {
        this.axiosInstance = axios.create({
            baseURL: ACCOUNT_API_URL,
            headers: {
                "Content-Type": "application/json",
            },
        });
    }

	addNewUser(nickname: string, email: string, password: string, confirmPassword: string) {
		const registerUserDto: RegisterUserDto = {
			nickname: nickname,
			email: email,
			password: password,
			confirmPassword: confirmPassword,
		}
		this.axiosInstance.put('/register', registerUserDto)
	}

	async login(nicknameOrEmail: string, password: string) {
		const loginUserDto: LoginUserDto = {
			nicknameOrEmail: nicknameOrEmail,
			password: password,
		}
    async addNewUser(
        nickname: string,
        email: string,
        password: string,
        confirmPassword: string
    ): Promise<string | null> {
        try {
            const registerUserDto: RegisterUserDto = {
                nickname: nickname,
                email: email,
                password: password,
                confirmPassword: confirmPassword,
            };
            await this.axiosInstance.put("/register", registerUserDto);
        } catch (err) {
            const error = err as AxiosError<{ error?: string; errors?: Record<string, string> }>;
            if (!error.response) {
                return "Network error. Please try again.";
            }
            if (error.response.status !== 400) {
                return "An unexpected error occurred. Please try again later.";
            }
            if (error.response.data?.errors) {
                const errors = error.response.data.errors;
                return Object.values(errors).join("\n");
            }
            if (error.response.data?.error) {
                return error.response.data.error;
            }
            return "Invalid request. Please check your input.";
        }

        return null;
    }

    async login(nicknameOrEmail: string, password: string): Promise<string | null> {
        try {
            const loginUserDto: LoginUserDto = {
                nicknameOrEmail: nicknameOrEmail,
                password: password,
            };

		const response = await this.axiosInstance.post<LoginResultDto>('/login', loginUserDto)
		window.sessionStorage.setItem(ACCOUNT_TOKEN_KEY, response.data.token)
		window.sessionStorage.setItem(REFRESH_TOKEN_KEY, response.data.refreshToken)
		window.sessionStorage.setItem(USER_NICKNAME, response.data.nickname)
	}
            const response = await this.axiosInstance.post<string>("/login", loginUserDto);
            window.sessionStorage.setItem(ACCOUNT_TOKEN_KEY, response.data);
        } catch (err) {
            const error = err as AxiosError;
            if (!error.response) {
                return "Network error. Please check your connection.";
            }
            if (error.response.status === 404) {
                return "User or password invalid";
            }
            if (error.response.status === 401) {
                return "Incorrect password. Please try again.";
            }
            return "An unexpected error occurred. Please try again later.";
        }

        return null;
    }

	logout() {
		// TODO: remove session token from backend?
		window.sessionStorage.removeItem(ACCOUNT_TOKEN_KEY)
		window.sessionStorage.removeItem(REFRESH_TOKEN_KEY)
		window.sessionStorage.removeItem(USER_NICKNAME)
	}

	async getLoggedInUser(): Promise<AccountDetailsFromTokenDto> {
		// const response = await this.axiosInstance.get<AccountDetailsFromTokenDto>('/user', {
		//     headers: {
		//       Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
		//     },
		//   })
		//return response.data;
		const response: AccountDetailsFromTokenDto = {
			userId: '123',
			nickname: 'test',
			email: 'test@wp.pl',
			role: 'user',
		}
		return response
	}
    async getLoggedInUser(): Promise<AccountDetailsFromTokenDto> {
        // const response = await this.axiosInstance.get<AccountDetailsFromTokenDto>('/user', {
        //     headers: {
        //       Authorization: `Bearer ${sessionStorage.getItem(ACCOUNT_TOKEN_KEY)}`,
        //     },
        //   })
        //return response.data;
        const response: AccountDetailsFromTokenDto = {
            userId: "123",
            nickname: "test",
            email: "test@wp.pl",
            role: "user",
        };
        return response;
    }

	isLoggedIn(): boolean {
		return window.sessionStorage.getItem(ACCOUNT_TOKEN_KEY) !== null
	}
}

export default new AccountService();
