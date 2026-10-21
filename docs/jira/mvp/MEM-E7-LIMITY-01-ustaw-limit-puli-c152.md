# MEM-E7-LIMITY-01: Ustaw limit puli (C152)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E7](epiki/E7-plany-limity-i-retencja-24-miesiace.md) Plany, limity i retencja 24 miesiące |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, limity, MEM-E7-LIMITY-01 |
| Moduł | limity |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-LIMITY-01](MEM-E0-LIMITY-01-szkielet-modulu-limity-struktura-publiczny-kontrak.md); [MEM-E3-LIMITY-01](MEM-E3-LIMITY-01-zarezerwuj-jednostki-z-puli-c150.md) |
| Blokuje | [MEM-E7-LIMITY-02](MEM-E7-LIMITY-02-polityka-p27-zawsze-gdy-plan-aktywny-albo-przelacz.md) |
| Kod roboczy | MEM-E7-LIMITY-01 |
| Źródło | ES:C152 — przejście 2: Plany → Limity zdarzeniem ([U] 2026-10-16: kontrakty skutków jako zdarzenia) |

## Cel

Ustaw limit puli (C152) — część epiku E7 „Plany, limity i retencja 24 miesiące”.

## Kontekst

- Moduł **limity** — kluczowe pytanie: Czy ta operacja zmieści się w puli i ile zostało?
- Czego moduł nie wie (nie wolno tego wymagać): skąd limit; kto płaci; czego dotyczy treść
- Projekt modułu: model procesów modułu limity
- Agregat AG2 „Pula”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Ustaw limit puli (C152) (źródło: ES:C152 — przejście 2: Plany → Limity zdarzeniem ([U] 2026-10-16: kontrakty skutków jako zdarzenia))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E7-LIMITY-02)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Ustaw limit puli”, then powstaje „Limit puli ustawiony” (E153). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Niezmiennik AG2 „Pula”: Zużycie ≤ limit; dwie równoległe rezerwacje nie przekroczą puli, nic „na minus” (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG2 „Pula”: Zwrot przestrzeni przy usunięciu; zużyty limit AI nie wraca (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG2 „Pula”: Jedna operacja = jeden skutek (klucz operacji = id treści; D-015). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG2 „Pula”: Limit nadpisuje tylko nowszy (numer kolejny; D-016). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG2 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: warunkowy UPDATE (zużycie + n ≤ limit) + księga operacji z unikalnym kluczem. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
