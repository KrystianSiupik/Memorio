-- The ledger lines of one account or album (MEM-E0-PLAT-15; entitlements.sh list), oldest first. Of each
-- source only the latest line counts; the plan is paid until the latest of those (EntitlementRules).
SET ROLE memorio_plans;

SELECT source, source_reference, valid_until, occurred_at, recorded_at, note
FROM plans.entitlement_entries
WHERE resource_kind = :'kind' AND resource_id::text = :'id'
ORDER BY occurred_at, recorded_at;
