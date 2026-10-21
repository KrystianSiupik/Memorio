# MEM-E10-ARCHIWUM-04: Zapytanie: Poczekalnia „do przypisania” (R4)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E10](epiki/E10-import-zdjec-z-telefonu-po-mvp.md) Import zdjęć z telefonu (po MVP) |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, archiwum, MEM-E10-ARCHIWUM-04 |
| Moduł | archiwum |
| Warstwa | infrastruktura (query handler) |
| Rodzaj | zwykły |
| Klasa problemu | T&P |
| Priorytet | P2 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-ARCHIWUM-01](MEM-E0-ARCHIWUM-01-szkielet-modulu-archiwum-struktura-publiczny-kontr.md) |
| Blokuje | [MEM-E10-ARCHIWUM-02](MEM-E10-ARCHIWUM-02-przypisz-miejsce-grupie-zdjec-c22.md); [MEM-E10-ARCHIWUM-03](MEM-E10-ARCHIWUM-03-usun-zdjecie-z-poczekalni-c23.md); [MEM-E10-PLAT-01](MEM-E10-PLAT-01-ekran-s07-import-zdjec-i-poczekalnia.md) |
| Kod roboczy | MEM-E10-ARCHIWUM-04 |
| Źródło | ES:R4 — S2:C6 |

## Cel

Zapytanie: Poczekalnia „do przypisania” (R4) — część epiku E10 „Import zdjęć z telefonu (po MVP)”.

## Kontekst

- Moduł **archiwum** — kluczowe pytanie: Co ta osoba zapisała, gdzie i kiedy?
- Czego moduł nie wie (nie wolno tego wymagać): kto poza autorem widzi treść; w których albumach jest wspomnienie; plany i pieniądze; czy treść poszła do AI
- Projekt modułu: model procesów modułu archiwum
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zapytanie: Poczekalnia „do przypisania” (R4) (źródło: ES:R4 — S2:C6)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E10-ARCHIWUM-02, MEM-E10-ARCHIWUM-03, MEM-E10-PLAT-01)

## Kryteria akceptacji

- [ ] Given dane źródłowe, when zapytanie „Poczekalnia „do przypisania””, then wynik zawiera tylko treści dostępne dla pytającego (filtr Dostępu, ADR 0011). *Sprawdzenie: test integracyjny macierzy użytkownik × zasób*
- [ ] Projekcja nie ma logiki biznesowej i daje się odbudować z danych źródłowych. *Sprawdzenie: review + test przebudowy*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
