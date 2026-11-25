import API_URL from './api';

export interface AnonymizationResult {
  originalText: string;
  anonymizedText: string;
  emailsFound: number;
  phonesFound: number;
  passportsFound: number;
}

export async function anonymizeText(text: string, token: string) {
  const response = await fetch(`${API_URL}/Anonymization`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ text }),
  });

  const result = await response.json().catch(() => null);

  if (!response.ok) {
    throw new Error(
      result?.message || 'Не удалось анонимизировать текст.'
    );
  }

  return result as AnonymizationResult;
}
export async function createAsyncAnonymization(
  text: string,
  token: string
) {
  const response = await fetch(`${API_URL}/Anonymization/async`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ text }),
  });

  const result = await response.json().catch(() => null);

  if (!response.ok) {
    throw new Error(
      result?.message || 'Не удалось отправить задачу на обработку.'
    );
  }

  return result;
}
export async function getAsyncJobStatus(
  jobId: string,
  token: string
) {
  const response = await fetch(
    `${API_URL}/Anonymization/jobs/${jobId}`,
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
      result?.message || 'Не удалось получить статус задачи.'
    );
  }

  return result;
}