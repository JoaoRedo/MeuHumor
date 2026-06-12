-- Adiciona Ansioso(a) = 6 e Bravo(a) = 7 na escala de humor.
-- Execute no SQL Editor do Supabase se o schema já foi criado com a escala 1-5.

ALTER TABLE public.mood_entries
    DROP CONSTRAINT IF EXISTS mood_entries_humor_check;

ALTER TABLE public.mood_entries
    ADD CONSTRAINT mood_entries_humor_check CHECK (humor BETWEEN 1 AND 7);

COMMENT ON COLUMN public.mood_entries.humor IS
    '1=Muito Triste, 2=Triste, 3=Neutro, 4=Feliz, 5=Muito Feliz, 6=Ansioso(a), 7=Bravo(a)';
