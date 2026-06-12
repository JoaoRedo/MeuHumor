import { FormEvent, useEffect, useState } from 'react'
import { moodApi } from '../lib/api'
import { MoodSelector } from '../components/MoodSelector'
import { MOOD_OPTIONS, type MoodEntry, type MoodValue, type MonthSummary } from '../types/mood'

function formatDate(date: string) {
  return new Date(date + 'T12:00:00').toLocaleDateString('pt-BR', {
    weekday: 'long',
    day: '2-digit',
    month: 'long',
  })
}

function getMoodEmoji(humor: number) {
  return MOOD_OPTIONS.find((o) => o.value === humor)?.emoji ?? '😐'
}

export function DashboardPage() {
  const today = new Date()
  const [humor, setHumor] = useState<MoodValue | null>(null)
  const [observacao, setObservacao] = useState('')
  const [todayEntry, setTodayEntry] = useState<MoodEntry | null>(null)
  const [summary, setSummary] = useState<MonthSummary | null>(null)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  const todayIso = today.toISOString().slice(0, 10)
  const mes = today.getMonth() + 1
  const ano = today.getFullYear()

  useEffect(() => {
    async function loadData() {
      setLoading(true)
      setError(null)

      try {
        const [history, monthSummary] = await Promise.all([
          moodApi.getHistory(),
          moodApi.getMonthSummary(mes, ano),
        ])

        const entryToday = history.find((e) => e.data === todayIso) ?? null
        setTodayEntry(entryToday)
        setSummary(monthSummary)

        if (entryToday) {
          setHumor(entryToday.humor as MoodValue)
          setObservacao(entryToday.observacao ?? '')
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados.')
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [todayIso, mes, ano])

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()

    if (!humor) {
      setError('Selecione como você está se sentindo.')
      return
    }

    if (todayEntry) {
      setError('Você já registrou seu humor hoje.')
      return
    }

    setSaving(true)
    setError(null)
    setSuccess(null)

    try {
      const created = await moodApi.register({
        humor,
        observacao: observacao.trim() || undefined,
      })

      setTodayEntry(created)
      setSuccess('Humor registrado com sucesso!')
      const monthSummary = await moodApi.getMonthSummary(mes, ano)
      setSummary(monthSummary)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro ao salvar registro.')
    } finally {
      setSaving(false)
    }
  }

  if (loading) {
    return (
      <div className="page-center">
        <div className="spinner" aria-label="Carregando" />
      </div>
    )
  }

  return (
    <div className="page-content">
      <section className="card hero-card">
        <p className="eyebrow">Hoje</p>
        <h1>{formatDate(todayIso)}</h1>
        <p className="subtitle">
          {todayEntry
            ? 'Você já registrou seu humor de hoje. Volte amanhã!'
            : 'Como você está se sentindo hoje?'}
        </p>
      </section>

      {todayEntry ? (
        <section className="card today-done">
          <div className="today-mood">
            <span className="today-emoji">{getMoodEmoji(todayEntry.humor)}</span>
            <div>
              <h2>{todayEntry.humorLabel}</h2>
              {todayEntry.observacao && <p className="today-note">{todayEntry.observacao}</p>}
            </div>
          </div>
        </section>
      ) : (
        <form className="card mood-form" onSubmit={handleSubmit}>
          <h2>Seu humor</h2>
          <MoodSelector value={humor} onChange={setHumor} disabled={saving} />

          <label className="field">
            <span>Observação do dia (opcional)</span>
            <textarea
              value={observacao}
              onChange={(e) => setObservacao(e.target.value)}
              placeholder="Como foi seu dia? Escreva um diário breve..."
              rows={5}
              disabled={saving}
            />
          </label>

          {error && <div className="alert alert-error">{error}</div>}
          {success && <div className="alert alert-success">{success}</div>}

          <button type="submit" className="btn btn-primary" disabled={saving || !humor}>
            {saving ? 'Salvando...' : 'Registrar humor'}
          </button>
        </form>
      )}

      {summary && (
        <section className="card summary-card">
          <h2>Resumo de {today.toLocaleDateString('pt-BR', { month: 'long', year: 'numeric' })}</h2>
          <div className="summary-stats">
            <div className="stat">
              <span className="stat-value">{summary.mediaHumor.toFixed(1)}</span>
              <span className="stat-label">Média</span>
            </div>
            <div className="stat">
              <span className="stat-value">{summary.totalDiasRegistrados}</span>
              <span className="stat-label">Dias registrados</span>
            </div>
          </div>

          <div className="summary-bars">
            {MOOD_OPTIONS.map((option) => {
              const count = summary.contagemPorCategoria[option.label] ?? 0
              const max = Math.max(...Object.values(summary.contagemPorCategoria), 1)
              const width = (count / max) * 100

              return (
                <div key={option.value} className="summary-bar-row">
                  <span className="summary-bar-label">
                    {option.emoji} {option.label}
                  </span>
                  <div className="summary-bar-track">
                    <div className={`summary-bar-fill mood-${option.value}`} style={{ width: `${width}%` }} />
                  </div>
                  <span className="summary-bar-count">{count}</span>
                </div>
              )
            })}
          </div>
        </section>
      )}
    </div>
  )
}
