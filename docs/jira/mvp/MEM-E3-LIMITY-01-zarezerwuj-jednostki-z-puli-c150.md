# MEM-E3-LIMITY-01: Zarezerwuj jednostki z puli (C150)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E3](epiki/E3-album-wspoldzielony-sklad-wklady-zgody.md) Album współdzielony: skład, wkłady, zgody |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, limity, MEM-E3-LIMITY-01 |
| Moduł | limity |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-LIMITY-01](MEM-E0-LIMITY-01-szkielet-modulu-limity-struktura-publiczny-kontrak.md) |
| Blokuje | [MEM-E2-LIMITY-01](MEM-E2-LIMITY-01-zwroc-jednostki-do-puli-c151.md); [MEM-E7-LIMITY-01](MEM-E7-LIMITY-01-ustaw-limit-puli-c152.md); [MEM-E2-LIMITY-02](MEM-E2-LIMITY-02-write-port-limity-dla-archiwum-zarezerwuj-zwroc-pr.md); [MEM-E3-LIMITY-03](MEM-E3-LIMITY-03-write-port-limity-dla-album-zarezerwuj-przestrzen-.md); [MEM-E2-LIMITY-03](MEM-E2-LIMITY-03-write-port-limity-dla-narracja-ai-zuzyj-limit-ai.md) |
| Kod roboczy | MEM-E3-LIMITY-01 |
| Źródło | ES:C150 — przejście 2: Limity z mapy (S5) nie miały komend w przejściu 1; wywoływane przez write port z Archiwum, Albumu i Narracji AI |

## Cel

Zarezerwuj jednostki z puli (C150) — część epiku E3 „Album współdzielony: skład, wkłady, zgody”.

## Kontekst

- Moduł **limity** — kluczowe pytanie: Czy ta operacja zmieści się w puli i ile zostało?
- Czego moduł nie wie (nie wolno tego wymagać): skąd limit; kto płaci; czego dotyczy treść
- Projekt modułu: model procesów modułu limity
- Agregat AG2 „Pula”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zarezerwuj jednostki z puli (C150) (źródło: ES:C150 — przejście 2: Limity z mapy (S5) nie miały komend w przejściu 1; wywoływane przez write port z Archiwum, Albumu i Narracji AI)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E2-LIMITY-01, MEM-E7-LIMITY-01, MEM-E2-LIMITY-02)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Zarezerwuj jednostki z puli”, then powstaje „Jednostki zarezerwowane” (E150). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Given warunek odrzucenia, when „Zarezerwuj jednostki z puli”, then „Pula pełna” (E151) i brak zapisu. *Sprawdzenie: test jednostkowy*
- [ ] Niezmiennik AG2 „Pula”: Zużycie ≤ limit; dwie równoległe rezerwacje nie przekroczą puli, nic „na minus” (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG2 „Pula”: Zwrot przestrzeni przy usunięciu; zużyty limit AI nie wraca (S2:B1). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG2 „Pula”: Jedna operacja = jeden skutek (klucz operacji = id treści; D-015). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG2 „Pula”: Limit nadpisuje tylko nowszy (numer kolejny; D-016). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG2 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: warunkowy UPDATE (zużycie + n ≤ limit) + księga operacji z unikalnym kluczem. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- HS-018: Komendy wywoływane tylko przez write port (C150, C151) nie mają aktora ani polityki w gramatyce modelu — es.mjs zgłasza ostrzeżenie. To ograniczenie notacji, nie modelu: wywołującym jest komenda innego kontekstu (C13, C47, C56, C82). — Architekt
