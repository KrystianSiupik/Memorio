-- Database bootstrap (MEM-E0-PLAT-03, ADR 0004): PostGIS, and one schema plus one role per module.
-- Scope: every environment (local Compose runs it on first start; other environments run it once
-- when the database is created and again after a module is added). Idempotent, not destructive.
-- Run as the database owner:
--   psql -v ON_ERROR_STOP=1 -d memorio -f db/bootstrap.sql   (or tools/db-bootstrap.sh locally)
--
-- SD4: a module reads and writes only its own schema. Each module role owns its schema and has no
-- rights on any other; nobody else (PUBLIC) has rights on a module schema. Roles get no password
-- here: each environment sets passwords from its secrets when the API first connects as a module.
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
    END LOOP;
END
$$;
