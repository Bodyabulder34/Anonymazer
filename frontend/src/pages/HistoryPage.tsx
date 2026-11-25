
import { useEffect, useState } from 'react';
import { useAuth } from '../context/AuthContext';
import {
    getHistory,
    type AnonymizationHistory,
} from '../services/historyService';

import './HistoryPage.css';

function HistoryPage() {
    const { token } = useAuth();

    const [items, setItems] = useState<AnonymizationHistory[]>([]);
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(0);
    const [totalCount, setTotalCount] = useState(0);

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        if (!token) return;

        let cancelled = false;

        async function loadHistory() {
            setLoading(true);
            setError('');

            try {
                const response = await getHistory(page, 5, token!);

                if (cancelled) return;

                setItems(response.items);
                setTotalPages(response.totalPages);
                setTotalCount(response.totalCount);
            } catch (err) {
                if (cancelled) return;

                setError(
                    err instanceof Error
                        ? err.message
                        : 'Не удалось загрузить историю.'
                );
            } finally {
                if (!cancelled) setLoading(false);
            }
        }

        loadHistory();

        return () => {
            cancelled = true;
        };
    }, [page, token]);

    function formatDate(date: string) {
        return new Date(date).toLocaleString('ru-RU', {
            dateStyle: 'medium',
            timeStyle: 'short',
        });
    }

    return (
        <div className="history-page">
            <div className="history-container">
                <div className="history-heading">
                    <h1>История операций</h1>
                    <p>Все ваши предыдущие операции анонимизации в одном месте.</p>
                </div>

                <div className="history-summary">
                    Всего операций: <strong>{totalCount}</strong>
                </div>

                {loading && (
                    <div className="history-message">
                        Загрузка истории...
                    </div>
                )}

                {error && (
                    <div className="history-error">
                        {error}
                    </div>
                )}

                {!loading && !error && items.length === 0 && (
                    <div className="history-empty">
                        История пока пуста. Выполните первую анонимизацию!
                    </div>
                )}

                {!loading && !error && items.map((item) => (
                    <article className="history-card" key={item.id}>
                        <div className="history-card-header">
                            <span>Операция #{item.id}</span>
                            <time>{formatDate(item.createdAt)}</time>
                        </div>

                        <div className="history-text-block">
                            <h3>Исходный текст</h3>
                            <p>{item.originalText}</p>
                        </div>

                        <div className="history-text-block result">
                            <h3>Анонимизированный текст</h3>
                            <p>{item.anonymizedText}</p>
                        </div>

                        <div className="detected-counts">
                            <h3>Обнаруженные данные</h3>

                            {Object.entries(item.detectedCounts || {}).length === 0 ? (
                                <span>Данные не обнаружены</span>
                            ) : (
                                Object.entries(item.detectedCounts).map(([name, count]) => (
                                    <span className="count-item" key={name}>
                                        {name}: <strong>{count}</strong>
                                    </span>
                                ))
                            )}
                        </div>
                    </article>
                ))}

                {!loading && !error && totalPages > 1 && (
                    <div className="history-pagination">
                        <button
                            onClick={() => setPage((current) => current - 1)}
                            disabled={page <= 1}
                        >
                            Назад
                        </button>

                        <span>
                            Страница {page} из {totalPages}
                        </span>

                        <button
                            onClick={() => setPage((current) => current + 1)}
                            disabled={page >= totalPages}
                        >
                            Далее
                        </button>
                    </div>
                )}
            </div>
        </div>
    );
}

export default HistoryPage;
