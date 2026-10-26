-- Dead letters by event, handler and exception (MEM-E0-PLAT-20; dead-letters.sh list), then the messages.
-- handler = the worker's queue of one module handler (received_at); sent = when the event was sent.
SET ROLE memorio_messaging;

SELECT message_type AS event, received_at AS handler, exception_type AS exception,
       count(*) AS messages, min(sent_at) AS oldest_sent
FROM wolverine.wolverine_dead_letters
GROUP BY message_type, received_at, exception_type
ORDER BY oldest_sent;

SELECT id, message_type AS event, received_at AS handler, exception_type AS exception, sent_at, replayable
FROM wolverine.wolverine_dead_letters
ORDER BY sent_at
LIMIT 100;
