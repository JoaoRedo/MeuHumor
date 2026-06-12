export const MOOD_OPTIONS = [
  { value: 1, label: 'Muito Triste', emoji: '😢' },
  { value: 2, label: 'Triste', emoji: '😔' },
  { value: 3, label: 'Neutro', emoji: '😐' },
  { value: 4, label: 'Feliz', emoji: '🙂' },
  { value: 5, label: 'Muito Feliz', emoji: '😄' },
] as const

export type MoodValue = (typeof MOOD_OPTIONS)[number]['value']

export interface MoodEntry {
  id: string
  data: string
  humor: number
  humorLabel: string
  observacao: string | null
  criadoEm: string
}

export interface MonthSummary {
  mes: number
  ano: number
  mediaHumor: number
  totalDiasRegistrados: number
  contagemPorCategoria: Record<string, number>
}

export interface CreateMoodPayload {
  humor: number
  observacao?: string
  data?: string
}
