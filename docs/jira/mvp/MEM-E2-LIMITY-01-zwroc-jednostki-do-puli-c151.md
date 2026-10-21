# MEM-E2-LIMITY-01: Zwróć jednostki do puli (C151)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E2](epiki/E2-wspomnienia-na-prywatnej-mapie-tworzenie-kosz-usuw.md) Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, limity, MEM-E2-LIMITY-01 |
| Moduł | limity |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-LIMITY-01](MEM-E0-LIMITY-01-szkielet-modulu-limity-struktura-publiczny-kontrak.md); [MEM-E3-LIMITY-01](MEM-E3-LIMITY-01-zarezerwuj-jednostki-z-puli-c150.md) |
| Blokuje | [MEM-E2-LIMITY-02](MEM-E2-LIMITY-02-write-port-limity-dla-archiwum-zarezerwuj-zwroc-pr.md) |
| Kod roboczy | MEM-E2-LIMITY-01 |
| Źródło | ES:C151 — przejście 2: S2:B1 „Usunięcie treści zwraca przestrzeń” |

## Cel

Zwróć jednostki do puli (C151) — część epiku E2 „Wspomnienia na prywatnej mapie: tworzenie, kosz, usuwanie”.

## Kontekst

- Moduł **limity** — kluczowe pytanie: Czy ta operacja zmieści się w puli i ile zostało?
- Czego moduł nie wie (nie wolno tego wymagać): skąd limit; kto płaci; czego dotyczy treść
- Projekt modułu: model procesów modułu limity
- Agregat AG2 „Pula”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zwróć jednostki do puli (C151) (źródło: ES:C151 — przejście 2: S2:B1 „Usunięcie treści zwraca przestrzeń”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E2-LIMITY-02)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Zwróć jednostki do puli”, then powstaje „Jednostki zwrócone” (E152). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Niezmiennik AG2 „Pula”: Zużycie ≤ limit; dwie równoległe rezerwacje nie przekroczą puli, nic „na minus” (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG2 „Pula”: Zwrot przestrzeni przy usunięciu; zużyty limit AI nie wraca (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG2 „Pula”: Jedna operacja = jeden skutek (klucz operacji = id treści; D-015). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG2 „Pula”: Limit nadpisuje tylko nowszy (numer kolejny; D-016). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG2 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: warunkowy UPDATE (zużycie + n ≤ limit) + księga operacji z unikalnym kluczem. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- HS-018: Komendy wywoływane tylko przez write port (C150, C151) nie mają aktora ani polityki w gramatyce modelu — es.mjs zgłasza ostrzeżenie. To ograniczenie notacji, nie modelu: wywołującym jest komenda innego kontekstu (C13, C47, C56, C82). — Architekt
