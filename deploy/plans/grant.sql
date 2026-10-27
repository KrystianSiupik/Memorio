-- A manual line of the entitlement ledger (MEM-E0-PLAT-15, D-188; entitlements.sh grant|end): testers, goodwill,
-- a complaint settled outside the stores. Never an edit of an earlier line: ending a manual entitlement early is
-- a new manual line valid until now, and of the manual source only the latest line counts (EntitlementRules).
-- The reason is required (ck_entitlement_entries_manual_note).
SET ROLE memorio_plans;

INSERT INTO plans.entitlement_entries
    (source, source_reference, resource_kind, resource_id, valid_until, occurred_at, recorded_at, note)
VALUES
    ('manual', gen_random_uuid()::text, :'kind', :'id'::uuid, :'until'::timestamptz, now(), now(), :'note')
RETURNING source_reference, resource_kind, resource_id, valid_until, note;
