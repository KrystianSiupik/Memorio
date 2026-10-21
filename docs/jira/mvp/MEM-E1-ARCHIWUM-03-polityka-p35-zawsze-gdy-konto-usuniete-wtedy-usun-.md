# MEM-E1-ARCHIWUM-03: Polityka P35: Zawsze gdy konto usunięte, wtedy usuń treści tej osoby

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E1](epiki/E1-konto-rejestracja-16-usuniecie-z-oknem-30-dni.md) Konto: rejestracja 16+, usunięcie z oknem 30 dni |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E1-ARCHIWUM-03 |
| Moduł | archiwum |
| Warstwa | aplikacja (handler zdarzenia) |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md); [MEM-E1-ARCHIWUM-01](MEM-E1-ARCHIWUM-01-usun-tresci-konta-c25.md); [MEM-E1-KONTO-08](MEM-E1-KONTO-08-zdarzenie-integracyjne-konto-usuniete-e7-w-kontrak.md) |
| Blokuje | - |
| Kod roboczy | MEM-E1-ARCHIWUM-03 |
| Źródło | ES:P35 — przejście 2: S2:B3 KOMENDA B |

## Cel

Polityka P35: Zawsze gdy konto usunięte, wtedy usuń treści tej osoby — część epiku E1 „Konto: rejestracja 16+, usunięcie z oknem 30 dni”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Polityka P35: Zawsze gdy konto usunięte, wtedy usuń treści tej osoby (źródło: ES:P35 — przejście 2: S2:B3 KOMENDA B)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (zob. epik)

## Kryteria akceptacji

- [ ] Given zdarzenie E7, when handler je przetwarza, then wywołana jest komenda C25. *Sprawdzenie: test integracyjny handlera*
- [ ] Podwójne dostarczenie zdarzenia nie daje drugiego skutku (inbox, ADR 0006). *Sprawdzenie: test idempotentności*
- [ ] Zdarzenie starsze niż ostatnio zastosowane dla tej samej pary (zasób, osoba) jest pomijane (D-013). *Sprawdzenie: test jednostkowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
