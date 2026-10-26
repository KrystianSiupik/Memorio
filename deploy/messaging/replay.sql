-- Marks dead letters for replay (MEM-E0-PLAT-20; dead-letters.sh replay <id>|--all): one by id, or every
-- one when id is 'all'. The worker's durability agent moves marked rows back to its inbox within seconds,
-- and the module handler runs again (it is idempotent, SD6).
SET ROLE memorio_messaging;

UPDATE wolverine.wolverine_dead_letters
SET replayable = true
WHERE (:'id' = 'all' OR id::text = :'id') AND replayable IS NOT TRUE
RETURNING id;
