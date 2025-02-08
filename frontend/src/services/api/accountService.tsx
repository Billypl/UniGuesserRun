import axios, { AxiosInstance } from "axios"
import { ACCOUNT_API_URL, ACCOUNT_TOKEN_KEY } from "../../Constants";

export interface RegisterUserDto {
    nickname: string;
    email: string;
    password: string;
    confirmPassword: string;
}

export interface LoginUserDto {
    nicknameOrEmail: string;
    password: string;
}

export interface AccountDetailsFromTokenDto {
    userId: string;
    nickname: string;
    email: string;
    role: string;
}

export class AccountService {
	private axiosInstance: AxiosInstance

	constructor() {
		this.axiosInstance = axios.create({
			baseURL: ACCOUNT_API_URL,
			headers: {
				'Content-Type': 'application/json',
			},
		})
	}

	addNewUser(nickname: string, email: string, password: string, confirmPassword: string) {
        const registerUserDto: RegisterUserDto = {
            nickname: nickname,
            email: email,
            password: password,
            confirmPassword: confirmPassword,
        }
		this.axiosInstance.put('/register',registerUserDto)
	}

	async login(nicknameOrEmail: string, password: string) {
        const loginUserDto: LoginUserDto = {
            nicknameOrEmail: nicknameOrEmail,
            password: password,
        }

		const response = await this.axiosInstance.post<string>("/login", loginUserDto);
        window.sessionStorage.setItem(ACCOUNT_TOKEN_KEY, response.data);
	}

    logout() {
        // remove session token from backend?
        window.sessionStorage.removeItem(ACCOUNT_TOKEN_KEY);
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
        }
        return response;
    }

    isLoggedIn(): boolean {
        return window.sessionStorage.getItem(ACCOUNT_TOKEN_KEY) !== null;
    }
}

export default new AccountService()
