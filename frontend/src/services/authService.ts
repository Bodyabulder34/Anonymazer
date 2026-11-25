
import API_URL from './api';

export interface RegisterRequest {
    username: string;
    email: string;
    password: string;
}

export interface AuthResponse {
    token: string;
}

export async function registerUser(data: RegisterRequest) {
    const response = await fetch(`${API_URL}/Auth/register`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });

    const result = await response.json().catch(() => null);

    if (!response.ok) {
        throw new Error(
            result?.message || 'Не удалось зарегистрироваться.'
        );
    }

    return result;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export async function loginUser(data: LoginRequest) {
  const response = await fetch(`${API_URL}/Auth/login`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(data),
  });

  const result = await response.json().catch(() => null);

  if (!response.ok) {
    throw new Error(
      result?.message || 'Не удалось войти в аккаунт.'
    );
  }

  return result;
}
