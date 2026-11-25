
import { useState, type FormEvent } from 'react';
import { useAuth } from '../context/AuthContext';

import {
  anonymizeText,
  createAsyncAnonymization,
  getAsyncJobStatus,
} from '../services/anonymizationService';

import './AnonymizationPage.css';

type ProcessingMode = 'sync' | 'async';

function AnonymizationPage() {
  const { token } = useAuth();

  const [text, setText] = useState('');
  const [result, setResult] = useState('');
  const [mode, setMode] = useState<ProcessingMode>('sync');

  const [loading, setLoading] = useState(false);
  const [status, setStatus] = useState('');
  const [error, setError] = useState('');

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!token || !text.trim()) return;

    setLoading(true);
    setError('');
    setResult('');
    setStatus('');

    try {
      if (mode === 'sync') {
        const response = await anonymizeText(text, token);

        setResult(response.anonymizedText);
        setStatus('Обработка успешно завершена.');
      } else {
        const job = await createAsyncAnonymization(text, token);

        if (!job.jobId) {
          throw new Error('Сервер не вернул идентификатор задачи.');
        }

        setStatus('Задача отправлена в очередь. Ожидаем результат...');

        let completed = false;

        for (let attempt = 0; attempt < 60; attempt++) {
          await new Promise(resolve => setTimeout(resolve, 2000));

          const jobStatus = await getAsyncJobStatus(job.jobId, token);
          const currentStatus = String(jobStatus.status).toLowerCase();

          if (currentStatus === 'completed') {
            setResult(jobStatus.anonymizedText || '');
            setStatus('Асинхронная обработка успешно завершена.');
            completed = true;
            break;
          }

          if (currentStatus === 'failed') {
            throw new Error(
              jobStatus.errorMessage || 'Не удалось обработать задачу.'
            );
          }

          setStatus(`Статус задачи: ${jobStatus.status}`);
        }

        if (!completed) {
          throw new Error(
            'Время ожидания результата истекло. Проверьте задачу позже.'
          );
        }
      }
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : 'Произошла ошибка при анонимизации.'
      );
    } finally {
      setLoading(false);
    }
  }

  async function copyResult() {
    try {
      await navigator.clipboard.writeText(result);
      setStatus('Результат скопирован.');
    } catch {
      setError('Не удалось скопировать результат.');
    }
  }

  return (
    <div className="anonymization-page">
      <div className="anonymization-container">
        <div className="anonymization-heading">
          <h1>Анонимизация текста</h1>
          <p>
            Вставьте текст, чтобы автоматически обнаружить и скрыть
            конфиденциальные данные.
          </p>
        </div>

        <form onSubmit={handleSubmit} className="anonymization-form">
          <label>Режим обработки</label>

          <div className="processing-modes">
            <button
              type="button"
              className={mode === 'sync' ? 'mode-button active' : 'mode-button'}
              onClick={() => setMode('sync')}
              disabled={loading}
            >
              Обычная обработка
            </button>

            <button
              type="button"
              className={mode === 'async' ? 'mode-button active' : 'mode-button'}
              onClick={() => setMode('async')}
              disabled={loading}
            >
              Через RabbitMQ
            </button>
          </div>

          <label htmlFor="source-text">Исходный текст</label>

          <textarea
            id="source-text"
            value={text}
            onChange={(event) => setText(event.target.value)}
            placeholder="Введите текст для анонимизации..."
            rows={9}
            required
            disabled={loading}
          />

          <button
            type="submit"
            className="anonymize-button"
            disabled={loading || !text.trim()}
          >
            {loading
              ? 'Обработка...'
              : mode === 'sync'
                ? 'Анонимизировать'
                : 'Отправить в очередь'}
          </button>
        </form>

        {status && (
          <div className="anonymization-status">
            {status}
          </div>
        )}

        {error && (
          <div className="anonymization-error">
            {error}
          </div>
        )}

        {result && (
          <div className="result-section">
            <div className="result-heading">
              <h2>Результат</h2>

              <button type="button" onClick={copyResult}>
                Копировать
              </button>
            </div>

            <div className="result-content">
              {result}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

export default AnonymizationPage;
