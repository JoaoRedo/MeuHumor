-- MeuHumor - Schema inicial para Supabase (PostgreSQL)
-- Execute no SQL Editor do Supabase ou via migration.

-- ---------------------------------------------------------------------------
-- 1. Tabela de perfil (id = UUID do Supabase Auth)
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS public.users (
    id          UUID PRIMARY KEY REFERENCES auth.users (id) ON DELETE CASCADE,
    nome        VARCHAR(255),
    email       VARCHAR(255),
    telefone    VARCHAR(20),
    criado_em   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    atualizado_em TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE public.users IS 'Perfil do usuário; id espelha auth.users.id';

-- ---------------------------------------------------------------------------
-- 2. Registros diários de humor
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS public.mood_entries (
    id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id       UUID NOT NULL REFERENCES public.users (id) ON DELETE CASCADE,
    data          DATE NOT NULL,
    humor         SMALLINT NOT NULL CHECK (humor BETWEEN 1 AND 5),
    observacao    TEXT,
    criado_em     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    atualizado_em TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT mood_entries_user_data_unique UNIQUE (user_id, data)
);

COMMENT ON TABLE public.mood_entries IS 'Um registro de humor por usuário por dia';
COMMENT ON COLUMN public.mood_entries.humor IS '1=Muito Triste, 2=Triste, 3=Neutro, 4=Feliz, 5=Muito Feliz';

CREATE INDEX IF NOT EXISTS idx_mood_entries_user_id ON public.mood_entries (user_id);
CREATE INDEX IF NOT EXISTS idx_mood_entries_data ON public.mood_entries (data DESC);
CREATE INDEX IF NOT EXISTS idx_mood_entries_user_data ON public.mood_entries (user_id, data DESC);

-- ---------------------------------------------------------------------------
-- 3. Relatórios mensais enviados (controle de envio)
-- ---------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS public.reports (
    id                UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id           UUID NOT NULL REFERENCES public.users (id) ON DELETE CASCADE,
    mes               SMALLINT NOT NULL CHECK (mes BETWEEN 1 AND 12),
    ano               SMALLINT NOT NULL CHECK (ano >= 2000),
    media_humor       NUMERIC(4, 2),
    resumo_json       JSONB,
    enviado_email     BOOLEAN NOT NULL DEFAULT FALSE,
    enviado_whatsapp  BOOLEAN NOT NULL DEFAULT FALSE,
    enviado_em        TIMESTAMPTZ,
    criado_em         TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT reports_user_mes_ano_unique UNIQUE (user_id, mes, ano)
);

CREATE INDEX IF NOT EXISTS idx_reports_user_id ON public.reports (user_id);

-- ---------------------------------------------------------------------------
-- Trigger: criar perfil automaticamente ao cadastrar no Auth
-- ---------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.handle_new_user()
RETURNS TRIGGER
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = public
AS $$
BEGIN
    INSERT INTO public.users (id, email, nome)
    VALUES (
        NEW.id,
        NEW.email,
        COALESCE(NEW.raw_user_meta_data ->> 'nome', NEW.raw_user_meta_data ->> 'full_name', '')
    );
    RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS on_auth_user_created ON auth.users;

CREATE TRIGGER on_auth_user_created
    AFTER INSERT ON auth.users
    FOR EACH ROW
    EXECUTE FUNCTION public.handle_new_user();

-- ---------------------------------------------------------------------------
-- Row Level Security (RLS)
-- ---------------------------------------------------------------------------
ALTER TABLE public.users ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.mood_entries ENABLE ROW LEVEL SECURITY;
ALTER TABLE public.reports ENABLE ROW LEVEL SECURITY;

-- users: cada usuário vê/edita apenas o próprio perfil
CREATE POLICY users_select_own ON public.users
    FOR SELECT USING (auth.uid() = id);

CREATE POLICY users_update_own ON public.users
    FOR UPDATE USING (auth.uid() = id);

-- mood_entries: CRUD apenas dos próprios registros
CREATE POLICY mood_entries_select_own ON public.mood_entries
    FOR SELECT USING (auth.uid() = user_id);

CREATE POLICY mood_entries_insert_own ON public.mood_entries
    FOR INSERT WITH CHECK (auth.uid() = user_id);

CREATE POLICY mood_entries_update_own ON public.mood_entries
    FOR UPDATE USING (auth.uid() = user_id);

CREATE POLICY mood_entries_delete_own ON public.mood_entries
    FOR DELETE USING (auth.uid() = user_id);

-- reports: leitura dos próprios relatórios
CREATE POLICY reports_select_own ON public.reports
    FOR SELECT USING (auth.uid() = user_id);
