# MEM-E1-KONTO-04: Usuń konto definitywnie (C4)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E1](epiki/E1-konto-rejestracja-16-usuniecie-z-oknem-30-dni.md) Konto: rejestracja 16+, usunięcie z oknem 30 dni |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, konto, MEM-E1-KONTO-04 |
| Moduł | konto |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-KONTO-01](MEM-E0-KONTO-01-szkielet-modulu-konto-struktura-publiczny-kontrakt.md) |
| Blokuje | [MEM-E1-KONTO-05](MEM-E1-KONTO-05-cofnij-usuniecie-konta-c5.md); [MEM-E1-KONTO-07](MEM-E1-KONTO-07-polityka-p1-zawsze-gdy-okno-usuniecia-konta-minie-.md); [MEM-E1-KONTO-08](MEM-E1-KONTO-08-zdarzenie-integracyjne-konto-usuniete-e7-w-kontrak.md) |
| Kod roboczy | MEM-E1-KONTO-04 |
| Źródło | ES:C4 — S2:B3 |

## Cel

Usuń konto definitywnie (C4) — część epiku E1 „Konto: rejestracja 16+, usunięcie z oknem 30 dni”.

## Kontekst

- Moduł **konto** — kluczowe pytanie: Czy ta osoba jest aktywnym użytkownikiem i czy jej konto ma zostać?
- Czego moduł nie wie (nie wolno tego wymagać): co dokładnie wypadnie przy usunięciu (pyta inne konteksty)
- Projekt modułu: model procesów modułu konto
- Agregat AG11 „Usunięcie konta”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Usuń konto definitywnie (C4) (źródło: ES:C4 — S2:B3)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E1-KONTO-05, MEM-E1-KONTO-07, MEM-E1-KONTO-08)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when polityka/port wykonuje „Usuń konto definitywnie”, then powstaje „Konto usunięte” (E7). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Niezmiennik AG11 „Usunięcie konta”: Jeden wynik okna 30 dni: usunięte albo cofnięte — warunkowy zapis (D-024). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG11 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: warunkowy UPDATE stanu. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
