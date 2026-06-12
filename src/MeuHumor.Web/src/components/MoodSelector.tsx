import { MOOD_OPTIONS, type MoodValue } from '../types/mood'

interface MoodSelectorProps {
  value: MoodValue | null
  onChange: (value: MoodValue) => void
  disabled?: boolean
}

export function MoodSelector({ value, onChange, disabled }: MoodSelectorProps) {
  return (
    <div className="mood-grid" role="radiogroup" aria-label="Selecione seu humor">
      {MOOD_OPTIONS.map((option) => (
        <button
          key={option.value}
          type="button"
          role="radio"
          aria-checked={value === option.value}
          disabled={disabled}
          className={`mood-option mood-${option.value} ${value === option.value ? 'selected' : ''}`}
          onClick={() => onChange(option.value)}
        >
          <span className="mood-emoji">{option.emoji}</span>
          <span className="mood-label">{option.label}</span>
        </button>
      ))}
    </div>
  )
}
