# MEM-E10-ARCHIWUM-03: Usuń zdjęcie z poczekalni (C23)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E10](epiki/E10-import-zdjec-z-telefonu-po-mvp.md) Import zdjęć z telefonu (po MVP) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E10-ARCHIWUM-03 |
| Moduł | archiwum |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P2 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md); [MEM-E10-ARCHIWUM-04](MEM-E10-ARCHIWUM-04-zapytanie-poczekalnia-do-przypisania-r4.md) |
| Blokuje | [MEM-E10-PLAT-01](MEM-E10-PLAT-01-ekran-s07-import-zdjec-i-poczekalnia.md) |
| Kod roboczy | MEM-E10-ARCHIWUM-03 |
| Źródło | ES:C23 — 2026-10-16, HS-016 |

## Cel

Usuń zdjęcie z poczekalni (C23) — część epiku E10 „Import zdjęć z telefonu (po MVP)”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Usuń zdjęcie z poczekalni (C23) (źródło: ES:C23 — 2026-10-16, HS-016)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E10-PLAT-01)

## Kryteria akceptacji

- [ ] Given aktor widzi: Poczekalnia „do przypisania”, when Użytkownik (autor wspomnień) wykonuje „Usuń zdjęcie z poczekalni”, then powstaje „Treść przeniesiona do kosza” (E20). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
