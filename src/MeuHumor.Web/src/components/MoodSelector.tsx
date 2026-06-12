import type { MoodType } from '../types/mood'

interface MoodSelectorProps {
  types: MoodType[]
  value: number | null
  onChange: (value: number) => void
  disabled?: boolean
}

export function MoodSelector({ types, value, onChange, disabled }: MoodSelectorProps) {
  return (
    <div className="mood-grid" role="radiogroup" aria-label="Selecione seu humor">
      {types.map((option) => (
        <button
          key={option.id}
          type="button"
          role="radio"
          aria-checked={value === option.id}
          disabled={disabled}
          className={`mood-option mood-${option.id} ${value === option.id ? 'selected' : ''}`}
          onClick={() => onChange(option.id)}
        >
          <span className="mood-emoji">{option.emoji ?? '😐'}</span>
          <span className="mood-label">{option.label}</span>
        </button>
      ))}
    </div>
  )
}
