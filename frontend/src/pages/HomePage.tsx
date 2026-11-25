
import { Link } from 'react-router-dom';
import './HomePage.css';
import { useAuth } from '../context/AuthContext';
function HomePage() {
    const { user } = useAuth();
  return (
    <div className="home-page">
      <section className="hero">
        <div className="hero-content">
          <div className="hero-label">
            <span className="hero-dot"></span>
            Защита персональных данных
          </div>

          <h1>
            Ваши данные.
            <br />
            <span>Под надёжной защитой.</span>
          </h1>

          <p className="hero-description">
            Anonymazer автоматически обнаруживает и обезличивает
            чувствительную информацию в тексте.
            Защитите конфиденциальные данные быстро и удобно.
          </p>

         <div className="hero-actions">
  {user ? (
    <Link to="/anonymize" className="primary-button">
      Начать анонимизацию
    </Link>
  ) : (
    <>
      <Link to="/register" className="primary-button">
        Начать работу
      </Link>

      <Link to="/login" className="secondary-button">
        Войти в аккаунт
      </Link>
    </>
  )}
</div>

          <div className="hero-note">
            Безопасная обработка · Автоматическое обнаружение · История операций
          </div>
        </div>

        <div className="hero-visual">
          <div className="visual-card">
            <div className="visual-header">
              <span className="visual-indicator"></span>
              Пример анонимизации
            </div>

            <div className="visual-content">
              <p className="visual-caption">Исходный текст</p>
              <div className="text-line">
                Иван Иванов, email: ivan@mail.ru
              </div>
              <div className="text-line">
                Телефон: +7 999 123-45-67
              </div>

              <div className="visual-divider"></div>

              <p className="visual-caption">Результат</p>
              <div className="result-line">
                [FULL_NAME], email: <span>[EMAIL]</span>
              </div>
              <div className="result-line">
                Телефон: <span>[PHONE]</span>
              </div>
            </div>

            <div className="visual-footer">
              <span className="success-dot"></span>
              Данные успешно обезличены
            </div>
          </div>

          <div className="decoration decoration-one"></div>
          <div className="decoration decoration-two"></div>
        </div>
      </section>
    </div>
  );
}

export default HomePage;
