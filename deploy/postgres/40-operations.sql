-- Operations schema of a deployed database (MEM-E0-PLAT-09): owned by the database owner, no module
-- role and nobody else (PUBLIC) has rights on it. restore_markers: the restore test writes a marker
-- just before it picks its recovery target, and the restored copy must contain it (SD13, QS2).
CREATE SCHEMA IF NOT EXISTS operations;
REVOKE ALL ON SCHEMA operations FROM PUBLIC;

CREATE TABLE IF NOT EXISTS operations.restore_markers (
    id uuid PRIMARY KEY,
    created_at timestamptz NOT NULL DEFAULT clock_timestamp()
);
