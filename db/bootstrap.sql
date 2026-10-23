-- Database bootstrap (MEM-E0-PLAT-03, ADR 0004; messaging: MEM-E0-PLAT-02, ADR 0006): PostGIS, the
-- messaging role and schemas, and one schema plus one role per module.
-- Scope: every environment (local Compose runs it on first start; other environments run it once
-- when the database is created and again after a module is added). Idempotent, not destructive.
-- Run as the database owner:
--   psql -v ON_ERROR_STOP=1 -d memorio -f db/bootstrap.sql   (or tools/db-bootstrap.sh locally)
--
-- SD4: a module reads and writes only its own schema. Each module role owns its schema and has no
-- rights on any other (only INSERT into the outbox tables of schema wolverine); nobody else (PUBLIC)
-- has rights on a module schema. Roles get no password here: each environment sets passwords from its
-- secrets (locally: db/local-passwords.sql).
-- The schema list must match the modules in ModuleCatalog (checked by Memorio.Database.Tests).

CREATE EXTENSION IF NOT EXISTS postgis;

-- public keeps only PostGIS types and functions: usable by every role, writable by none.
REVOKE CREATE ON SCHEMA public FROM PUBLIC;
GRANT USAGE ON SCHEMA public TO PUBLIC;

DO $$
DECLARE
    module_schema text;
    module_role text;
BEGIN
    EXECUTE format('REVOKE CONNECT, TEMPORARY ON DATABASE %I FROM PUBLIC', current_database());

    -- Messaging (ADR 0006, MEM-E0-PLAT-02): Wolverine's outbox, inbox and dead letters (schema wolverine)
    -- and its queues (wolverine_queues) belong to memorio_messaging; the API and the worker connect as
    -- it and Wolverine creates its tables there on startup.
    IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'memorio_messaging') THEN
        CREATE ROLE memorio_messaging LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT;
    END IF;
    EXECUTE format('GRANT CONNECT ON DATABASE %I TO memorio_messaging', current_database());
    CREATE SCHEMA IF NOT EXISTS wolverine AUTHORIZATION memorio_messaging;
    CREATE SCHEMA IF NOT EXISTS wolverine_queues AUTHORIZATION memorio_messaging;
    ALTER SCHEMA wolverine OWNER TO memorio_messaging;
    ALTER SCHEMA wolverine_queues OWNER TO memorio_messaging;
    REVOKE ALL ON SCHEMA wolverine, wolverine_queues FROM PUBLIC;

    FOREACH module_schema IN ARRAY ARRAY[
        'access', 'account', 'ai_narration', 'album', 'archive', 'limits', 'moderation',
        'notifications', 'place_profiles', 'places', 'plans', 'provenance', 'publishing', 'succession'
    ]
    LOOP
        module_role := 'memorio_' || module_schema;

        IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = module_role) THEN
            EXECUTE format('CREATE ROLE %I LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT', module_role);
        END IF;

        EXECUTE format('GRANT CONNECT ON DATABASE %I TO %I', current_database(), module_role);
        EXECUTE format('CREATE SCHEMA IF NOT EXISTS %I AUTHORIZATION %I', module_schema, module_role);
        EXECUTE format('ALTER SCHEMA %I OWNER TO %I', module_schema, module_role);
        EXECUTE format('REVOKE ALL ON SCHEMA %I FROM PUBLIC', module_schema);
        EXECUTE format('ALTER ROLE %I SET search_path = %I, public', module_role, module_schema);

        -- Outbox (ADR 0006): a module stores its integration events in its own transaction, so its role
        -- may add rows to the envelope tables, never read, change or remove them (D-134).
        EXECUTE format('GRANT USAGE ON SCHEMA wolverine TO %I', module_role);
        EXECUTE format(
            'ALTER DEFAULT PRIVILEGES FOR ROLE memorio_messaging IN SCHEMA wolverine GRANT INSERT ON TABLES TO %I',
            module_role);
        EXECUTE format('GRANT INSERT ON ALL TABLES IN SCHEMA wolverine TO %I', module_role);
    END LOOP;
END
$$;
