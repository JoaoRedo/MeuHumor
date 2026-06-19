-- Exibe timestamps no fuso de São Paulo no SQL Editor e em novas conexões.
-- O valor continua armazenado corretamente; apenas a exibição muda.
-- Execute no SQL Editor do Supabase.

ALTER DATABASE postgres SET timezone TO 'America/Sao_Paulo';

-- Confirme (abra uma nova query depois de executar):
-- SHOW timezone;
-- SELECT now();
