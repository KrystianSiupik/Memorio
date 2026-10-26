-- Deletes one dead letter on purpose (MEM-E0-PLAT-20; dead-letters.sh discard <id> --reason "..."),
-- returning what the operations log records.
SET ROLE memorio_messaging;

DELETE FROM wolverine.wolverine_dead_letters
WHERE id::text = :'id'
RETURNING id, message_type, received_at, exception_type;
