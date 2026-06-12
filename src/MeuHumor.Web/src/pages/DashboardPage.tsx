import { FormEvent, useEffect, useState } from 'react'

import { moodApi } from '../lib/api'

import { MoodSelector } from '../components/MoodSelector'

import { useMoodTypes } from '../context/MoodTypesContext'

import type { MoodEntry, MonthSummary } from '../types/mood'



function formatDate(date: string) {

  return new Date(date + 'T12:00:00').toLocaleDateString('pt-BR', {

    weekday: 'long',

    day: '2-digit',

    month: 'long',

  })

}



export function DashboardPage() {

  const { types, loading: typesLoading, error: typesError } = useMoodTypes()

  const today = new Date()

  const [humor, setHumor] = useState<number | null>(null)

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

  const isEditing = todayEntry !== null

  const hasChanges =
    isEditing &&
    todayEntry !== null &&
    (humor !== todayEntry.humor ||
      observacao.trim() !== (todayEntry.observacao ?? '').trim())



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

          setHumor(entryToday.humor)

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



    setSaving(true)

    setError(null)

    setSuccess(null)



    const payload = {

      humor,

      observacao: observacao.trim() || undefined,

    }



    try {

      const saved = isEditing

        ? await moodApi.updateToday(payload)

        : await moodApi.register(payload)



      setTodayEntry(saved)

      setSuccess(isEditing ? 'Humor atualizado com sucesso!' : 'Humor registrado com sucesso!')

      const monthSummary = await moodApi.getMonthSummary(mes, ano)

      setSummary(monthSummary)

    } catch (err) {

      setError(err instanceof Error ? err.message : 'Erro ao salvar registro.')

    } finally {

      setSaving(false)

    }

  }



  if (loading || typesLoading) {

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

          {isEditing

            ? 'Você já registrou seu humor hoje. Pode alterar até o fim do dia.'

            : 'Como você está se sentindo hoje?'}

        </p>

      </section>



      {(error || typesError) && <div className="alert alert-error">{error ?? typesError}</div>}



      <form className="card mood-form" onSubmit={handleSubmit}>

        <h2>{isEditing ? 'Editar humor de hoje' : 'Seu humor'}</h2>

        <MoodSelector types={types} value={humor} onChange={setHumor} disabled={saving} />



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



        {success && <div className="alert alert-success">{success}</div>}



        <button
          type="submit"
          className="btn btn-primary"
          disabled={saving || !humor || types.length === 0 || (isEditing && !hasChanges)}
        >

          {saving ? 'Salvando...' : isEditing ? 'Salvar alterações' : 'Registrar humor'}

        </button>

      </form>



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

            {types.map((option) => {

              const count = summary.contagemPorCategoria[option.label] ?? 0

              const max = Math.max(...Object.values(summary.contagemPorCategoria), 1)

              const width = (count / max) * 100



              return (

                <div key={option.id} className="summary-bar-row">

                  <span className="summary-bar-label">

                    {option.emoji} {option.label}

                  </span>

                  <div className="summary-bar-track">

                    <div className={`summary-bar-fill mood-${option.id}`} style={{ width: `${width}%` }} />

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


