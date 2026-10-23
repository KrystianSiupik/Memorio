-- Local development only (compose.yaml, tools/db-bootstrap.sh): every Memorio role gets the password
-- memorio_local. Never run in a deployed environment, where passwords come from its secrets (D-129).
DO $$
DECLARE
    memorio_role text;
BEGIN
    FOR memorio_role IN SELECT rolname FROM pg_roles WHERE rolname LIKE 'memorio\_%' AND rolname <> current_user
    LOOP
        EXECUTE format('ALTER ROLE %I PASSWORD %L', memorio_role, 'memorio_local');
    END LOOP;
END
$$;
