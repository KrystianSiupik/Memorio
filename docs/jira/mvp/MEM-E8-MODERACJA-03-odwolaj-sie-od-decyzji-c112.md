# MEM-E8-MODERACJA-03: Odwołaj się od decyzji (C112)

| Pole | Wartość |
|---|---|
| Jira | jeszcze nie założony (plan lokalny, D-068) |
| Epik | [E8](epiki/E8-zgloszenia-moderacja-i-nakazy-organow.md) Zgłoszenia, moderacja i nakazy organów |
| Wersja | MVP |
| Typ | Task |
| Etykiety | Backend, moderacja, MEM-E8-MODERACJA-03 |
| Moduł | moderacja |
| Warstwa | aplikacja |
| Rodzaj | zwykły |
| Klasa problemu | CRUD |
| Priorytet | P0 |
| Estymacja | 1–3 h |
| Pewność | wysoka |
| Status | gotowy |
| Zależy od | [MEM-E0-MODERACJA-01](MEM-E0-MODERACJA-01-szkielet-modulu-moderacja-struktura-publiczny-kont.md); [MEM-E8-MODERACJA-08](MEM-E8-MODERACJA-08-zapytanie-moje-zgloszenia-i-decyzje-moderacji-z-uz.md) |
| Blokuje | [MEM-E8-PLAT-02](MEM-E8-PLAT-02-ekran-s22-moje-zgloszenia-i-decyzje.md) |
| Kod roboczy | MEM-E8-MODERACJA-03 |
| Źródło | ES:C112 — S2:C2 „odwołanie dla autora” |

## Cel

Odwołaj się od decyzji (C112) — część epiku E8 „Zgłoszenia, moderacja i nakazy organów”.

## Kontekst

- Moduł **moderacja** — kluczowe pytanie: Co z tą treścią robimy po zgłoszeniu lub nakazie, i w jakim terminie?
- Czego moduł nie wie (nie wolno tego wymagać): prywatna treść poza zgłoszonym zakresem; relacje w albumach
- Projekt modułu: model procesów modułu moderacja
- Reguły: ADR 0001 (modularny monolit), 0019 (Onion + lekki CQRS), 0006 (outbox, Wolverine), 0011 (autoryzacja przez Dostęp)

## Zakres

- Odwołaj się od decyzji (C112) (źródło: ES:C112 — S2:C2 „odwołanie dla autora”)

## Poza zakresem

- ekrany i połączenia z innymi modułami — osobne tickety (MEM-E8-PLAT-02)

## Kryteria akceptacji

- [ ] Given aktor widzi: Moje zgłoszenia i decyzje moderacji (z uzasadnieniem), when Użytkownik (autor wspomnień) wykonuje „Odwołaj się od decyzji”, then powstaje „Odwołanie od decyzji złożone” (E114). *Sprawdzenie: test jednostkowy handlera*
- [ ] Niepoprawne dane wejściowe odrzucone walidacją z czytelnym komunikatem, bez zapisu. *Sprawdzenie: test jednostkowy schematu wejścia*

## Ryzyka i pytania

- HS-007: Odwołanie od decyzji moderacji (C112): kto je rozpatruje w trzyosobowym zespole, skoro ta sama osoba mogła wydać decyzję? — PO + prawnik
