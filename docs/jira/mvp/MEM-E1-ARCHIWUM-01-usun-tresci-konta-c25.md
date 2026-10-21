# MEM-E1-ARCHIWUM-01: Usuń treści konta (C25)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E1](epiki/E1-konto-rejestracja-16-usuniecie-z-oknem-30-dni.md) Konto: rejestracja 16+, usunięcie z oknem 30 dni |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E1-ARCHIWUM-01 |
| Moduł | archiwum |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P0 |
| Estymacja | 2–4 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md) |
| Blokuje | [MEM-E1-ARCHIWUM-03](MEM-E1-ARCHIWUM-03-polityka-p35-zawsze-gdy-konto-usuniete-wtedy-usun-.md) |
| Kod roboczy | MEM-E1-ARCHIWUM-01 |
| Źródło | ES:C25 — przejście 2: S2:B3: usunięcie konta — Archiwum usuwa treści autora |

## Cel

Usuń treści konta (C25) — część epiku E1 „Konto: rejestracja 16+, usunięcie z oknem 30 dni”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Usuń treści konta (C25) (źródło: ES:C25 — przejście 2: S2:B3: usunięcie konta — Archiwum usuwa treści autora)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E1-ARCHIWUM-03)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Usuń treści konta”, then powstaje „Treść usunięta definitywnie” (E22). *Sprawdzenie: test jednostkowy handlera*
- [ ] Ponowienie tej samej operacji (ten sam klucz/zdarzenie) nie daje drugiego skutku; brak odpowiedzi drugiej strony kończy się stanem opisanym w kontraktach modułów. *Sprawdzenie: test integracyjny z podwójnym wywołaniem*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
