import { useEffect, useState } from 'react'
import { moodApi } from '../lib/api'
import type { MoodEntry } from '../types/mood'

function formatDate(date: string) {
  return new Date(date + 'T12:00:00').toLocaleDateString('pt-BR', {
    weekday: 'short',
    day: '2-digit',
    month: 'short',
    year: 'numeric',
  })
}

export function HistoryPage() {
  const [entries, setEntries] = useState<MoodEntry[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    async function loadHistory() {
      setLoading(true)
      setError(null)

      try {
        const history = await moodApi.getHistory()
        setEntries(history)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar histórico.')
      } finally {
        setLoading(false)
      }
    }

    loadHistory()
  }, [])

  if (loading) {
    return (
      <div className="page-center">
        <div className="spinner" aria-label="Carregando" />
      </div>
    )
  }

  return (
    <div className="page-content">
      <section className="card hero-card compact">
        <h1>Histórico</h1>
        <p className="subtitle">Todos os seus registros de humor, do mais recente ao mais antigo.</p>
      </section>

      {error && <div className="alert alert-error">{error}</div>}

      {entries.length === 0 ? (
        <section className="card empty-state">
          <p>Nenhum registro ainda. Volte à página inicial e registre seu humor de hoje.</p>
        </section>
      ) : (
        <section className="history-list">
          {entries.map((entry) => (
            <article key={entry.id} className={`card history-item mood-border-${entry.humor}`}>
              <div className="history-item-header">
                <span className="history-emoji">{entry.humorEmoji ?? '😐'}</span>
                <div>
                  <h3>{entry.humorLabel}</h3>
                  <time dateTime={entry.data}>{formatDate(entry.data)}</time>
                </div>
              </div>
              {entry.observacao && <p className="history-note">{entry.observacao}</p>}
            </article>
          ))}
        </section>
      )}
    </div>
  )
}
