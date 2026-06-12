import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import { moodApi } from '../lib/api'
import type { MoodType } from '../types/mood'
import { useAuth } from './AuthContext'

interface MoodTypesContextValue {
  types: MoodType[]
  loading: boolean
  error: string | null
  refresh: () => Promise<void>
  getTypeById: (id: number) => MoodType | undefined
}

const MoodTypesContext = createContext<MoodTypesContextValue | null>(null)

export function MoodTypesProvider({ children }: { children: ReactNode }) {
  const { user } = useAuth()
  const [types, setTypes] = useState<MoodType[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const loadTypes = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await moodApi.getMoodTypes()
      setTypes(data)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro ao carregar tipos de humor.')
      setTypes([])
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    if (user) {
      loadTypes()
    } else {
      setTypes([])
    }
  }, [user, loadTypes])

  const getTypeById = useCallback(
    (id: number) => types.find((type) => type.id === id),
    [types],
  )

  const value = useMemo(
    () => ({ types, loading, error, refresh: loadTypes, getTypeById }),
    [types, loading, error, loadTypes, getTypeById],
  )

  return <MoodTypesContext.Provider value={value}>{children}</MoodTypesContext.Provider>
}

export function useMoodTypes() {
  const context = useContext(MoodTypesContext)
  if (!context) {
    throw new Error('useMoodTypes deve ser usado dentro de MoodTypesProvider')
  }
  return context
}
