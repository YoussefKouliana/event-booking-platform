import { createContext, useContext, useReducer, useEffect, useCallback } from 'react';
import type { ReactNode } from 'react';
import axios from 'axios';
import type { AxiosError } from 'axios';

// Configure axios defaults
const API_BASE_URL = 'https://localhost:7193/api';
axios.defaults.baseURL = API_BASE_URL;

// Types
interface User {
  id: string;
  email: string;
  name?: string;
  firstName?: string;
  lastName?: string;
  exp: number;
}

interface AuthState {
  user: User | null;
  token: string | null;
  isLoading: boolean;
  error: string | null;
  isAuthenticated: boolean;
}

interface LoginResponse {
  success: boolean;
  error?: string;
}

interface RegisterResponse {
  success: boolean;
  error?: string;
}

interface AuthContextType extends AuthState {
  login: (email: string, password: string) => Promise<LoginResponse>;
  register: (email: string, password: string, firstName?: string, lastName?: string) => Promise<RegisterResponse>;
  logout: () => void;
  clearError: () => void;
  isTokenExpired: () => boolean;
}

interface AuthProviderProps {
  children: ReactNode;
}

// Auth actions
const AUTH_ACTIONS = {
  LOGIN_START: 'LOGIN_START',
  LOGIN_SUCCESS: 'LOGIN_SUCCESS',
  LOGIN_FAILURE: 'LOGIN_FAILURE',
  LOGOUT: 'LOGOUT',
  REGISTER_START: 'REGISTER_START',
  REGISTER_SUCCESS: 'REGISTER_SUCCESS',
  REGISTER_FAILURE: 'REGISTER_FAILURE',
  SET_USER: 'SET_USER',
  CLEAR_ERROR: 'CLEAR_ERROR'
} as const;

type AuthAction =
  | { type: typeof AUTH_ACTIONS.LOGIN_START }
  | { type: typeof AUTH_ACTIONS.LOGIN_SUCCESS; payload: { token: string; user: User } }
  | { type: typeof AUTH_ACTIONS.LOGIN_FAILURE; payload: string }
  | { type: typeof AUTH_ACTIONS.LOGOUT }
  | { type: typeof AUTH_ACTIONS.REGISTER_START }
  | { type: typeof AUTH_ACTIONS.REGISTER_SUCCESS }
  | { type: typeof AUTH_ACTIONS.REGISTER_FAILURE; payload: string }
  | { type: typeof AUTH_ACTIONS.SET_USER; payload: User }
  | { type: typeof AUTH_ACTIONS.CLEAR_ERROR };

// Create the Auth Context
const AuthContext = createContext<AuthContextType | undefined>(undefined);

// Initial state
const initialState: AuthState = {
  user: null,
  token: localStorage.getItem('token'),
  isLoading: false,
  error: null,
  isAuthenticated: false
};

// Auth reducer
function authReducer(state: AuthState, action: AuthAction): AuthState {
  switch (action.type) {
    case AUTH_ACTIONS.LOGIN_START:
    case AUTH_ACTIONS.REGISTER_START:
      return {
        ...state,
        isLoading: true,
        error: null
      };

    case AUTH_ACTIONS.LOGIN_SUCCESS:
      return {
        ...state,
        isLoading: false,
        token: action.payload.token,
        user: action.payload.user,
        isAuthenticated: true,
        error: null
      };

    case AUTH_ACTIONS.REGISTER_SUCCESS:
      return {
        ...state,
        isLoading: false,
        error: null
      };

    case AUTH_ACTIONS.LOGIN_FAILURE:
    case AUTH_ACTIONS.REGISTER_FAILURE:
      return {
        ...state,
        isLoading: false,
        error: action.payload,
        token: null,
        user: null,
        isAuthenticated: false
      };

    case AUTH_ACTIONS.LOGOUT:
      return {
        ...state,
        user: null,
        token: null,
        isAuthenticated: false,
        error: null
      };

    case AUTH_ACTIONS.SET_USER:
      return {
        ...state,
        user: action.payload,
        isAuthenticated: true
      };

    case AUTH_ACTIONS.CLEAR_ERROR:
      return {
        ...state,
        error: null
      };

    default:
      return state;
  }
}

