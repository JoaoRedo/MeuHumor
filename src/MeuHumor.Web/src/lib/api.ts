import { supabase } from './supabase'
import type { CreateMoodPayload, MonthSummary, MoodEntry } from '../types/mood'

const apiBaseUrl = import.meta.env.VITE_API_URL ?? ''

async function getAccessToken(): Promise<string> {
  const { data } = await supabase.auth.getSession()
  const token = data.session?.access_token

  if (!token) {
    throw new Error('Sessão expirada. Faça login novamente.')
  }

  return token
}

async function apiFetch<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = await getAccessToken()

  const response = await fetch(`${apiBaseUrl}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
      ...options.headers,
    },
  })

  if (!response.ok) {
    const body = await response.json().catch(() => ({}))
    const message = (body as { message?: string }).message ?? `Erro ${response.status}`
    throw new Error(message)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return response.json() as Promise<T>
}

export const moodApi = {
  register: (payload: CreateMoodPayload) =>
    apiFetch<MoodEntry>('/api/mood', {
      method: 'POST',
      body: JSON.stringify(payload),
    }),

  getHistory: () => apiFetch<MoodEntry[]>('/api/mood/history'),

  getMonthSummary: (mes: number, ano: number) =>
    apiFetch<MonthSummary>(`/api/mood/month-summary?mes=${mes}&ano=${ano}`),
}
