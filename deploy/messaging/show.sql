-- One dead letter in full (MEM-E0-PLAT-20; dead-letters.sh show <id>): the exception and the event.
-- envelope: Wolverine's stored envelope, headers and the event's JSON; bytes that are not text show as \ooo (octal).
-- Integration events carry ids and facts only, never content of users (contracts, D-038).
SET ROLE memorio_messaging;

SELECT id, message_type AS event, received_at AS handler, source, sent_at, replayable,
       exception_type, exception_message, encode(body, 'escape') AS envelope
FROM wolverine.wolverine_dead_letters
WHERE id::text = :'id';
