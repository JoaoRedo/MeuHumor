export interface MoodType {
  id: number
  label: string
  emoji: string | null
  ordem: number
}

export interface MoodEntry {
  id: string
  data: string
  humor: number
  humorLabel: string
  humorEmoji: string | null
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

export interface UpdateMoodPayload {
  humor: number
  observacao?: string
}
