import exp from "constants"

export const GAME_API_URL = 'https://localhost:7157/api/game'
export const SCOREBOARD_API_URL = 'https://localhost:7157/api/scoreboard'
export const ACCOUNT_API_URL = 'https://localhost:7157/api/account'
export const PLACE_API_URL = 'https://localhost:7157/api/place'

export const GAME_TOKEN_KEY = 'game_token'
export const ACCOUNT_TOKEN_KEY = 'account_token'
export const REFRESH_TOKEN_KEY = 'refresh_token'
export const USER_NICKNAME_KEY = 'nickname'

export const MENU_ROUTE = '/menu'
export const LOGIN_ROUTE = '/login'
export const REGISTER_ROUTE = '/register'
export const GAME_ROUTE = '/game'
export const GAME_RESULTS_ROUTE = '/game_results'
export const SCOREBOARD_ROUTE = '/scoreboard'
export const ADD_PLACE_ROUTE = '/add_place'
export const PLACE_QUEUE_ROUTE = '/place_queue'
export const PLACES_ROUTE = '/places'

export const JWT_USER_ROLE_KEY = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
export const JWT_USER_ID_KEY = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
export const JWT_USER_NICKNAME_KEY = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
export const JWT_USER_EMAIL_KEY = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'

export const USER_ROLE_ADMIN = 'Admin'
export const USER_ROLE_MODERATOR = 'Moderator'
export const USER_ROLE_USER = 'User'