
import API_URL from './api';

export interface AnonymizationHistory {
    id: number;
    originalText: string;
    anonymizedText: string;
    createdAt: string;
    detectedCounts: Record<string, number>;
}

export interface HistoryPage {
    items: AnonymizationHistory[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
}

export async function getHistory(
    page: number,
    pageSize: number,
    token: string
): Promise<HistoryPage> {
    const response = await fetch(
        `${API_URL}/Anonymization/history?page=${page}&pageSize=${pageSize}`,
        {
            method: 'GET',
            headers: {
                Authorization: `Bearer ${token}`,
            },
        }
    );

    const result = await response.json().catch(() => null);

    if (!response.ok) {
        throw new Error(
            result?.message ||
            result?.error ||
            'Не удалось загрузить историю операций.'
        );
    }

    return result as HistoryPage;
}
