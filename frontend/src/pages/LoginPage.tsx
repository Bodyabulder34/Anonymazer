
import { useState, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';

import { loginUser } from '../services/authService';
import { useAuth } from '../context/AuthContext';

import './AuthPage.css';

function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const { login } = useAuth();
  const navigate = useNavigate();

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    setError('');
    setLoading(true);

    try {
      const result = await loginUser({
        email,
        password,
      });

      login(result.token, {
        username: result.username,
        role: result.role,
        expiresAt: result.expiresAt,
      });

      navigate('/');
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : 'Не удалось войти в аккаунт.'
      );
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="auth-icon">A</div>

        <h1>С возвращением!</h1>
        <p className="auth-description">
          Войдите в аккаунт Anonymazer
        </p>

        <form className="auth-form" onSubmit={handleSubmit}>
          <label htmlFor="email">Электронная почта</label>
          <input
            id="email"
            type="email"
            placeholder="Введите email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            required
          />

          <label htmlFor="password">Пароль</label>
          <input
            id="password"
            type="password"
            placeholder="Введите пароль"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            required
          />

          {error && <div className="auth-error">{error}</div>}

          <button
            type="submit"
            className="auth-button"
            disabled={loading}
          >
            {loading ? 'Выполняется вход...' : 'Войти'}
          </button>
        </form>

        <p className="auth-switch">
          Ещё нет аккаунта? <Link to="/register">Зарегистрироваться</Link>
        </p>
      </div>
    </div>
  );
}

export default LoginPage;
