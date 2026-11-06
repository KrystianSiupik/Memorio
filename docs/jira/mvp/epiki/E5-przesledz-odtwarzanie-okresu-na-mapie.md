# E5: Prześledź: odtwarzanie okresu na mapie

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony |
| Cel planu | G1 |
| Priorytet | P0 |
| Moduły | album, archiwum, - |
| Zależy od epików | E2 |
| Estymacja | 5–11 h |
| Źródło | S1 §15 pkt 7, §8 |

## Kryteria ukończenia epiku

- [ ] Wszystkie tickety P0 epiku zamknięte, a testy przejść jego ekranów zielone. *Sprawdzenie: CI + test na urządzeniu*

## Tickety

| Kod | Tytuł | Moduł / warstwa | h | Status |
|---|---|---|---|---|
| [MEM-E5-ALBUM-01](../MEM-E5-ALBUM-01-utworz-album-z-odtworzonego-okresu-c71.md) | Utwórz album z odtworzonego okresu (C71) | album / aplikacja | 2–4 | zrobiony |
| [MEM-E5-ARCHIWUM-01](../MEM-E5-ARCHIWUM-01-read-port-wspomnienia-okresu-przecinajace-okres-w-.md) | Read port: Wspomnienia okresu (przecinające okres, w kolejności użytkownika) (R7) | archiwum / kontrakt | 1–3 | zrobiony |
| [MEM-E5-PLAT-01](../MEM-E5-PLAT-01-ekran-s08-przesledz-okres.md) | Ekran S08 „Prześledź okres” | - / UI | 2–4 | zrobiony (bez testu na urządzeniu i na prawdziwej mapie, D-302) |

**Po epiku (D-303):** wspomnienie z dokładnością „dzień” ma nieobowiązkową godzinę; w obrębie dnia kolejność po godzinie, bez godziny na końcu.
