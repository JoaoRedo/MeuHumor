-- Cria tabela de tipos de humor e vincula mood_entries por FK.
-- Execute no SQL Editor do Supabase.

CREATE TABLE IF NOT EXISTS public.mood_types (
    id            SMALLINT PRIMARY KEY,
    label         VARCHAR(80) NOT NULL,
    emoji         VARCHAR(10),
    ordem         SMALLINT NOT NULL,
    ativo         BOOLEAN NOT NULL DEFAULT TRUE,
    criado_em     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    atualizado_em TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE public.mood_types IS 'Catálogo de tipos de humor (editável sem deploy)';

INSERT INTO public.mood_types (id, label, emoji, ordem) VALUES
    (1, 'Muito Triste', '😢', 1),
    (2, 'Triste',       '😔', 2),
    (3, 'Neutro',       '😐', 3),
    (4, 'Feliz',        '🙂', 4),
    (5, 'Muito Feliz',  '😄', 5),
    (6, 'Ansioso(a)',   '😰', 6),
    (7, 'Bravo(a)',     '😠', 7)
ON CONFLICT (id) DO UPDATE SET
    label = EXCLUDED.label,
    emoji = EXCLUDED.emoji,
    ordem = EXCLUDED.ordem,
    atualizado_em = NOW();

ALTER TABLE public.mood_entries
    DROP CONSTRAINT IF EXISTS mood_entries_humor_check;

ALTER TABLE public.mood_entries
    DROP CONSTRAINT IF EXISTS mood_entries_humor_fkey;

ALTER TABLE public.mood_entries
    ADD CONSTRAINT mood_entries_humor_fkey
    FOREIGN KEY (humor) REFERENCES public.mood_types (id);

ALTER TABLE public.mood_entries
    ALTER COLUMN humor SET NOT NULL;

COMMENT ON COLUMN public.mood_entries.humor IS 'FK para public.mood_types.id';

ALTER TABLE public.mood_types ENABLE ROW LEVEL SECURITY;

DROP POLICY IF EXISTS mood_types_select_all ON public.mood_types;

CREATE POLICY mood_types_select_all ON public.mood_types
    FOR SELECT USING (true);
