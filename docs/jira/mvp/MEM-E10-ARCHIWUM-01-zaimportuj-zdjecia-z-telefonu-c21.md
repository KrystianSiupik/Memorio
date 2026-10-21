# MEM-E10-ARCHIWUM-01: Zaimportuj zdjęcia z telefonu (C21)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E10](epiki/E10-import-zdjec-z-telefonu-po-mvp.md) Import zdjęć z telefonu (po MVP) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E10-ARCHIWUM-01 |
| Moduł | archiwum |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | Integracja |
| Priorytet | P2 |
| Estymacja | 2–4 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md) |
| Blokuje | [MEM-E10-PLAT-01](MEM-E10-PLAT-01-ekran-s07-import-zdjec-i-poczekalnia.md) |
| Kod roboczy | MEM-E10-ARCHIWUM-01 |
| Źródło | ES:C21 — S1:§16 „za zgodą użytkownika”; S2:C10 |

## Cel

Zaimportuj zdjęcia z telefonu (C21) — część epiku E10 „Import zdjęć z telefonu (po MVP)”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zaimportuj zdjęcia z telefonu (C21) (źródło: ES:C21 — S1:§16 „za zgodą użytkownika”; S2:C10)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E10-PLAT-01)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Użytkownik (autor wspomnień) wykonuje „Zaimportuj zdjęcia z telefonu”, then powstaje „Zdjęcia zaimportowane” (E30), „Duplikat pominięty (ten sam plik tej samej osoby)” (E31), „Zdjęcie bez geotagu trafiło do poczekalni” (E32). *Sprawdzenie: test jednostkowy handlera*
- [ ] Ponowienie tej samej operacji (ten sam klucz/zdarzenie) nie daje drugiego skutku; brak odpowiedzi drugiej strony kończy się stanem opisanym w kontraktach modułów. *Sprawdzenie: test integracyjny z podwójnym wywołaniem*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
