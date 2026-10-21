# MEM-E8-MODERACJA-02: Rozstrzygnij zgłoszenie (C111)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E8](epiki/E8-zgloszenia-moderacja-i-nakazy-organow.md) Zgłoszenia, moderacja i nakazy organów |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, moderacja, MEM-E8-MODERACJA-02 |
| Moduł | moderacja |
| Warstwa | domena + aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | RC (rywalizacja o zasoby) |
| Priorytet | P0 |
| Estymacja | 3–4 h |
| Pewność | średnia |
| Status | gotowy |
| Zależy od | [MEM-E0-MODERACJA-01](MEM-E0-MODERACJA-01-szkielet-modulu-moderacja-struktura-publiczny-kont.md); [MEM-E8-MODERACJA-07](MEM-E8-MODERACJA-07-zapytanie-kolejka-zgloszen-r9.md) |
| Blokuje | [MEM-E8-MODERACJA-11](MEM-E8-MODERACJA-11-zdarzenie-integracyjne-tresc-ukryta-e113-w-kontrak.md); [MEM-E8-PLAT-03](MEM-E8-PLAT-03-ekran-s26-panel-obslugi-zgloszenia-nakazy-sukcesja.md) |
| Kod roboczy | MEM-E8-MODERACJA-02 |
| Źródło | ES:C111 — S2:C2 „wynik z uzasadnieniem do obu stron”; S3:L10 (art. 17 DSA) |

## Cel

Rozstrzygnij zgłoszenie (C111) — część epiku E8 „Zgłoszenia, moderacja i nakazy organów”.

## Kontekst

- Moduł **moderacja** — kluczowe pytanie: Co z tą treścią robimy po zgłoszeniu lub nakazie, i w jakim terminie?
- Czego moduł nie wie (nie wolno tego wymagać): prywatna treść poza zgłoszonym zakresem; relacje w albumach
- Projekt modułu: model procesów modułu moderacja
- Agregat AG4 „Zgłoszenie”
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Rozstrzygnij zgłoszenie (C111) (źródło: ES:C111 — S2:C2 „wynik z uzasadnieniem do obu stron”; S3:L10 (art. 17 DSA))

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E8-MODERACJA-11, MEM-E8-PLAT-03)

## Kryteria akceptacji

- [ ] Given aktor widzi: Kolejka zgłoszeń, when Obsługa GEO wykonuje „Rozstrzygnij zgłoszenie”, then powstaje „Zgłoszenie rozstrzygnięte z uzasadnieniem” (E112), „Treść ukryta” (E113). *Sprawdzenie: test jednostkowy handlera i agregatu*
- [ ] Niezmiennik AG4 „Zgłoszenie”: Zgłoszenie ma najwyżej jedną decyzję; druga próba dostaje „już rozstrzygnięte” (S2:C2, [U] 2026-10-16). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Niezmiennik AG4 „Zgłoszenie”: Powtórne zgłoszenie tej samej treści przez tę samą osobę nie mnoży kolejki (S2:C2). *Sprawdzenie: test jednostkowy agregatu (przypadek odrzucenia)*
- [ ] Współbieżność: dwie równoległe komendy z macierzy konfliktów AG4 (aggregates.md) — jedna odrzucona albo stan spójny; strategia: optymistyczna wersja zgłoszenia. *Sprawdzenie: test integracyjny z dwoma równoległymi transakcjami*

## Ryzyka i pytania

- brak otwartych hot spotów dla tego elementu
