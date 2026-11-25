
import { Link, NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import './Header.css';

function Header() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate('/');
  }

  return (
    <header className="header">
      <div className="header-container">
        <Link to="/" className="logo">
          Anony<span>mazer</span>
        </Link>

        <nav className="navigation">
          <NavLink to="/" end>
            Главная
          </NavLink>

          {user ? (
            <>
              <NavLink to="/anonymize">
                Анонимизация
              </NavLink>

              <NavLink to="/history">
                История
              </NavLink>

              <span className="user-name">
                {user.username}
              </span>

              <button
                className="logout-button"
                onClick={handleLogout}
              >
                Выйти
              </button>
            </>
          ) : (
            <>
              <NavLink to="/login">
                Войти
              </NavLink>

              <NavLink to="/register" className="register-link">
                Регистрация
              </NavLink>
            </>
          )}
        </nav>
      </div>
    </header>
  );
}

export default Header;
