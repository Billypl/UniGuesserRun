import axios, { AxiosInstance } from 'axios'

const API_URL = 'https://localhost:7157/api/account'

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

export class AccountService {
	private axiosInstance: AxiosInstance

	constructor() {
		this.axiosInstance = axios.create({
			baseURL: API_URL,
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
        window.sessionStorage.setItem("token", response.data);
	}
}

export default new AccountService()