// Decode JWT to get user info
function decodeToken(token: string): User | null {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return {
      id: payload.nameid,
      email: payload.email,
      name: payload.unique_name,
      firstName: payload.given_name,
      lastName: payload.family_name,
      exp: payload.exp
    };
  } catch (error) {
    return null;
  }
}

// Check if token is expired
function isTokenExpired(token: string | null): boolean {
  if (!token) return true;
  
  try {
    const user = decodeToken(token);
    if (!user) return true;
    
    // Check if token expires in the next 5 minutes
    const now = Date.now() / 1000;
    return user.exp < (now + 300);
  } catch (error) {
    return true;
  }
}

// Get error message from axios error
function getErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const axiosError = error as AxiosError<any>;
    
    // Handle connection errors
    if (axiosError.code === 'ERR_NETWORK' || axiosError.code === 'ERR_CONNECTION_REFUSED') {
      return 'Unable to connect to server. Please make sure the backend is running.';
    }
    
    return axiosError.response?.data?.message || 
           axiosError.response?.data || 
           axiosError.message || 
           'An error occurred';
  }
  if (error instanceof Error) {
    return error.message;
  }
  return 'An unknown error occurred';
}

// Auth Provider Component
export function AuthProvider({ children }: AuthProviderProps) {
  const [state, dispatch] = useReducer(authReducer, initialState);

  // Setup axios interceptor for auth
  useEffect(() => {
    const requestInterceptor = axios.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('token');
        if (token && !isTokenExpired(token)) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    const responseInterceptor = axios.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          // Don't call logout here to avoid infinite loops
          localStorage.removeItem('token');
          dispatch({ type: AUTH_ACTIONS.LOGOUT });
        }
        return Promise.reject(error);
      }
    );

    return () => {
      axios.interceptors.request.eject(requestInterceptor);
      axios.interceptors.response.eject(responseInterceptor);
    };
  }, []);

  // Check for existing token on mount
  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token && !isTokenExpired(token)) {
      const user = decodeToken(token);
      if (user) {
        dispatch({
          type: AUTH_ACTIONS.SET_USER,
          payload: user
        });
      }
    } else if (token) {
      // Token exists but is expired
      localStorage.removeItem('token');
    }
  }, []);

  // Auth functions
  const login = async (email: string, password: string): Promise<LoginResponse> => {
    dispatch({ type: AUTH_ACTIONS.LOGIN_START });

    try {
      const response = await axios.post<{ token: string }>('/auth/login', {
        email,
        password
      });

      const { token } = response.data;
      const user = decodeToken(token);

      if (!user) {
        throw new Error('Invalid token received');
      }

      localStorage.setItem('token', token);

      dispatch({
        type: AUTH_ACTIONS.LOGIN_SUCCESS,
        payload: { token, user }
      });

      return { success: true };
    } catch (error) {
      const errorMessage = getErrorMessage(error);

      dispatch({
        type: AUTH_ACTIONS.LOGIN_FAILURE,
        payload: errorMessage
      });

      return { success: false, error: errorMessage };
    }
  };

  const register = async (
    email: string, 
    password: string, 
    firstName = '', 
    lastName = ''
  ): Promise<RegisterResponse> => {
    dispatch({ type: AUTH_ACTIONS.REGISTER_START });

    try {
      await axios.post('/auth/register', {
        email,
        password,
        firstName,
        lastName
      });

      dispatch({ type: AUTH_ACTIONS.REGISTER_SUCCESS });

      // Auto-login after successful registration
      return await login(email, password);
    } catch (error) {
      const errorMessage = getErrorMessage(error);

      dispatch({
        type: AUTH_ACTIONS.REGISTER_FAILURE,
        payload: errorMessage
      });

      return { success: false, error: errorMessage };
    }
  };

  const logout = (): void => {
    localStorage.removeItem('token');
    dispatch({ type: AUTH_ACTIONS.LOGOUT });
  };

  const clearError = useCallback((): void => {
    dispatch({ type: AUTH_ACTIONS.CLEAR_ERROR });
  }, []);

  const value: AuthContextType = {
    ...state,
    login,
    register,
    logout,
    clearError,
    isTokenExpired: () => isTokenExpired(state.token)
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}

// Hook to use auth context
export function useAuth(): AuthContextType {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}

export default AuthContext;