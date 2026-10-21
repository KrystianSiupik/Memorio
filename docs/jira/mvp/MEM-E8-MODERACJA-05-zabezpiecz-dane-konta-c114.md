# MEM-E8-MODERACJA-05: Zabezpiecz dane konta (C114)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E8](epiki/E8-zgloszenia-moderacja-i-nakazy-organow.md) Zgłoszenia, moderacja i nakazy organów |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, moderacja, MEM-E8-MODERACJA-05 |
| Moduł | moderacja |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-MODERACJA-01](MEM-E0-MODERACJA-01-szkielet-modulu-moderacja-struktura-publiczny-kont.md) |
| Blokuje | [MEM-E8-MODERACJA-10](MEM-E8-MODERACJA-10-polityka-p20-zawsze-gdy-przyjdzie-nakaz-wydania-lu.md); [MEM-E8-PLAT-03](MEM-E8-PLAT-03-ekran-s26-panel-obslugi-zgloszenia-nakazy-sukcesja.md) |
| Kod roboczy | MEM-E8-MODERACJA-05 |
| Źródło | ES:C114 — S2:C11 C8/Z6 „zabezpieczenie danych konta bez ich ujawnienia” |

## Cel

Zabezpiecz dane konta (C114) — część epiku E8 „Zgłoszenia, moderacja i nakazy organów”.

## Kontekst

- Moduł **moderacja** — kluczowe pytanie: Co z tą treścią robimy po zgłoszeniu lub nakazie, i w jakim terminie?
- Czego moduł nie wie (nie wolno tego wymagać): prywatna treść poza zgłoszonym zakresem; relacje w albumach
- Projekt modułu: model procesów modułu moderacja
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Zabezpiecz dane konta (C114) (źródło: ES:C114 — S2:C11 C8/Z6 „zabezpieczenie danych konta bez ich ujawnienia”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E8-MODERACJA-10, MEM-E8-PLAT-03)

## Kryteria akceptacji

- [ ] Given poprawny stan wejściowy, when Obsługa GEO wykonuje „Zabezpiecz dane konta”, then powstaje „Dane konta zabezpieczone” (E117). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
