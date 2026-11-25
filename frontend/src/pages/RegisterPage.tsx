
import { useState, type FormEvent } from 'react';
import { Link } from 'react-router-dom';
import { registerUser } from '../services/authService';
import './AuthPage.css';

function RegisterPage() {
    const [username, setUsername] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
    const [loading, setLoading] = useState(false);

    async function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();

        setError('');
        setSuccess('');
        setLoading(true);

        try {
            await registerUser({
                username,
                email,
                password,
            });

            setSuccess('Регистрация прошла успешно! Теперь вы можете войти.');
            setUsername('');
            setEmail('');
            setPassword('');
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : 'Произошла ошибка при регистрации.'
            );
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="auth-page">
            <div className="auth-card">
                <div className="auth-icon">A</div>

                <h1>Создать аккаунт</h1>
                <p className="auth-description">
                    Зарегистрируйтесь в Anonymazer
                </p>

                <form className="auth-form" onSubmit={handleSubmit}>
                    <label htmlFor="username">Имя пользователя</label>
                    <input
                        id="username"
                        type="text"
                        placeholder="Придумайте имя пользователя"
                        value={username}
                        onChange={(event) => setUsername(event.target.value)}
                        minLength={3}
                        maxLength={30}
                        required
                    />

                    <label htmlFor="email">Электронная почта</label>
                    <input
                        id="email"
                        type="email"
                        placeholder="Введите email"
                        value={email}
                        onChange={(event) => setEmail(event.target.value)}
                        maxLength={254}
                        required
                    />

                    <label htmlFor="password">Пароль</label>
                    <input
                        id="password"
                        type="password"
                        placeholder="Придумайте пароль"
                        value={password}
                        onChange={(event) => setPassword(event.target.value)}
                        minLength={8}
                        maxLength={128}
                        required
                    />

                    {error && <div className="auth-error">{error}</div>}

                    {success && <div className="auth-success">{success}</div>}

                    <button
                        type="submit"
                        className="auth-button"
                        disabled={loading}
                    >
                        {loading ? 'Регистрация...' : 'Зарегистрироваться'}
                    </button>
                </form>

                <p className="auth-switch">
                    Уже есть аккаунт? <Link to="/login">Войти</Link>
                </p>
            </div>
        </div>
    );
}

export default RegisterPage;
