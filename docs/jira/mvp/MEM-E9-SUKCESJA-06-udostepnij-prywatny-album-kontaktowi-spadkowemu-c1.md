# MEM-E9-SUKCESJA-06: Udostępnij prywatny album kontaktowi spadkowemu (C125)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E9](epiki/E9-sukcesja-i-przejecie-albumu-nastepca-albo-inny-ucz.md) Sukcesja i przejęcie albumu (następca albo inny uczestnik) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, sukcesja, MEM-E9-SUKCESJA-06 |
| Moduł | sukcesja |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-SUKCESJA-01](MEM-E0-SUKCESJA-01-szkielet-modulu-sukcesja-struktura-publiczny-kontr.md) |
| Blokuje | [MEM-E9-SUKCESJA-10](MEM-E9-SUKCESJA-10-polityka-p23-zawsze-gdy-zgon-potwierdzony-wtedy-ud.md); [MEM-E9-SUKCESJA-11](MEM-E9-SUKCESJA-11-zdarzenie-integracyjne-album-prywatny-udostepniony.md) |
| Kod roboczy | MEM-E9-SUKCESJA-06 |
| Źródło | ES:C125 — S2:C8 |

## Cel

Udostępnij prywatny album kontaktowi spadkowemu (C125) — część epiku E9 „Sukcesja i przejęcie albumu (następca albo inny uczestnik)”.

## Kontekst

- Moduł **sukcesja** — kluczowe pytanie: Kto przejmuje album lub odczyt, gdy właściciela już nie ma?
- Czego moduł nie wie (nie wolno tego wymagać): budowa albumu; mechanika dostępu
- Projekt modułu: model procesów modułu sukcesja
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Udostępnij prywatny album kontaktowi spadkowemu (C125) (źródło: ES:C125 — S2:C8)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E9-SUKCESJA-10, MEM-E9-SUKCESJA-11)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Udostępnij prywatny album kontaktowi spadkowemu”, then powstaje „Album prywatny udostępniony kontaktowi spadkowemu” (E126). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
